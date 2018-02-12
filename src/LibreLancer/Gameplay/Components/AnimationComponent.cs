﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2017
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using LibreLancer.Utf.Anm;
namespace LibreLancer
{
	public class AnimationComponent : GameComponent
	{
		class ActiveAnimation
		{
			public string Name;
			public Script Script;
			public double StartTime;
			public bool Loop;
		}

		AnmFile anm;
		List<ActiveAnimation> animations = new List<ActiveAnimation>();
		public AnimationComponent(GameObject parent, AnmFile animation) : base(parent)
		{
			anm = animation;
		}

		public void StartAnimation(string animationName, bool loop = true)
		{
			if (anm.Scripts.ContainsKey(animationName))
			{
				var sc = anm.Scripts[animationName];
				animations.Add(new ActiveAnimation() { Name = animationName, Script = sc, StartTime = totalTime, Loop = loop });
			}
			else
				FLLog.Error("Animation", animationName + " not present");
		}

		public bool HasAnimation(string animationName)
		{
			if (animationName == null) return false;
			return anm.Scripts.ContainsKey(animationName);
		}

		public event Action<string> AnimationCompleted;

		double totalTime = 0;
		public override void Update(TimeSpan time)
		{
			totalTime += time.TotalSeconds;
			int c = animations.Count;
			for (int i = animations.Count - 1; i >= 0; i--)
			{
				if (ProcessAnimation(animations[i]))
				{
					if (AnimationCompleted != null)
						AnimationCompleted(animations[i].Name);
					animations.RemoveAt(i);
				}
			}
			if (c > 0)
				Parent.UpdateCollision();
		}

		bool ProcessAnimation(ActiveAnimation a)
		{
			bool finished = true;
			foreach (var map in a.Script.ObjectMaps)
			{
				if (!ProcessObjectMap(map, a.StartTime, a.Loop))
					finished = false;
			}
			foreach (var map in a.Script.JointMaps)
			{
				if (!ProcessJointMap(map, a.StartTime, a.Loop))
					finished = false;
			}
			return finished;
		}

		bool ProcessObjectMap(ObjectMap om, double startTime, bool loop)
		{
			return false;
		}

		bool ProcessJointMap(JointMap jm, double startTime, bool loop)
		{
			var joint = Parent.CmpConstructs.Find(jm.ChildName);
			double t = totalTime - startTime;
			//looping?
			if (jm.Channel.Interval == -1)
			{
				var duration = jm.Channel.Frames[jm.Channel.FrameCount - 1].Time.Value;
				if (!loop && t >= duration)
					return true;
				else
					t = t % duration;
			}
			float t1 = 0;
			for (int i = 0; i < jm.Channel.Frames.Length - 1; i++)
			{
				var t0 = jm.Channel.Frames[i].Time ?? (jm.Channel.Interval * i);
				t1 = jm.Channel.Frames[i + 1].Time ?? (jm.Channel.Interval * (i + 1));
				var v1 = jm.Channel.Frames[i].JointValue;
				var v2 = jm.Channel.Frames[i + 1].JointValue;
				if (t >= t0 && t <= t1)
				{
					var dist = Math.Abs(v2 - v1);
					if (Math.Abs(t1 - t0) < 0.5f && dist > 1f)
					{
						//Deal with the horrible rotation scripts
						//Disable interpolation between e.g.  3.137246 to -3.13287
						//Don't remove this or things go crazy ~ Callum
						joint.Update((float)v2);
					}
					else
					{
						var x = (t - t0) / (t1 - t0);
						var val = v1 + (v2 - v1) * x;
						joint.Update((float)val);
					}
				}
			}
			return false;
		}
	}
}

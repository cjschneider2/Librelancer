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
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using LibreLancer.GameData.Items;
using LibreLancer.Fx;
namespace LibreLancer
{
	public class EngineComponent : GameComponent
	{
		public Engine Engine;
		public float Speed = 1f;
		List<AttachedEffect> fireFx = new List<AttachedEffect>();
		GameObject parent;
		public EngineComponent(GameObject parent, Engine engine, FreelancerGame game) : base(parent)
		{
			var fx = game.GameData.GetEffect(engine.FireEffect);
			var hps = parent.GetHardpoints();
			foreach (var hp in hps)
			{
				if (!hp.Name.Equals("hpengineglow", StringComparison.OrdinalIgnoreCase) &&
				    hp.Name.StartsWith("hpengine", StringComparison.OrdinalIgnoreCase))
				{
					fireFx.Add(new AttachedEffect(hp, new ParticleEffectRenderer(fx)));
				}
			}
			this.parent = parent;
			Engine = engine;
		}
		public override void Update(TimeSpan time)
		{
			for (int i = 0; i < fireFx.Count; i++)
				fireFx[i].Update(parent, time, Speed);
		}
		public override void Register(Physics.PhysicsWorld physics)
		{
            for (int i = 0; i < fireFx.Count; i++)
                Parent.ForceRenderCheck.Add(fireFx[i].Effect);
		}
		public override void Unregister(Physics.PhysicsWorld physics)
		{
            for (int i = 0; i < fireFx.Count; i++)
                Parent.ForceRenderCheck.Remove(fireFx[i].Effect);
		}

	}
}

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
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LibreLancer.Media
{
	public class AudioManager
	{
		//TODO: Heuristics to determine max number of sources
		const int MAX_SOURCES = 32;
		const int MAX_BUFFERS = 256;
		internal bool Ready = false;
		bool running = true;
		//ConcurrentQueues to avoid threading errors
		ConcurrentQueue<SoundEffectInstance> toAdd = new ConcurrentQueue<SoundEffectInstance> ();
		ConcurrentQueue<uint> freeSources = new ConcurrentQueue<uint>();
		internal Queue<uint> streamingSources = new Queue<uint>();
		internal Queue<uint> Buffers = new Queue<uint>();
		List<SoundEffectInstance> sfxInstances = new List<SoundEffectInstance>();
		internal ConcurrentQueue<Action> Actions = new ConcurrentQueue<Action>();
		internal List<StreamingSource> activeStreamers = new List<StreamingSource>();
		internal List<StreamingSource> toRemove = new List<StreamingSource>();
		public MusicPlayer Music { get; private set; }

		internal IUIThread UIThread;
		public AudioManager(IUIThread uithread)
		{
			Music = new MusicPlayer(this);
			this.UIThread = uithread;
			Thread AudioThread = new Thread (new ThreadStart (UpdateThread));
            AudioThread.Name = "Audio";
            AudioThread.Start();
        }

		bool AllocateSource(out uint source)
		{
			while (!Ready) { }
			if (freeSources.Count > 0)
			{
				return freeSources.TryDequeue(out source);
			}
			else
			{
				source = uint.MaxValue;
				return false;
			}
		}

		internal void ReturnSource(uint ID)
		{
			freeSources.Enqueue(ID);
		}
		internal void ReturnBuffer(uint ID)
		{
			lock(Buffers) Buffers.Enqueue(ID);
		}

		internal StreamingSource CreateStreaming(StreamingSound sound)
		{
			return new StreamingSource(this, sound, streamingSources.Dequeue());
		}

		public SoundData AllocateData()
		{
			while (!Ready) { }
			lock (Buffers)
			{
				if (Buffers.Count < 1) throw new Exception("Out of buffers");
				return new SoundData(Buffers.Dequeue(), this);
			}
		}

		void UpdateThread()
		{
			//Init context
			IntPtr dev = Alc.alcOpenDevice(null);
			IntPtr ctx = Alc.alcCreateContext(dev, IntPtr.Zero);
			Alc.alcMakeContextCurrent(ctx);
			for (int i = 0; i < MAX_SOURCES; i++)
			{
				freeSources.Enqueue(Al.GenSource());
			}
			for (int i = 0; i < MAX_BUFFERS; i++)
			{
				Buffers.Enqueue(Al.GenBuffer());
			}
			uint musicSource;
			for (int i = 0; i < 2; i++)
			{
				while (!freeSources.TryDequeue(out musicSource)) {}
				streamingSources.Enqueue(musicSource);
			}
			FLLog.Debug("Audio", "Audio initialised");
			Ready = true;
			while (running) {
				//insert into items to update
				while (toAdd.Count > 0) {
					SoundEffectInstance item;
					if (toAdd.TryDequeue (out item))
						sfxInstances.Add(item);
				}
				Action toRun;
				if (Actions.TryDequeue(out toRun)) toRun();
				//update SFX
				for (int i = sfxInstances.Count - 1; i >= 0; i--) {
					int state;
					Al.alGetSourcei(sfxInstances[i].ID, Al.AL_SOURCE_STATE, out state);
					if (state != Al.AL_PLAYING)
					{
						if (sfxInstances[i].Dispose != null)
							sfxInstances[i].Dispose.Dispose();
						freeSources.Enqueue(sfxInstances[i].ID);
						sfxInstances.RemoveAt(i);
						i--;
					}
				}
				//update Streaming
				foreach (var item in activeStreamers)
					item.Update();
				foreach (var item in toRemove)
				{
					activeStreamers.Remove(item);
					if (item.Stopped != null)
						item.OnStopped();
				}
				Thread.Sleep (5);
			}
			//Delete context
			Alc.alcMakeContextCurrent(IntPtr.Zero);
			Alc.alcDestroyContext(ctx);
			Alc.alcCloseDevice(ctx);
		}

		struct SoundEffectInstance
		{
			public uint ID;
			public SoundData Dispose;
		}

		public void PlaySound(Stream stream, float volume = 1f)
		{
			uint src;
			if (!AllocateSource(out src)) return;
			var data = AllocateData();
			data.LoadStream(stream);
			Al.alSourcei(src, Al.AL_BUFFER, (int)data.ID);
			Al.alSourcef(src, Al.AL_GAIN, volume);
			Al.alSourcePlay(src);
			toAdd.Enqueue(new SoundEffectInstance() { ID = src, Dispose = data });
		}

		public void PlaySound(SoundData data, float volume = 1f)
		{
			uint src;
			if (!AllocateSource(out src)) return;
			Al.alSourcei(src, Al.AL_BUFFER, (int)data.ID);
			Al.alSourcef(src, Al.AL_GAIN, volume);
			Al.alSourcePlay(src);
			toAdd.Enqueue(new SoundEffectInstance() { ID = src });
		}

		public void Dispose()
		{
			running = false;
		}

		internal void RunActionBlocking(Action action)
		{
			bool ran = false;
			Actions.Enqueue(() =>
			{
				action();
                ran = true;
			});
			while (!ran) { Thread.Sleep(1);  }; //sleep stops hang on Windows Release builds
		}
	}
}


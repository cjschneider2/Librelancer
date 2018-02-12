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
using LibreLancer.Utf.Ale;

namespace LibreLancer.Fx
{
	public class FxAppearance : FxNode
	{
		public PhysicsDebugRenderer Debug;
		public bool DrawNormals = false;
		public FxAppearance (AlchemyNode ale) : base(ale)
		{
		}
		public virtual void Draw(ref Particle particle, float globaltime, NodeReference reference, ResourceManager res, Billboards billboards, ref Matrix4 transform, float sparam)
		{
			
		}
		public virtual void OnParticleSpawned(int idx, NodeReference reference, ParticleEffectInstance instance)
		{
		}
	}
}


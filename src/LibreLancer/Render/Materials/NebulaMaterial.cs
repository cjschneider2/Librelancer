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
using LibreLancer.Vertices;
using LibreLancer.Utf.Mat;
namespace LibreLancer
{
	public class NebulaMaterial : RenderMaterial
	{
		public string DtSampler;
		public SamplerFlags DtFlags;
		public NebulaMaterial ()
		{
		}
		ShaderVariables GetShader(IVertexType vtype)
		{
			switch (vtype.GetType ().Name) {
			case "VertexPositionNormalColorTexture":
				return ShaderCache.Get (
					"PositionColorTexture.vs",
					"Nebula_PositionColorTexture.frag"
				);
			case "VertexPositionNormalTexture":
				return ShaderCache.Get(
					"PositionTextureFlip.vs",
					"Nebula_PositionColorTexture.frag"
				);
			default:
				throw new NotImplementedException ();
			}
		}
		public override void Use(RenderState rstate, IVertexType vertextype, ref Lighting lights)
		{
            if (Camera == null)
                return;
			//fragment shader you multiply tex sampler rgb by vertex color and alpha the same (that is should texture have alpha of its own, sometimes they may as well)
			rstate.BlendMode = BlendMode.Additive;
			var shader = GetShader(vertextype);
			shader.SetWorld(ref World);
            shader.SetViewProjection(Camera);
			//Dt
			shader.SetDtSampler(0);
			BindTexture (rstate, 0, DtSampler, 0, DtFlags);
			shader.UseProgram ();
		}

		public override void ApplyDepthPrepass(RenderState rstate)
		{
			throw new InvalidOperationException();
		}

		public override bool IsTransparent
		{
			get
			{
				return true;
			}
		}
	}
}


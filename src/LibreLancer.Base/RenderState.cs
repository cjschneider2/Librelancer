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


namespace LibreLancer
{
	public enum DepthFunction
	{
		Less = 0x201,
		LessEqual = 0x203
	}
	//OpenGL Render States
	public class RenderState
	{
		static internal RenderState Instance;
		public Color4 ClearColor {
			get {
				return clearColor;
			} set {
				if (value == clearColor)
					return;
				clearColor = value;
				clearDirty = true;
			}
		}

		public bool Wireframe {
			get {
				return isWireframe;
			} set {
				if (value == isWireframe)
					return;
				isWireframe = value;
				wireframeDirty = true;
			}
		}

		public bool DepthEnabled {
			get {
				return depthEnabled;
			} set {
				if (depthEnabled == value)
					return;
				depthEnabled = value;
				depthDirty = true;
			}
		}

		public TextureFiltering PreferredFilterLevel { get; set; }

		public bool DepthWrite
		{
			get
			{
				return depthwrite;
			} set
			{
				if (depthwrite == value)
					return;
				depthwrite = value;
				depthwritedirty = true;
			}
		}
		public BlendMode BlendMode {
			get {
				return blend;
			} set {
				if (blend == value)
					return;
				blendDirty = true;
				blend = value;
			}
		}

		public DepthFunction DepthFunction
		{
			set
			{
				GL.DepthFunc((int)value);
			}
		}

		public bool Cull {
			get {
				return cull;
			} set {
				if (cull == value)
					return;
				cull = value;
				cullDirty = true;
			}
		}

		CullFaces requestedCull = CullFaces.Back;
		CullFaces cullFace = CullFaces.Back;
		public CullFaces CullFace
		{
			get {
				return requestedCull;
			} set {
				requestedCull = value;
			}
		}

		internal void Trash()
		{
			cullDirty = true;
			clearDirty = true;
			wireframeDirty = true;
			depthDirty = true;
			blendDirty = true;
		}
		bool cull = true;
		bool cullDirty = false;

		Color4 clearColor = Color4.Black;
		bool clearDirty = false;

		bool isWireframe = false;
		bool wireframeDirty = false;

		bool depthEnabled = true;
		bool depthDirty = false;

		BlendMode blend = BlendMode.Normal;
		bool blendDirty = false;
		bool blendEnable = true;

		bool depthwrite = true;
		bool depthwritedirty = false;
		public RenderState ()
		{
			GL.ClearColor (0f, 0f, 0f, 1f);
			GL.Enable (GL.GL_BLEND);
			GL.Enable (GL.GL_DEPTH_TEST);
			GL.BlendFunc (GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			GL.DepthFunc (GL.GL_LEQUAL);
			GL.Enable (GL.GL_CULL_FACE);
			GL.CullFace (GL.GL_BACK);
			Instance = this;
			PreferredFilterLevel = TextureFiltering.Trilinear;
		}

		public void SetViewport(int x, int y, int w, int h)
		{
			GL.Viewport(x,y,w,h);
		}

		public void ClearAll()
		{
			Apply();
			GL.Clear (GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
		}

		public void ClearDepth()
		{
			GL.Clear (GL.GL_DEPTH_BUFFER_BIT);
		}
		public void Apply()
		{
			if (clearDirty) {
				GL.ClearColor (clearColor.R, clearColor.G, clearColor.B, clearColor.A);
				clearDirty = false;
			}

			if (wireframeDirty) {
				GL.PolygonMode (GL.GL_FRONT_AND_BACK, isWireframe ? GL.GL_LINE : GL.GL_FILL);
				if (isWireframe)
					GL.LineWidth(1.7f);
				else
					GL.LineWidth(1);
				wireframeDirty = false;
			}

			if (depthDirty) {
				if (depthEnabled)
					GL.Enable (GL.GL_DEPTH_TEST);
				else
					GL.Disable (GL.GL_DEPTH_TEST);
				depthDirty = false;
			}

			if (blendDirty) {
				if (!blendEnable && blend != BlendMode.Opaque) {
					GL.Enable (GL.GL_BLEND);
					blendEnable = true;
				}
				if (blendEnable && blend == BlendMode.Opaque) {
					GL.Disable (GL.GL_BLEND);
					blendEnable = false;
				}
				switch (blend)
				{
					case BlendMode.Normal:
						GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
						break;
					case BlendMode.Additive:
						GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE);
						break;
					case BlendMode.OneInvSrcColor:
						GL.BlendFunc (GL.GL_ONE, GL.GL_ONE_MINUS_SRC_COLOR);
						break;
				}
				blendDirty = false;
			}
			if (cullDirty) {
				if (cull)
					GL.Enable (GL.GL_CULL_FACE);
				else
					GL.Disable (GL.GL_CULL_FACE);
				cullDirty = false;
			}
			if (requestedCull != cullFace)
			{
				cullFace = requestedCull;
				GL.CullFace(cullFace == CullFaces.Back ? GL.GL_BACK : GL.GL_FRONT);
			}
			if (depthwritedirty)
			{
				GL.DepthMask(depthwrite);
				depthwritedirty = false;
			}
		}
	}
}


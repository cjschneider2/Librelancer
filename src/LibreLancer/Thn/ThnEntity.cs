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
	public class ThnEntity
	{
		public string Name;
		public string Template = "";
		public EntityTypes Type;
		public Vector3? Ambient;
		public Vector3? Up;
		public Vector3? Front;
		public int LightGroup;
		public int SortGroup;
		public int UserFlag;
		public string MeshCategory;
		public Vector3? Position;
		public Matrix4? RotationMatrix;
		public float? FovH;
		public float? HVAspect;
		public ThnLightProps LightProps;
		public MotionPath Path;
		public ThnObjectFlags ObjectFlags;
		public bool NoFog = false;
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", Name, Type);
		}
	}
}


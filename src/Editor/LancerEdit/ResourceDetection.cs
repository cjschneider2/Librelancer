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
 * Portions created by the Initial Developer are Copyright (C) 2013-2018
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using LibreLancer;
using LibreLancer.Utf.Cmp;
using LibreLancer.Utf.Mat;
namespace LancerEdit
{
	static class ResourceDetection
	{
		public static void DetectDrawable(string name, IDrawable drawable, ResourceManager res, List<MissingReference> missing, List<uint> matrefs, List<string> texrefs)
		{
			if (drawable is CmpFile)
			{
				var cmp = (CmpFile)drawable;
				foreach (var part in cmp.Parts) DetectResourcesModel(part.Value.Model, name + ", " + part.Value.Model.Path, res, missing, matrefs, texrefs);
			}
			if (drawable is ModelFile)
			{
				DetectResourcesModel((ModelFile)drawable, name, res, missing, matrefs, texrefs);
			}
			if (drawable is SphFile)
			{
				var sph = (SphFile)drawable;
				for (int i = 0; i < sph.SideMaterials.Length; i++)
				{
                    if (!sph.SideMaterials[i].Loaded) sph.SideMaterials[i] = null;
					if (sph.SideMaterials[i] == null)
					{
						var str = "Material: " + sph.SideMaterialNames[i];
						if (!HasMissing(missing, str)) missing.Add(new MissingReference(str, string.Format("{0} M{1}", name, i)));
					}
					else
					{
						var crc = CrcTool.FLModelCrc(sph.SideMaterialNames[i]);
						if (!matrefs.Contains(crc)) matrefs.Add(crc);
						DoMaterialRefs(sph.SideMaterials[i], res, missing, texrefs, string.Format(" - {0} M{1}", name, i));
					}
				}
			}
		}
		static void DetectResourcesModel(ModelFile mdl, string mdlname, ResourceManager res, List<MissingReference> missing, List<uint> matrefs, List<string> texrefs)
		{
			var lvl = mdl.Levels[0];
			for (int i = lvl.StartMesh; i < (lvl.StartMesh + lvl.MeshCount); i++)
			{
				if (lvl.Mesh.Meshes[i].Material == null)
				{
					var str = "Material: 0x" + lvl.Mesh.Meshes[i].MaterialCrc.ToString("X");
					if (!HasMissing(missing, str)) missing.Add(new MissingReference(str, string.Format("{0}, VMesh(0x{1:X}) #{2}", mdlname, lvl.MeshCrc, i)));
				}
				else
				{
					if (!matrefs.Contains(lvl.Mesh.Meshes[i].MaterialCrc)) matrefs.Add(lvl.Mesh.Meshes[i].MaterialCrc);
					var m = lvl.Mesh.Meshes[i].Material;
					DoMaterialRefs(m, res, missing, texrefs, string.Format(" - {0} VMesh(0x{1:X}) #{2}", mdlname, lvl.MeshCrc, i));
				}
			}
		}

		static void DoMaterialRefs(Material m, ResourceManager res, List<MissingReference> missing, List<string> texrefs, string refstr)
		{
			RefTex(m.DtName, res, missing, texrefs, m.Name, refstr);
			if (m.Render is NomadMaterial)
			{
				var nt = m.NtName ?? "NomadRGB1_NomadAlpha1";
				RefTex(nt, res, missing, texrefs, m.Name, refstr);
			}
			RefTex(m.EtName, res, missing, texrefs, m.Name, refstr);
			RefTex(m.DmName, res, missing, texrefs, m.Name, refstr);
			RefTex(m.Dm1Name, res, missing, texrefs, m.Name, refstr);
		}

		static void RefTex(string tex, ResourceManager res, List<MissingReference> missing, List<string> texrefs, string mName, string refstr)
		{
			if (tex != null)
			{
				if (!HasTexture(texrefs, tex)) texrefs.Add(tex);
				if (res.FindTexture(tex) == null)
				{
					var str = "Texture: " + tex;
					if (!HasMissing(missing, str)) missing.Add(new MissingReference(str, mName + refstr));
				}
			}
		}

		public static bool HasTexture(List<string> refs, string item)
		{
			foreach (string tex in refs)
				if (tex.Equals(item, StringComparison.InvariantCultureIgnoreCase)) return true;
			return false;
		}
		public static bool HasMissing(List<MissingReference> missing, string item)
		{
			foreach (var m in missing)
				if (m.Missing == item) return true;
			return false;
		}
	}
}

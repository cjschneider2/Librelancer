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
 * The Original Code is Starchart code (http://flapi.sourceforge.net/).
 * Data structure from Freelancer UTF Editor by Cannon & Adoxa, continuing the work of Colin Sanby and Mario 'HCl' Brito (http://the-starport.net)
 * 
 * The Initial Developer of the Original Code is Malte Rupprecht (mailto:rupprema@googlemail.com).
 * Portions created by the Initial Developer are Copyright (C) 2011
 * the Initial Developer. All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using LibreLancer.Utf.Vms;

namespace LibreLancer.Utf.Mat
{
	public class TxmFile : UtfFile
	{
		public Dictionary<string, TextureData> Textures { get; private set; }
		public Dictionary<string, TexFrameAnimation> Animations { get; private set; }
        public TxmFile()
        {
            Textures = new Dictionary<string, TextureData>();
			Animations = new Dictionary<string, TexFrameAnimation>();
        }

        public TxmFile(IntermediateNode textureLibraryNode)
            : this()
        {
            setTextures(textureLibraryNode);
        }

        private void setTextures(IntermediateNode textureLibraryNode)
        {
            foreach (IntermediateNode textureNode in textureLibraryNode)
            {
				LeafNode child = null;
				bool isTexture = true;
				bool isTgaMips = false;
				if (textureNode.Count == 1)
				{
					child = textureNode[0] as LeafNode;
				}
				else
				{
					//TODO: Mipmapping
					foreach (var node in textureNode)
					{
						var n = node.Name.ToLowerInvariant().Trim();
						if (n == "mip0")
						{
							child = node as LeafNode;
							isTgaMips = true;
						}
						if (n == "mips")
						{
							child = node as LeafNode;
							isTgaMips = false;
							break;
						}
						if (n == "fps")
						{
							isTexture = false;
							break;
						}
					}
				}
				if (isTexture)
				{
					if (child == null) throw new Exception("Invalid texture library");

					TextureData data = new TextureData(child, textureNode.Name, isTgaMips);
					if (isTgaMips)
					{
						foreach (var node in textureNode)
							data.SetLevel(node);
					}
					if (data == null) throw new Exception("Invalid texture library");

					string key = textureNode.Name;
					if (Textures.ContainsKey(key))
					{
						FLLog.Error("Txm", "Duplicate texture " + key + " in texture library");
					} else 
						Textures.Add(key, data);
				}
				else {
					Animations.Add(textureNode.Name, new TexFrameAnimation(textureNode));
				}
            }
        }
    }
}
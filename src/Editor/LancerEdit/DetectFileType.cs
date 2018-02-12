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
using System.IO;
namespace LancerEdit
{
	enum FileType
	{
		Utf,
		Ini
	}
	class DetectFileType
	{
		public static FileType Detect(string filename)
		{
			using (var reader = new BinaryReader(File.OpenRead(filename)))
			{
				var str = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));
				if (str == "UTF ") if (reader.ReadInt32() == 257) return FileType.Utf;
				return FileType.Ini;
			}
		}
	}
}

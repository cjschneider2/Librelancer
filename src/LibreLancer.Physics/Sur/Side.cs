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

namespace LibreLancer.Physics.Sur
{
	//TODO: Sur - ???
	struct Side
	{
		public bool Flag;
		public ushort Offset;
		public ushort Vertex;
		public Side(BinaryReader reader)
		{
			Vertex = reader.ReadUInt16();
			var arg = reader.ReadUInt16();
			Offset = (ushort)(arg & 0x7FFF);
			Flag = ((arg >> 15) & 1) == 1;
		}
	}
}
	
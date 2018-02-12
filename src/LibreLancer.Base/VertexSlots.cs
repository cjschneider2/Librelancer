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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreLancer
{
    public class VertexSlots
    {
        public const int Position = 0;
        public const int Color = 1;
		public const int Normal = 2;
        public const int Texture1 = 3;
		public const int Texture2 = 4;
		public const int Dimensions = 5;
		public const int Right = 6;
		public const int Up = 7;
		public const int BoneFirst = 8;
		public const int BoneCount = 9;
    }
}

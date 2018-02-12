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
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
		public Rectangle(int x, int y, int w, int h)
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}
		public bool Contains(int x, int y)
		{
			return (
			    x >= X &&
			    x <= (X + Width) &&
			    y >= Y &&
			    y <= (Y + Height)
			);
		}
		public bool Contains(Point pt)
		{
			return Contains (pt.X, pt.Y);
		}

		public bool Intersects(Rectangle other)
		{
			return (other.X < (X + Width) &&
					X < (other.X + other.Width) &&
					other.Y < (Y + Height) &&
					Y < (other.Y + other.Height));
		}

		public override bool Equals(object obj)
		{
			if (obj is Rectangle)
			{
				return ((Rectangle)obj) == this;
			}
			return false;
		}
		public static bool operator ==(Rectangle a, Rectangle b)
		{
			return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
		}
		public static bool operator !=(Rectangle a, Rectangle b)
		{
			return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
		}
    }
}


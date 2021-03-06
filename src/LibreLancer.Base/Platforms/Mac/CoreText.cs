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
using System.Runtime.InteropServices;
namespace LibreLancer.Platforms.Mac
{
	static class CoreText
	{
		[DllImport(Cocoa.CoreTextPath)]
		public static extern IntPtr CTFontDescriptorCreateWithAttributes(IntPtr attributes);

		[DllImport(Cocoa.CoreTextPath)]
		public static extern IntPtr CTFontDescriptorCopyAttribute(IntPtr descriptor, IntPtr attribute);

		[DllImport(Cocoa.CoreTextPath)]
		public static extern void CFRelease(IntPtr obj);

		public static IntPtr kCTFontURLAttribute = Cocoa.GetStringConstant (Cocoa.CoreTextLibrary, "kCTFontURLAttribute");
		public static IntPtr kCTFontTraitsAttribute = Cocoa.GetStringConstant(Cocoa.CoreTextLibrary, "kCTFontTraitsAttribute");
		public static IntPtr kCTFontAttributeName = Cocoa.GetStringConstant(Cocoa.CoreTextLibrary, "kCTFontAttributeName");
		public static IntPtr kCTFontFamilyNameAttribute = Cocoa.GetStringConstant(Cocoa.CoreTextLibrary, "kCTFontFamilyNameAttribute");
		public static IntPtr kCTFontSymbolicTrait = Cocoa.GetStringConstant(Cocoa.CoreTextLibrary, "kCTFontSymbolicTrait");
	}
	public enum CTFontSymbolicTraits : uint
	{
		None = 0,
		Italic = (1 << 0),
		Bold = (1 << 1)
	}
}


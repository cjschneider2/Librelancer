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
 * Portions created by the Initial Developer are Copyright (C) 2013-2017
 * the Initial Developer. All Rights Reserved.
 */
using System;
namespace LibreLancer
{
	public class HudNumberBoxElement : HudModelElement
	{
		public float Velocity = 0;
		public float ThrustAvailable = 1f;
		public Color4 TextColor = new Color4(160, 196, 210, 255);
		public Color4 CriticalColor = Color4.Red;
		public int CriticalThreshold = 20;
		Font font;
		public HudNumberBoxElement(UIManager manager) : base(manager, "hud_numberboxes.cmp", 0.01f, -0.952f, 1.93f, 2.5f)
		{
			font = manager.Game.Fonts.GetSystemFont("Agency FB");
		}

		public override void DrawText()
		{
			var thrustbox = FromScreenRect(-0.2925f, -0.93f, 0.077f, 0.055f);
			var speedbox = FromScreenRect(0.231f, -0.93f, 0.077f, 0.055f);
			var sz = GetTextSize(thrustbox.Height);
			var pct = (int)MathHelper.Clamp(ThrustAvailable * 100, 0, 100);
			DrawTextCentered(font, sz, pct + "%", thrustbox, pct <= CriticalThreshold ? CriticalColor : TextColor);
			DrawTextCentered(font, sz, ((int)Velocity).ToString(), speedbox, TextColor);
		}

		float GetTextSize(float px)
		{
			return (px * (72.0f / 96.0f));
		}
	}
}

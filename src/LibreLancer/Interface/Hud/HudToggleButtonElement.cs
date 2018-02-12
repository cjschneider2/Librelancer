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
	public enum ToggleState
	{
		Active,
		Inactive
	}
	public class HudToggleButtonElement : UIElement
	{
		protected IDrawable drawableA;
		IDrawable drawableB;

		public ToggleState State = ToggleState.Inactive;

		public HudToggleButtonElement(UIManager manager, string pathA, string pathB, float x, float y, float scaleX, float scaleY) : base(manager)
		{
			UIScale = new Vector2(scaleX, scaleY);
			UIPosition = new Vector2(x, y);
			drawableA = manager.Game.ResourceManager.GetDrawable(manager.Game.GameData.ResolveDataPath(pathA));
			if(pathB != null)
				drawableB = manager.Game.ResourceManager.GetDrawable(manager.Game.GameData.ResolveDataPath(pathB));
		}

		public override void DrawBase()
		{
			if (State == ToggleState.Active)
			{
				drawableA.Update(IdentityCamera.Instance, TimeSpan.Zero, TimeSpan.FromSeconds(Manager.Game.TotalTime));
				drawableA.Draw(Manager.Game.RenderState, GetWorld(UIScale, Position), Lighting.Empty);
			}
			else
			{
				drawableB.Update(IdentityCamera.Instance, TimeSpan.Zero, TimeSpan.FromSeconds(Manager.Game.TotalTime));
				drawableB.Draw(Manager.Game.RenderState, GetWorld(UIScale, Position), Lighting.Empty);
			}
		}

		public override void WasClicked()
		{
			if (Tag != null) Manager.OnClick(Tag);
		}

		public override void DrawText()
		{
		}

		public override bool TryGetHitRectangle(out Rectangle rect)
		{
			var topleft = Manager.ScreenToPixel(Position.X - 0.015f * UIScale.X, Position.Y + 0.015f * UIScale.Y);
			var bottomRight = Manager.ScreenToPixel(Position.X + 0.015f * UIScale.X, Position.Y - 0.015f * UIScale.Y);
			rect = new Rectangle(
				(int)topleft.X,
				(int)topleft.Y,
				(int)(bottomRight.X - topleft.X),
				(int)(bottomRight.Y - topleft.Y)
			);
			return true;
		}
	}
}

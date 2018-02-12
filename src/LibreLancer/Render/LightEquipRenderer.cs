/* The contents of this file are subject to the Mozilla Public License
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
using LibreLancer.Utf.Ale;
using LibreLancer.GameData.Items;
namespace LibreLancer
{
    public class LightEquipRenderer : ObjectRenderer
    {
        const float BASE_SIZE = 10f;
        Vector3 pos;
        SystemRenderer sys;
        LightEquipment equip;
        static Random rnd = new Random();
        public LightEquipRenderer(LightEquipment e)
        {
            equip = e;
            colorBulb = equip.Color;
            colorGlow = equip.GlowColor;
        }
        static TextureShape bulbshape = null;
        static Texture2D bulbtex = null;
        static TextureShape shineshape = null;
        static Texture2D shinetex = null;
        static bool frameStart = false;
        public static void FrameStart()
        {
            frameStart = true;
        }
        const float CULL_DISTANCE = 20000;
        const float CULL = CULL_DISTANCE * CULL_DISTANCE;
        public override void Draw(ICamera camera, CommandBuffer commands, SystemLighting lights, NebulaRenderer nr)
        {
            if (frameStart)
            {
                sys.Game.ResourceManager.TryGetShape("bulb", out bulbshape);
                bulbtex = (Texture2D)sys.Game.ResourceManager.FindTexture(bulbshape.Texture);
                sys.Game.ResourceManager.TryGetShape("shine", out shineshape);
                shinetex = (Texture2D)sys.Game.ResourceManager.FindTexture(shineshape.Texture);
                frameStart = false;
            }
            sys.Game.Billboards.Draw(
                shinetex,
                pos,
                new Vector2(equip.GlowSize) * 2,
                new Color4(colorGlow, 1f),
                new Vector2(shineshape.Dimensions.X, shineshape.Dimensions.Y),
                new Vector2(shineshape.Dimensions.X + shineshape.Dimensions.Width, shineshape.Dimensions.Y),
                new Vector2(shineshape.Dimensions.X, shineshape.Dimensions.Y + shineshape.Dimensions.Height),
                new Vector2(shineshape.Dimensions.X + shineshape.Dimensions.Width, shineshape.Dimensions.Y + shineshape.Dimensions.Height),
                0,
                SortLayers.LIGHT_SHINE,
                BlendMode.Additive
            );
            sys.Game.Billboards.Draw(
                bulbtex,
                pos,
                new Vector2(equip.BulbSize) * 2,
                new Color4(colorBulb, 1),
                new Vector2(bulbshape.Dimensions.X, bulbshape.Dimensions.Y),
                new Vector2(bulbshape.Dimensions.X + bulbshape.Dimensions.Width, bulbshape.Dimensions.Y),
                new Vector2(bulbshape.Dimensions.X, bulbshape.Dimensions.Y + bulbshape.Dimensions.Height),
                new Vector2(bulbshape.Dimensions.X + bulbshape.Dimensions.Width, bulbshape.Dimensions.Y + bulbshape.Dimensions.Height),
                0,
                SortLayers.LIGHT_BULB,
                BlendMode.Additive
            );

        }

        double timer = 0;
        bool lt_on = true;
        Color3f colorBulb;
        Color3f colorGlow;
        public override void Update(TimeSpan time, Vector3 position, Matrix4 transform)
        {
            if (sys == null)
                return;
            pos = position;
            if (equip.Animated)
            {
                timer -= time.TotalSeconds;
                if (timer < 0)
                {
                    if (lt_on)
                    {
                        timer = equip.BlinkDuration;
                        colorBulb = equip.Color;
                        colorGlow = equip.GlowColor;
                    }
                    else
                    {
                        timer = equip.AvgDelay + rnd.NextFloat(-(equip.AvgDelay / 2f), +(equip.AvgDelay / 2f));
                        colorBulb = equip.MinColor;
                        colorGlow = equip.MinColor;
                    }
                    lt_on = !lt_on;
                }
            }
        }

        public override bool PrepareRender(ICamera camera, NebulaRenderer nr, SystemRenderer sys)
        {
            var visible = (
                VectorMath.DistanceSquared(camera.Position, pos) < CULL &&
                camera.Frustum.Intersects(new BoundingSphere(pos, equip.BulbSize * 3))
            );
            this.sys = sys;
            if(visible) {
                sys.AddObject(this);
                return true;
            } else {
                return false;
            }
            /*if (lt_on && camera.Frustum.Intersects(new BoundingSphere(pos, 100)))
			{
				sys.PointLightDX(pos, 50, new Color4(equip.GlowColor, 1), new Vector3(1, 0.01f, 0.000055f));
			}*/ //TODO: Configurable
        }
    }
}


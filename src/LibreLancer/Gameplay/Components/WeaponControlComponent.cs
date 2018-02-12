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
namespace LibreLancer
{
    //For objects that shoot
    public class WeaponControlComponent : GameComponent
    {
        public Vector3 AimPoint = Vector3.Zero;
        public WeaponControlComponent(GameObject parent) : base(parent)
        {
        }

        public override void FixedUpdate(TimeSpan time)
        {
            if (AimPoint != Vector3.Zero)
            {
                foreach (var wp in Parent.GetChildComponents<WeaponComponent>())
                {
                    wp.AimTowards(AimPoint, time);
                }
            }
        }

        public void FireAll()
        {
            foreach(var wp in Parent.GetChildComponents<WeaponComponent>())
            {
                wp.Fire(AimPoint);
            }
        }

    }
}

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
using BulletSharp;
using BM = BulletSharp.Math;
namespace LibreLancer.Physics
{
    public abstract class Collider : IDisposable
    {
        internal abstract CollisionShape BtShape { get; }
        public virtual void Dispose()
        {
            if(BtShape != null) {
                BtShape.Dispose();
            }
        }
        public float Radius {
            get {
                BtShape.GetBoundingSphere(out _, out float r);
                return r;
            }
        }

    }
}

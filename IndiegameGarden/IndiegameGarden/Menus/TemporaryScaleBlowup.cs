using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace IndiegameGarden.Menus
{
    public class TemporaryScaleBlowup: Gamelet
    {
        public TemporaryScaleBlowup()
        {
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            double x = MathHelper.TwoPi * 0.08f * SimTime;
            (Parent as Motion).ScaleModifier = 1f + (float) Math.Sin( x);
            if (x > Math.PI)
                Delete = true;
        }
    }
}

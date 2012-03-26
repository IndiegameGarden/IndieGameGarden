using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TTengine.Core;

namespace IndiegameGarden.Menus
{
    public class GrowThenShrinkBehavior: Gamelet
    {
        public float sz1, sz2, spd1, spd2;
        GameThumbnail thumb;
        bool isInShrink = false;

        public GrowThenShrinkBehavior(float size1, float size2, float speed1, float speed2)
        {
            sz1 = size1;
            sz2 = size2;
            spd1 = speed1;
            spd2 = speed2;
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
            thumb = Parent as GameThumbnail;
            thumb.MotionB.ScaleTarget = sz1;
            thumb.MotionB.ScaleSpeed = spd1;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (thumb.Motion.Scale == sz1 && !isInShrink)
            { // target reached?
                thumb.MotionB.ScaleTarget = sz2;
                thumb.MotionB.ScaleSpeed = spd2;
                isInShrink = true;
            }
        }


    }
}

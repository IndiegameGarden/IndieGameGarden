// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TTengine.Core;

namespace IndiegameGarden.Menus
{
    public class DodgeSidewaysBehavior: Gamelet
    {
        float ampl, layerDepthWhileMoving, layerDepthOld, moveDuration;

        public DodgeSidewaysBehavior(float ampl, float layerDepthWhileMoving)
        {
            this.moveDuration = 0.3f;
            this.ampl = ampl;
            this.layerDepthWhileMoving = layerDepthWhileMoving;
        }

        // set a new layerdepth of parent, when attached to parent.
        protected override void OnNewParent()
        {
            base.OnNewParent();
            layerDepthOld = Parent.LayerDepth;
            //Parent.LayerDepth = layerDepthWhileMoving;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            Vector2 posDelta = new Vector2( ampl * (float)Math.Sin( MathHelper.TwoPi * 0.5f/moveDuration * SimTime ) , 0f);
            Parent.PositionModifier += posDelta;

            if (SimTime >= moveDuration)
            {
                // delete myself and restore the old layerdepth of Parent again.
                //Parent.LayerDepth = layerDepthOld;
                Delete = true;
            }
        }
    }
}

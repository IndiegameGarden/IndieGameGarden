// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

namespace IndiegameGarden.Menus
{
    public class GameThumbnailCursor: EffectSpritelet
    {
        int xSel, ySel;

        public int X
        {
            get { return xSel; }
            set { xSel = value; }
        }

        public int Y
        {
            get { return ySel; }
            set { ySel = value; }
        }

        public GameThumbnailCursor()
            : base("WhiteTexture","GameThumbnailCursor")
        {
            LayerDepth = 0f;
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();
            // check parent to select texture
            // TODO Texture = (Parent as GameThumbnail).Texture;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }

        public bool GameletInRange(Gamelet g)
        {
            float d = (g.Position - this.Position).Length();
            if (d < 0.3)
                return true;
            return false;

        }

    }
}

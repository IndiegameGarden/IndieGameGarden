// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt
﻿
using System;
using System.Collections.Generic;
using System.Text;

using TTengine.Core;

namespace IndiegameGarden.Menus
{
    public class LoadingText: Gamelet
    {
        bool isFirstDraw = true;
        GameTextBox tbox;

        public LoadingText()
        {
            tbox = new GameTextBox("Loading...");
            tbox.Position = new Microsoft.Xna.Framework.Vector2(0.15f, 0.15f);
            Add(tbox);
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            if (isFirstDraw)
            {
                isFirstDraw = false;
            }
            else if (!isFirstDraw)
            {
                // disable text drawing after first draw.
                tbox.Active = false; 
            }
        }
    }
}

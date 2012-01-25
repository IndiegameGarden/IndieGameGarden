using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using IndiegameGarden.Menus;

namespace IndiegameGarden.Base
{
    public class LoadingScreen: Screenlet 
    {
        GameTextBox tbox;
        bool isFirstDraw = true;

        public LoadingScreen(int x, int y): base(x,y)
        {            
            tbox = new GameTextBox("Loading...");
            tbox.Motion.Position = new Microsoft.Xna.Framework.Vector2(0.15f, 0.15f);
            Add(tbox);
        }

        public void SetGame(IndieGame g)
        {
            tbox.Text = "Loading " + g.Name + " ...";
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
                //tbox.Active = false; 
                GardenGame.Instance.SuppressDraw();
            }
        }



    }
}

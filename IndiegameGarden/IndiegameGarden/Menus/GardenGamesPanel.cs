// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using TTengine.Core;
using TTengine.Modifiers;

using IndiegameGarden.Store;
using IndiegameGarden.Util;

namespace IndiegameGarden.Menus
{
    public class GardenGamesPanel: GamesPanel
    {
        const float LAYER_BACK  = 1.0f;
        const float LAYER_FRONT = 0.0f;
        const float LAYER_ZOOMING_ITEM = 0.1f;
        const float LAYER_DODGING_ITEM = 0.3f;
        const float LAYER_GRID_ITEMS = 0.9f;

        int sizeX, sizeY, centerRow, centerCol;
        int indexGameSelected = 0;
        Dictionary<string, GameThumbnail> thumbnailsCache = new Dictionary<string, GameThumbnail>();
        // cursor is the graphics plus (x,y) coordinate selection 
        GameThumbnailCursor cursor;
        bool isQuitting = false;
        float timeSinceUserInput = 0f;

        public GardenGamesPanel()
        {
            SizeX = 5;
            SizeY = 4;
            cursor = new GameThumbnailCursor();
            Add(cursor);
            //cursor.Visible = false;
        }

        /// <summary>
        /// number of games placed horizontally
        /// </summary>
        public int SizeX {
            get { return sizeX;}
            set 
            { 
                sizeX = value;
                centerCol = (sizeX-1) / 2;
            }
        }

        /// <summary>
        /// number of games placed vertically
        /// </summary>
        public int SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                centerRow = (sizeY-1) / 2;
            }
        }

        public override void UpdateList(List<IndieGame> gl)
        {
            this.gl = gl;
            if (gl.Count > 0)
            {
                if (SelectedGame == null)
                {
                    SelectedGame = gl[0];
                    cursor.X = 0;
                    cursor.Y = 0;
            }
                else
                {
                    if (!gl.Contains(SelectedGame))
                    {
                        SelectedGame = gl[0];
                        cursor.X = 0;
                        cursor.Y = 0;
                    }else{
                        // gl contains the previously selected game. Relocate it in new list.
                        SetCursorToGame(SelectedGame);
                    }
                }
            }
        }

        protected void SetCursorToGame(IndieGame g)
        {
            int idx = gl.IndexOf(g);
            if (idx < 0)
                return; // not found
            // calculate x,y
            cursor.X = (idx % SizeY);
            cursor.Y = idx / SizeX;
        }

        protected void SelectGameBelowCursor()
        {
            indexGameSelected = cursor.X + SizeX * cursor.Y;
            SelectedGame = gl[indexGameSelected];
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            timeSinceUserInput += p.dt;

            if (gl == null)
                return;

            // loop over games
            int Ng = SizeX * SizeY;

            IndieGame g;
            GameThumbnail th;
            const float XSPACING = 0.16f;
            const float YSPACING = 0.16f;

            for (int i = 0; i < gl.Count; i++)
            {
                // fetch that game from list
                g = gl[i];

                // check if selected game
                if (g == SelectedGame)
                    indexGameSelected = i;

                // if GameThumbnail for current game does not exist yet, create it                
                if (!thumbnailsCache.ContainsKey(g.GameID))
                {
                    // create now
                    th = new GameThumbnail(g.GameID);
                    Add(th);
                    thumbnailsCache.Add(g.GameID, th);
                    //th.Position = new Vector2(RandomMath.RandomBetween(-0.4f,2.0f), RandomMath.RandomBetween(-0.4f,1.4f) );
                    //th.Scale = RandomMath.RandomBetween(0.01f, 0.09f); 
                    th.Position = new Vector2(0.5f, 0.5f);
                    th.Scale = 0.01f;

                    th.LayerDepth = LAYER_GRID_ITEMS;
                    th.Visible = false;
                    th.Intensity = 0.3f;
                    //th.Alpha = 0f;
                }else{
                    // retrieve GameThumbnail from cache
                    th = thumbnailsCache[g.GameID];
                }
                
                // check if thnail visible
                if (!th.Visible && cursor.GameletInRange(th))
                {
                    th.Visible = true;
                    th.Add(new MyFuncyModifier(delegate(float v) { th.Alpha = v / 5.0f; th.Intensity = v / 5.0f;  }, 0f, 5f));
                }


                // grid position - calculate target x/y pos for game
                int x = i % SizeX ;
                int y = (i / SizeX) - (indexGameSelected / SizeX) + centerRow + 1;

                // coordinate position
                Vector2 targetPos = new Vector2(x * XSPACING , y * YSPACING);
                MoveToTarget(th, targetPos, 4f);

                if (!isQuitting)
                {
                    // if selected - size adapt
                    if (i == indexGameSelected)
                    {
                        ResizeToTarget(th, 0.45f, 0.01f, 0.002f);
                        th.LayerDepth = LAYER_FRONT;

                        // move the cursor to this selection
                        MoveCursorTo(th.Position, 4f);
                        float zmTarget = 1.0f;
                        const float TIME_START_ZOOM = 2.0f;
                        if (timeSinceUserInput > TIME_START_ZOOM)
                        {
                            zmTarget += (timeSinceUserInput - TIME_START_ZOOM) * 0.03f;
                        }
                        ZoomToTarget(zmTarget, th.Position, 0.002f);
                    }
                    else
                    {
                        ResizeToTarget(th, 0.35f, 0.02f, 0.002f);
                    }
                }
                else
                {
                    // isQuitting
                    ResizeToTarget(th, 0.001f, 0.02f, 0.005f);
                }

            }
        }

        private void MoveCursorTo(Vector2 pos, float spd)
        {
            cursor.Velocity = spd * (pos - cursor.Position);
        }

        // TODO move to class
        private void MoveToTarget(GameThumbnail t, Vector2 targetPos, float spd)
        {
            t.Velocity = spd * ( targetPos - t.Position );
        }

        // TODO move to class
        private void ResizeToTarget(GameThumbnail t, float targetScale, float spd, float spdMin)
        {
            if (t.Scale < targetScale)
            {                
                t.Scale += spdMin + spd * (targetScale - t.Scale); //*= 1.01f;
                if (t.Scale > targetScale)
                {
                    t.Scale = targetScale;
                }
            }
            else if (t.Scale > targetScale)
            {
                t.Scale += -spdMin + spd * (targetScale - t.Scale); //*= 1.01f;
                if (t.Scale < targetScale)
                {             
                    t.Scale = targetScale;
                }
            }
            t.LayerDepth = 0.8f - t.Scale / 1000.0f;
        }

        /// <summary>
        /// zoom entire panel towards the zoom target, around position 'center'
        /// </summary>
        /// <param name="targetZoom"></param>
        /// <param name="targetCenter"></param>
        /// <param name="spd"></param>
        public void ZoomToTarget(float targetZoom, Vector2 targetCenter, float spd)
        {
            float sc = Zoom;
            if (sc < targetZoom)
            {
                sc += spd;
                if (sc > targetZoom)
                    sc = targetZoom;
            }
            else if (sc > targetZoom)
            {
                sc -= spd;
                if (sc < targetZoom)
                    sc = targetZoom;
            }
            Vector2 v = (targetCenter - ZoomCenter);
            if (v.Length() < spd)
                ZoomCenter = targetCenter;
            else
            {
                float spd2 = 125 * v.Length() * spd;
                if (spd2 < spd)
                    spd2 = spd;
                v.Normalize();
                ZoomCenter += spd2 * v;
            }
            Zoom = sc;
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            if (SelectedGame != null)
                Screen.DebugText(0f, 0f, "Selected: " + gl.IndexOf(SelectedGame) + " " + SelectedGame.GameID );
            Screen.DebugText(0f, 0.1f, "Zoom: " + Zoom);
        }

        public override void ChangedSelectedGameEvent(IndieGame newSel, IndieGame oldSel)
        {
            try
            {
                if (oldSel != null)
                {
                    GameThumbnail thOld = thumbnailsCache[oldSel.GameID];
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        public override void SendUserInput(GamesPanel.UserInput inp)
        {
            timeSinceUserInput = 0f;
            switch (inp)
            {
                case UserInput.DOWN:
                    if (cursor.Y < SizeY -1 )
                    {
                        cursor.Y++;
                        SelectGameBelowCursor();
                    }
                    break;
                case UserInput.UP:
                    if (cursor.Y > 0)
                    {
                        cursor.Y--;
                        SelectGameBelowCursor();
                    }
                    break;
                case UserInput.LEFT:
                    if (cursor.X > 0)
                    {
                        cursor.X--;
                        SelectGameBelowCursor();
                    }
                    break;
                case UserInput.RIGHT:
                    if (cursor.X < SizeX-1)
                    {
                        cursor.X++;
                        SelectGameBelowCursor();
                    }   
                    break;
                case UserInput.QUITTING:
                    isQuitting = true;
                    break;
                case UserInput.ABORT_QUITTING:
                    isQuitting = false;
                    break;
            }
        }
    }
}

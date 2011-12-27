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
        const float SCALE_REGULAR = 1f; //0.16f;
        const float SCALE_GRID_X = 0.16f;
        const float SCALE_GRID_Y = 0.16f;
        const float CURSOR_SCALE_REGULAR = 0.95f; //5.9375f;
        const float THUMBNAIL_SCALE_SELECTED = 0.35f; //2f;
        const float THUMBNAIL_SCALE_UNSELECTED = 0.28f; //1.5625f;

        // maximum sizes of grid
        public double SizeX=32, SizeY=32;
        Dictionary<string, GameThumbnail> thumbnailsCache = new Dictionary<string, GameThumbnail>();
        // cursor is the graphics thingy plus (x,y) coordinate selection 
        GameThumbnailCursor cursor;
        // UI related vars - related to whether user indicates to quit program or user cancelled this
        bool isQuitting = false;
        bool abortIsQuitting = false;
        float timeSinceUserInput = 0f;
        Vector2 PanelShiftPos = Vector2.Zero;

        public GardenGamesPanel()
        {
            cursor = new GameThumbnailCursor();
            Add(cursor);
            cursor.Scale = CURSOR_SCALE_REGULAR;
            Zoom = SCALE_REGULAR;
            //cursor.Visible = false;
        }

        public override void UpdateList(GameCollection gl)
        {
            // first process old list - start fading away of items
            for (int i = 0; i < gl.Count; i++)
            {
                IndieGame g = gl[i];
                if (thumbnailsCache.ContainsKey(g.GameID))
                {
                    GameThumbnail th = thumbnailsCache[g.GameID];
                    th.FadeToTarget(0f,4f);
                }
            }
            this.gl = gl;

            // update selection
            if (gl.Count > 0)
            {
                if (SelectedGame == null)
                {
                    SelectedGame = gl[0];
                    cursor.SetToGame(SelectedGame);
                }
                else
                {
                    if (!gl.Contains(SelectedGame))
                    {
                        SelectedGame = gl[0];
                        cursor.SetToGame(SelectedGame);
                    }
                    else
                    {
                        // gl contains the previously selected game. Relocate it in new list.
                        cursor.SetToGame(SelectedGame);
                    }
                }
            }
        }

        protected void SelectGameBelowCursor()
        {
            IndieGame g = gl.FindGameAt(cursor.GridPosition);
            if (g != null)
            {
                SelectedGame = g;                
            }
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            timeSinceUserInput += p.dt;

            if (gl == null)
                return;

            IndieGame g;
            GameThumbnail th;

            // loop all games
            for (int i = 0; i < gl.Count; i++)
            {
                // fetch that game from list
                g = gl[i];

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
                    th.Intensity = 0.0f;
                    th.Alpha = 0f;
                }else{
                    // retrieve GameThumbnail from cache
                    th = thumbnailsCache[g.GameID];
                }
                
                // check if thnail visible and in range. If so, start displaying it (fade in)
                if (!th.Visible && cursor.GameletInRange(th))
                {
                    th.Visible = true;
                    th.Intensity = 0f;
                    th.FadeToTarget(1.0f, 4.3f);
                }

                // coordinate position where to move game thumbnail to 
                // TODO include centerRow effect
                Vector2 targetPos = (g.Position - PanelShiftPos) * new Vector2(SCALE_GRID_X,SCALE_GRID_Y);
                th.MoveToTarget(targetPos, 4f);
                // cursor
                cursor.Target = (cursor.GridPosition - PanelShiftPos) * new Vector2(SCALE_GRID_X,SCALE_GRID_Y);

                // panel shift effect
                Vector2 cp = cursor.Position;
                float dx = 1f * p.dt;
                if (cp.X < 0f)
                {
                    PanelShiftPos.X -= dx;
                }
                else if (cp.X > 1.2f)
                {
                    PanelShiftPos.X += dx;
                }
                if (cp.Y < 0.2f)
                {
                    PanelShiftPos.Y -= dx;
                }
                else if (cp.Y > 0.8f)
                {
                    PanelShiftPos.Y += dx;
                }

                // quitting and selected game behaviour
                if (!isQuitting)
                {
                    // if selected - size adapt
                    if (g == SelectedGame)
                    {
                        th.ScaleToTarget(THUMBNAIL_SCALE_SELECTED, 0.01f, 0.002f);
                        th.LayerDepth = LAYER_FRONT;

                        float zmTarget = 1.0f;
                        const float TIME_START_ZOOM = 1.4f;
                        if (timeSinceUserInput > TIME_START_ZOOM)
                        {
                            zmTarget += Math.Min( (timeSinceUserInput - TIME_START_ZOOM) * 0.08f, 3f);
                        }
                        ZoomToTarget(zmTarget * SCALE_REGULAR, th.PositionAbs, 0.004f);
                    }
                    else if (SelectedGame == null)
                    {
                        //
                    }
                    else
                    {
                        th.ScaleToTarget(THUMBNAIL_SCALE_UNSELECTED, 0.02f, 0.002f);
                    }
                }
                else
                {
                    // isQuitting
                    ZoomToTarget(0.001f, ZoomCenter, 0.0005f * Zoom);
                }

                if (abortIsQuitting)
                {
                    ZoomToTarget(SCALE_REGULAR, ZoomCenter, 0.0005f);
                }

            }
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
                sc *= (1.0f+spd);
                if (sc > targetZoom)
                    sc = targetZoom;
            }
            else if (sc > targetZoom)
            {
                sc /= (1.0f+spd);
                if (sc < targetZoom)
                    sc = targetZoom;
            }
            Vector2 v = (targetCenter - ZoomCenter);
            if (v.Length() < spd)
                ZoomCenter = targetCenter;
            else
            {
                float spd2 = 5 * v.Length() * spd;
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
        }

        public override void SendUserInput(GamesPanel.UserInput inp)
        {
            timeSinceUserInput = 0f;
            switch (inp)
            {
                case UserInput.DOWN:
                    if (cursor.GridPosition.Y < SizeY -1 )
                    {
                        cursor.GridPosition.Y += 1f;
                        SelectGameBelowCursor();
                    }
                    break;
                case UserInput.UP:
                    if (cursor.GridPosition.Y > 0)
                    {
                        cursor.GridPosition.Y -= 1f;
                        SelectGameBelowCursor();
                    }
                    break;
                case UserInput.LEFT:
                    if (cursor.GridPosition.X > 0)
                    {
                        cursor.GridPosition.X -= 1f;
                        SelectGameBelowCursor();
                    }
                    break;
                case UserInput.RIGHT:
                    if (cursor.GridPosition.X < SizeX - 1)
                    {
                        cursor.GridPosition.X += 1f;
                        SelectGameBelowCursor();
                    }   
                    break;
                case UserInput.QUITTING:
                    isQuitting = true;
                    abortIsQuitting = false;
                    break;
                case UserInput.ABORT_QUITTING:
                    isQuitting = false;
                    abortIsQuitting = true;
                    break;
            }
        }
    }
}

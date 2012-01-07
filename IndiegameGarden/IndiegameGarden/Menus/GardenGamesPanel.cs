// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using TTengine.Core;
using TTengine.Modifiers;

using IndiegameGarden.Base;
using IndiegameGarden.Util;

namespace IndiegameGarden.Menus
{
    /**
     * a GamesPanel that arranges games in a large rectangular matrix, the "garden". User can travel
     * the garden with a cursor. Only a part of the garden is shown at a time.
     */
    public class GardenGamesPanel: GamesPanel
    {
        // below: UI constants
        public const float LAYER_BACK = 1.0f;
        public const float LAYER_FRONT = 0.0f;
        public const float LAYER_ZOOMING_ITEM = 0.1f;
        public const float LAYER_DODGING_ITEM = 0.3f;
        public const float LAYER_GRID_ITEMS = 0.9f;

        public const float PANEL_SCALE_REGULAR = 1f; //0.16f;
        public const float PANEL_SCALE_GRID_X = 0.16f;
        public const float PANEL_SCALE_GRID_Y = 0.16f;
        public const float PANEL_SPEED_SHIFT = 2.1f;
        public const float PANEL_SIZE_X = 1.333f;
        public const float PANEL_SIZE_Y = 1.0f;
        public const float CURSOR_SCALE_REGULAR = 0.95f; //5.9375f;
        public const float THUMBNAIL_SCALE_UNSELECTED = 0.28f; //1.5625f;
        public const float THUMBNAIL_SCALE_SELECTED = 0.35f; //2f;
        public const float THUMBNAIL_SCALE_SELECTED1 = 2.857f;
        public const float THUMBNAIL_SCALE_SELECTED2 = 3.5f;

        // maximum sizes of grid
        public double GridMaxX=32, GridMaxY=32;

        // zoom, scale etc. related vars for panel
        public float ZoomTarget = 1.0f;
        public float ZoomSpeed = 0f;

        Dictionary<string, GameThumbnail> thumbnailsCache = new Dictionary<string, GameThumbnail>();
        // cursor is the graphics selection thingy 
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
            Zoom = PANEL_SCALE_REGULAR;
            //cursor.Visible = false;
        }

        public override void OnUpdateList(GameCollection gl)
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

        // shorthand method to select the game currently indicated by cursor
        protected void SelectGameBelowCursor()
        {
            IndieGame g = gl.FindGameAt(cursor.GridPosition);
            SelectedGame = g;                
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            timeSinceUserInput += p.dt;

            // handle dynamic zooming
            if (Zoom < ZoomTarget && ZoomSpeed > 0f)
            {
                Zoom *= (1.0f + ZoomSpeed);
                if (Zoom > ZoomTarget)
                    Zoom = ZoomTarget;
            }
            else if (Zoom > ZoomTarget && ZoomSpeed > 0f)
            {
                Zoom /= (1.0f + ZoomSpeed);
                if (Zoom < ZoomTarget)
                    Zoom = ZoomTarget;
            }

            //-- loop all games adapt their display properties where needed
            if (gl == null)
                return;
            IndieGame g;
            GameThumbnail th;
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

                // coordinate position where to move a game thumbnail to 
                Vector2 targetPos = (g.Position - PanelShiftPos) * new Vector2(PANEL_SCALE_GRID_X,PANEL_SCALE_GRID_Y);
                th.Target = targetPos;
                th.TargetSpeed = 4f;

                // cursor where to move to
                cursor.Target = (cursor.GridPosition - PanelShiftPos) * new Vector2(PANEL_SCALE_GRID_X,PANEL_SCALE_GRID_Y);

                // panel shift effect when cursor hits edges of panel
                Vector2 cp = cursor.Position;
                float chw = cursor.WidthAbs / 2.0f; // cursor-half-width
                float chh = cursor.HeightAbs / 2.0f; // cursor-half-height
                float dx = PANEL_SPEED_SHIFT * p.dt;
                if (cp.X <= chw)
                {
                    PanelShiftPos.X -= dx;
                }
                else if (cp.X >= PANEL_SIZE_X - chw)
                {
                    PanelShiftPos.X += dx;
                }
                if (cp.Y <= chh)
                {
                    PanelShiftPos.Y -= dx;
                }
                else if (cp.Y >= PANEL_SIZE_Y - chh)
                {
                    PanelShiftPos.Y += dx;
                }

                // quitting and selected game behaviour
                if (!isQuitting)
                {
                    // if selected - size adapt
                    if (g == SelectedGame)
                    {
                        th.ScaleTarget = THUMBNAIL_SCALE_SELECTED;
                        th.ScaleSpeed = 0.01f;
                        //th.LayerDepth = LAYER_FRONT;
                    }
                    else if (SelectedGame == null)
                    {
                        //
                    }
                    else
                    {
                        th.ScaleTarget = THUMBNAIL_SCALE_UNSELECTED;
                        th.ScaleSpeed = 0.02f;
                    }
                }
                else
                {
                    // isQuitting
                    // FIXME out of game loop!
                    ZoomTarget = 0.001f;
                    ZoomSpeed = 0.005f;
                }

                if (abortIsQuitting)
                {
                    ZoomToNormal();
                }

            }
        }

        // shorthand method to restore zoom of panel back to normal
        public void ZoomToNormal()
        {
            ZoomTarget = PANEL_SCALE_REGULAR;
            //ZoomCenter = Screen.Center; // don't specify - use previous zoomcenter
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            // DEBUG
            if (SelectedGame != null)
                Screen.DebugText(0f, 0f, "Selected: " + gl.IndexOf(SelectedGame) + " " + SelectedGame.GameID );
            Screen.DebugText(0f, 0.1f, "Zoom: " + Zoom);
        }

        public override void OnChangedSelectedGame(IndieGame newSel, IndieGame oldSel)
        {
            // unselect the previous game
            if (oldSel != null)
            {
                GameThumbnail th = thumbnailsCache[oldSel.GameID];
                if (th != null)
                {
                    th.ScaleTarget = THUMBNAIL_SCALE_UNSELECTED;
                    th.ScaleSpeed = 0.01f;
                }
            }
        }

        public override void OnUserInput(GamesPanel.UserInput inp)
        {
            timeSinceUserInput = 0f;
            
            switch (inp)
            {
                case UserInput.DOWN:
                    if (cursor.GridPosition.Y < GridMaxY -1 )
                    {
                        cursor.GridPosition.Y += 1f;
                        SelectGameBelowCursor();
                    }
                    ZoomToNormal();
                    break;
               
                case UserInput.UP:
                    if (cursor.GridPosition.Y > 0)
                    {
                        cursor.GridPosition.Y -= 1f;
                        SelectGameBelowCursor();
                    }
                    ZoomToNormal();
                    break;
                
                case UserInput.LEFT:
                    if (cursor.GridPosition.X > 0)
                    {
                        cursor.GridPosition.X -= 1f;
                        SelectGameBelowCursor();
                    }
                    ZoomToNormal();
                    break;
                
                case UserInput.RIGHT:
                    if (cursor.GridPosition.X < GridMaxX - 1)
                    {
                        cursor.GridPosition.X += 1f;
                        SelectGameBelowCursor();
                    }
                    ZoomToNormal();
                    break;
                
                case UserInput.QUITTING:
                    isQuitting = true;
                    abortIsQuitting = false;
                    break;
                
                case UserInput.ABORT_QUITTING:
                    isQuitting = false;
                    abortIsQuitting = true;
                    break;

                case UserInput.SELECT1:
                    if (SelectedGame != null)
                    {
                        // zoom in on selected game
                        GameThumbnail th = thumbnailsCache[SelectedGame.GameID];
                        if (th != null)
                        {
                            ZoomTarget = THUMBNAIL_SCALE_SELECTED1;
                            ZoomCenter = th.PositionAbs;
                            ZoomSpeed = 0.05f;
                        }
                    }
                    break;

                case UserInput.SELECT2:
                    if (SelectedGame != null)
                    {
                        // zoom in on selected game
                        GameThumbnail th = thumbnailsCache[SelectedGame.GameID];
                        if (th != null)
                        {
                            ZoomTarget = THUMBNAIL_SCALE_SELECTED2;
                            ZoomCenter = th.Position;
                            ZoomSpeed = 0.05f;
                        }
                    }
                    break;
            }
        }
    }
}

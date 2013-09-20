// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using TTengine.Core;
using TTengine.Modifiers;
using TTengine.Util;
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
        // below: UI constants; TODO find all layer refs and init globally
        const float LAYER_BACK = 1.0f;
        const float LAYER_FRONT = 0.0f;
        const float LAYER_ZOOMING_ITEM = 0.1f;
        const float LAYER_DODGING_ITEM = 0.3f;
        const float LAYER_GRID_ITEMS = 0.9f;

        const float PANEL_ZOOM_REGULAR = 1.2f;
        const float PANEL_DELTA_GRID_X = 0.16f;
        const float PANEL_DELTA_GRID_Y = 0.12f;
        const float PANEL_SPEED_SHIFT = 3.2f;
        const float PANEL_SIZE_X = 1.333f;
        const float PANEL_SIZE_Y = 1.0f;
        const float PANEL_ZOOM_TARGET_QUITTING = 0.01f;
        const float PANEL_ZOOM_SPEED_QUITTING = 0.008f;
        const float PANEL_ZOOM_SPEED_REGULAR = 0.03f;
        const float PANEL_ZOOM_SPEED_ABORTQUITTING = PANEL_ZOOM_SPEED_REGULAR;
        static Vector2 PANEL_INITIAL_SHIFT_POS = new Vector2(-1.5f,-3f);

        const float        CURSOR_SCALE_REGULAR = 0.28f; 
        float               CURSOR_DISCOVERY_RANGE = 9999f;
        const float         CURSOR_DISCOVERY_RANGE_MIN = 9999f;
        const float         CURSOR_DISCOVERY_RANGE_MAX = 9999f; 
        const float         CURSOR_FADEOUT_RANGE = 9999f;
        const float        CURSOR_DESTRUCTION_RANGE = 9999f;
        const float        CURSOR_MARGIN_X = 0.15f;
        const float        CURSOR_MARGIN_Y = 0.15f;
        static Vector2     CURSOR_INITIAL_POSITION = new Vector2(0f, 0f);

        public const float THUMBNAIL_SCALE_UNSELECTED = 0.44f; 
        const float        THUMBNAIL_SCALE_SELECTED = 0.51f; 
        public const float THUMBNAIL_MAX_WIDTH_PIXELS = 320f;
        public const float THUMBNAIL_MAX_HEIGHT_PIXELS = 240f;
        const float        THUMBNAIL_FADE_SPEED = 0.3f;

        static Vector2 INFOBOX_SHOWN_POSITION = new Vector2(0.05f, 0.895f);
        static Vector2 INFOBOX_DESCRIPTION_HIDDEN_POSITION = new Vector2(0.05f, 0.97f);
        static Vector2 INFOBOX_ALL_HIDDEN_POSITION = new Vector2(0.05f, 1.15f);
        const float    INFOBOX_SPEED_MOVE = 3.8f;
        static Vector2 HELPTEXT_SHOWN_POSITION = new Vector2(0.18f, 0.17f);
        static Vector2 HELPTEXT_HIDDEN_POSITION = new Vector2(0.18f, -0.2f);
        const float    HELPTEXT_SCALE_DEFAULT = 1.0f;
        const float    HELPTEXT_SPEED_MOVE = 3.8f;
        static Vector2 CREDITS_SHOWN_POSITION = new Vector2(0.4f, 0.165f);
        static Vector2 CREDITS_HIDDEN_POSITION = new Vector2(0.4f, -0.22f);
        const float    CREDITS_SCALE_DEFAULT = 1f;
        const float    CREDITS_SPEED_MOVE = 3.8f;
        
        const float TIME_BEFORE_GAME_LAUNCH = 0.2f;
        const float TIME_AFTER_GAME_LAUNCH_CANCELS = 4f;
        const float TIME_BEFORE_EXIT = 0.05f;
        const float TIME_BEFORE_EXIT_CONTINUES = 0.03f;

        // maximum sizes of grid
        public double GridMaxX=99, GridMaxY=99; // TODO link to GameLib size (100)

        Dictionary<string, GameThumbnail> thumbnailsCache = new Dictionary<string, GameThumbnail>();

        // cursor is the graphics selection thingy         
        GameThumbnailCursor cursor;

        // box showing info of a game such as title and download progressContributionSingleFile
        GameInfoBox infoBox;

        // showing controls help message
        Spritelet helpTextBitmap;

        // showing credits
        Spritelet creditsBitmap;

        // UI related vars - related to whether user indicates to quit program or user cancelled this
        bool isExiting = false;
        bool isExitingUnstoppable = false;
        bool isGameLaunchOngoing = false;
        bool isGameLaunchConfirmed = false;
        bool isLaunchWebsite = false;
        float timeExiting = 0f;
        float timeLaunching = 0f;
        Vector2 PanelShiftPos = PANEL_INITIAL_SHIFT_POS;
        int selectionLevel = 1;
        GameChooserMenu parentMenu;

        public GardenGamesPanel(GameChooserMenu parent)
        {
            parentMenu = parent;

            // cursor
            cursor = new GameThumbnailCursor();
            Add(cursor);
            cursor.Motion.Scale = CURSOR_SCALE_REGULAR;
            cursor.Motion.Position = CURSOR_INITIAL_POSITION * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);
            cursor.Motion.TargetPos = CURSOR_INITIAL_POSITION * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);
            cursor.GridPosition = CURSOR_INITIAL_POSITION;
            cursor.Motion.Add(new MyFuncyModifier(delegate(float v) { return v / 10.3f; }, "Rotate"));

            // info box - will be added to parent upon OnNewParent() event
            infoBox = new GameInfoBox();
            infoBox.Motion.Position = INFOBOX_ALL_HIDDEN_POSITION;
            infoBox.Motion.TargetPos = INFOBOX_SHOWN_POSITION;
            infoBox.Motion.TargetPosSpeed = INFOBOX_SPEED_MOVE;

            // controls help 
            helpTextBitmap = new Spritelet("keymap");
            helpTextBitmap.Motion.Scale = HELPTEXT_SCALE_DEFAULT;
            helpTextBitmap.Motion.Position = HELPTEXT_HIDDEN_POSITION;
            helpTextBitmap.Motion.TargetPos = HELPTEXT_SHOWN_POSITION;
            helpTextBitmap.Motion.TargetPosSpeed = HELPTEXT_SPEED_MOVE;

            // credits
            creditsBitmap = new Spritelet("credits.png");
            creditsBitmap.Motion.Scale = CREDITS_SCALE_DEFAULT;
            creditsBitmap.Motion.Position = CREDITS_HIDDEN_POSITION;
            creditsBitmap.Motion.TargetPos = CREDITS_HIDDEN_POSITION;
            creditsBitmap.Motion.TargetPosSpeed = CREDITS_SPEED_MOVE;

            // default zoom
            Motion.Zoom = PANEL_ZOOM_REGULAR;
            Motion.ZoomTarget = PANEL_ZOOM_REGULAR;
            Motion.ZoomSpeed = PANEL_ZOOM_SPEED_REGULAR;
            Motion.ZoomCenterTarget = cursor.Motion;
        }

        public override void OnUpdateList(GameCollection gl)
        {
            this.gl = gl;
            SelectGameBelowCursor();
        }

        // shorthand method to select the game currently indicated by cursor
        protected void SelectGameBelowCursor()
        {
            if (gl != null)
            {
                GardenItem g = gl.FindGameAt(cursor.GridPosition);
                SelectedGame = g;
                infoBox.ClearProgressBar();
                if (g != null && !g.IsVisible)  // reset back to null for invisible items. Not selectable.
                    SelectedGame = null;
                if (g != null)
                    g.Refresh();
            }
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();

            // some items are part of the parent, to avoid scaling issues in GardenGamesPanel
            // (which get rescaled/zoomed based on user input).
            Parent.Add(infoBox);
            Parent.Add(helpTextBitmap);
            Parent.Add(creditsBitmap);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            GardenItem selGame = SelectedGame;

            // update info box
            if (selGame == null && infoBox != null)
            {
                infoBox.SetGameInfo(selGame);
            }

            // handle download/install/launching of a game
            if (isGameLaunchOngoing)
                timeLaunching += p.Dt;
            if (selGame != null && isGameLaunchOngoing && timeLaunching < TIME_BEFORE_GAME_LAUNCH)
            {
                GameThumbnail th = thumbnailsCache[selGame.GameID];
                float sc = (1f + timeLaunching/3f);
                th.Motion.ScaleTarget = THUMBNAIL_SCALE_SELECTED * sc; // blow up size of thumbnail while user requests launch
                th.Motion.ScaleSpeed = 0.02f;
                cursor.Motion.ScaleTarget = CURSOR_SCALE_REGULAR * sc;
                cursor.Motion.ScaleSpeed = th.Motion.ScaleSpeed / selGame.ScaleIcon; // TODO correct ScaleIcon?
            }

            if (!isGameLaunchOngoing)
            {
                cursor.Motion.ScaleTarget = CURSOR_SCALE_REGULAR;                
            }

            // launch of a game
            if (isGameLaunchConfirmed && selGame != null)
            {
                cursor.Motion.ScaleTarget = CURSOR_SCALE_REGULAR;

                GameThumbnail thumb = thumbnailsCache[selGame.GameID];
                if (selGame.IsIggClient)
                {
                    if (selGame.IsInstalled)
                    {
                        BentoGame.Instance.music.FadeOut();
                        BentoGame.Instance.ActionLaunchGame(selGame, thumb);
                        isExiting = true;
                        timeExiting = TIME_BEFORE_EXIT;
                    }
                    else
                    {
                        BentoGame.Instance.ActionDownloadAndInstallGame(selGame);
                    }
                }
                else
                {
                    if (selGame.IsWebGame)
                    {
                        BentoGame.Instance.ActionLaunchWebsitePlayGame(selGame, thumb);
                    }
                    else if (!selGame.IsGrowable)
                    {
                        thumb.Motion.Add(new TemporaryScaleBlowup());
                    }
                    else if (selGame.IsInstalled)
                    {
                        BentoGame.Instance.music.FadeOut();
                        BentoGame.Instance.ActionLaunchGame(selGame, thumb);
                    }
                    else // if (not installed)
                    {
                        BentoGame.Instance.ActionDownloadAndInstallGame(selGame);
                    }
                }
                isGameLaunchOngoing = false;
                isGameLaunchConfirmed = false;
                timeLaunching = 0f;
            }

            // handle exit key
            if (isExiting)
            {
                BentoGame.Instance.music.FadeOut();
                timeExiting += p.Dt;
                if (timeExiting > TIME_BEFORE_EXIT)
                {
                    parentMenu.background.Motion.ScaleModifier = 1f / (1f + (timeExiting-TIME_BEFORE_EXIT) / 11f);
                    if (!isExitingUnstoppable)
                    {
                        BentoGame.Instance.SignalExitGame();
                        isExitingUnstoppable = true;
                        Motion.ZoomSpeed = PANEL_ZOOM_SPEED_QUITTING;
                    }
                    return;
                }
            }
            else
            {
                if (timeExiting > 0f)
                {
                    if(BentoGame.Instance.music.UserWantsMusic)
                        BentoGame.Instance.music.FadeIn();
                    timeExiting = 0f;
                }
            }

            //-- website launch
            if (isLaunchWebsite)
            {
                if (selGame != null && selGame.DeveloperWebsiteURL.Length > 0 )
                {
                    GameThumbnail thumb = thumbnailsCache[selGame.GameID];
                    BentoGame.Instance.ActionLaunchWebsite(selGame, thumb);
                }
                isLaunchWebsite = false;
            }

            //-- loop all nearby games adapt their display properties where needed
            if (gl == null)
                return;
            GardenItem g;

            // upd cache with possibly new items around cursor
            List<GardenItem> c = gl.GetItemsAround((int)cursor.GridPosition.X, (int)cursor.GridPosition.Y, (int) CURSOR_DISCOVERY_RANGE);
            if (selGame != null)
                c.Add(selGame);
            for (int i = c.Count - 1; i >= 0; i--)
            {
                g = c[i];

                // if GameThumbnail for current game does not exist yet, create it                
                if (!thumbnailsCache.ContainsKey(g.GameID) && g.IsVisible && g.GameID.Length > 0 )
                {
                    // create now
                    GameThumbnail th = new GameThumbnail(g);
                    Add(0, th);
                    thumbnailsCache.Add(g.GameID, th);
                    //th.Position = new Vector2(RandomMath.RandomBetween(-0.4f,2.0f), RandomMath.RandomBetween(-0.4f,1.4f) );
                    //th.Scale = RandomMath.RandomBetween(0.01f, 0.09f); 
                    // create with new position and scale
                    th.Motion.Position = new Vector2(0f,0f);
                    th.Motion.Scale = 0.05f;
                    th.Motion.ScaleTarget = 0.05f;
                    th.Motion.ScaleSpeed = 0.01f; // TODO const

                    th.DrawInfo.LayerDepth = LAYER_GRID_ITEMS + ((float)th.ID) * 0.0000001f;
                    th.Visible = false;
                    th.ColorB.Alpha = 0.0f;

                    // special case thumbnails 
                    if (g.GameID.Equals("igg_controls"))
                        th.Motion.Add(new MyFuncyModifier( delegate(float v) { return v/6.3f; }, "Rotate"));
                }
            }
                
            // visit all cached items and adjust positions, visibility, etc.
            int thumbnailLoadsStarted = 0;
            List<GameThumbnail> toRemoveFromCache = new List<GameThumbnail>();
            foreach(GameThumbnail th in thumbnailsCache.Values)
            {
                g = th.Game;

                // check if out of range. If so, remove from cache later
                if (cursor.DistanceTo(th) > CURSOR_DESTRUCTION_RANGE)
                {
                    toRemoveFromCache.Add(th);
                    th.Delete = true;
                    continue;
                }

                // check if thnail invvisible but in range. If so, start loading it
                if (!th.Visible && cursor.DistanceTo(th) <= CURSOR_DISCOVERY_RANGE && thumbnailLoadsStarted == 0)
                {
                    th.ColorB.Alpha = 0f;
                    if(th.LoadInBackground())
                        thumbnailLoadsStarted++;                                                
                }

                // check if thnail is loaded and still in range. If so, start displaying it (fade in)
                if (th.IsLoaded() && cursor.DistanceTo(th) <= CURSOR_DISCOVERY_RANGE)
                {
                    if (th.Game.IsGrowable)
                    {
                        th.ColorB.AlphaTarget = 1f; // (0.65f + 0.35f * g.InstallProgress);
                        th.ColorB.SaturationTarget = (0.8f + 0.2f * g.InstallProgress);
                    }
                    else
                    {
                        th.ColorB.AlphaTarget = 1f;
                        th.ColorB.SaturationTarget = 1f;
                    }
                }

                // check if thnail is in range to fade out
                if (th.IsLoaded() && cursor.DistanceTo(th) >= CURSOR_FADEOUT_RANGE)
                {
                    th.ColorB.AlphaTarget = 0f;
                    th.ColorB.FadeSpeed = THUMBNAIL_FADE_SPEED*2.5f;
                    th.IsFadingOut = true;
                    th.Motion.ScaleTarget = 0.001f;
                    th.Motion.ScaleSpeed = 0.014f;
                    th.Motion.TargetPosSpeed = PANEL_SPEED_SHIFT * 0.5f;
                    if (th.Motion.PositionAbsZoomed.Y < 0.5f)
                        th.Motion.TargetPos = new Vector2(th.Motion.Position.X,-0.5f);
                    else
                        th.Motion.TargetPos = new Vector2(th.Motion.Position.X, 1.5f);

                }
                else
                {
                    th.IsFadingOut = false;
                }

                // set target position and scale of each active thumbnail
                if (!th.IsFadingOut)
                {
                    th.Motion.ScaleTarget = THUMBNAIL_SCALE_UNSELECTED;
                    th.ColorB.FadeSpeed = THUMBNAIL_FADE_SPEED; // TODO do this only once per th?
                    // coordinate position where to move a game thumbnail to 
                    Vector2 targetPos = (g.Position - PanelShiftPos) * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);
                    th.Motion.TargetPos = targetPos;
                    th.Motion.TargetPosSpeed = PANEL_SPEED_SHIFT;
                }                

            } // end for loop over all games

            foreach (GameThumbnail th in toRemoveFromCache)
            {
                thumbnailsCache.Remove(th.Game.GameID);
            }

            // --- for selected game only
            if (selGame != null)
            {
                g = selGame;
                // update text box with currently selected game info
                if( g != infoBox.game)
                    infoBox.SetGameInfo(g);

                //-- credits text
                if (g.GameID.Equals("igg_credits") && !isExiting)
                {
                    creditsBitmap.Motion.TargetPos = CREDITS_SHOWN_POSITION;
                    Vector2 cpd = cursor.Motion.PositionAbsZoomed;
                    if (cpd.Y <= 0.35f) // TODO const
                    {
                        float dxp = PANEL_SPEED_SHIFT * p.Dt;
                        PanelShiftPos.Y -= dxp;
                    }
                }
                else
                {
                    creditsBitmap.Motion.TargetPos = CREDITS_HIDDEN_POSITION;
                }

                if (!isGameLaunchOngoing)
                {
                    if (thumbnailsCache.ContainsKey(g.GameID))
                    {
                        GameThumbnail th = thumbnailsCache[g.GameID];

                        if (g.IsInstalling)
                        {
                            // wobble the size of icon when installing.
                            th.Motion.ScaleTarget = 1.0f + 0.1f * (float)Math.Sin(MathHelper.TwoPi * 0.16f * SimTime);
                        }
                        else
                        {
                            // displaying selected thumbnails larger
                            if (g.IsGrowable)
                            {
                                th.Motion.ScaleTarget = THUMBNAIL_SCALE_SELECTED;
                            }
                            else
                            {
                                th.Motion.ScaleTarget = THUMBNAIL_SCALE_UNSELECTED;
                            }
                        }
                    }
                }
            }

            // cursor where to move to
            cursor.Motion.TargetPos = (cursor.GridPosition - PanelShiftPos) * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);
            cursor.Motion.TargetPosSpeed = PANEL_SPEED_SHIFT;

            // panel shift effect when cursor hits edges of panel
            /*
            Vector2 cp = cursor.Motion.PositionAbsZoomed;
            float chw = cursor.DrawInfo.WidthAbs / 2.0f; // cursor-half-width
            float chh = cursor.DrawInfo.HeightAbs / 2.0f; // cursor-half-height
            float dx = PANEL_SPEED_SHIFT * p.Dt;
            const float xMargin = CURSOR_MARGIN_X; // TODO into gui props
            const float yMargin = CURSOR_MARGIN_Y;
            if (cp.X <= chw + xMargin)
            {
                PanelShiftPos.X -= dx;
            }
            else if (cp.X >= PANEL_SIZE_X - chw - xMargin)
            {
                PanelShiftPos.X += dx;
            }
            if (cp.Y <= chh + yMargin)
            {
                PanelShiftPos.Y -= dx;
            }
            else if (cp.Y >= PANEL_SIZE_Y - chh - yMargin)
            {
                PanelShiftPos.Y += dx;
            }
            */
        }

        public override void OnChangedSelectedGame(GardenItem newSel, GardenItem oldSel)
        {
            //
        }

        public override void OnUserInput(GamesPanel.UserInput inp, Vector2 pointerPos)
        {
            //((mousepos / screenheight - screen.center) / motionparent.zoom ) + MotionParent.zoomcenter
            Vector2 panelPos = (((pointerPos / Screen.HeightPixels) - Screen.Center) / Motion.Zoom) + Motion.ZoomCenter;
            
            Vector2 gridPos = new Vector2( panelPos.X / PANEL_DELTA_GRID_X, panelPos.Y / PANEL_DELTA_GRID_Y) + PanelShiftPos;
            TTutil.Round(ref gridPos);
            //cursor.Motion.TargetPos = (cursor.GridPosition - PanelShiftPos) * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);
            //cursor.Motion.TargetPos = panelPos;

            if (gridPos.Equals(cursor.GridPosition))
            {
                OnUserInput(GamesPanel.UserInput.START_SELECT);
            }

            cursor.GridPosition = gridPos;

            SelectGameBelowCursor();
        }

        public override void OnUserInput(GamesPanel.UserInput inp)
        {
            // HACK: for web games launch, make sure that music is turned back on upon next user input after play
            if (!isGameLaunchOngoing && 
                !BentoGame.Instance.music.IsFadedIn && 
                BentoGame.Instance.music.UserWantsMusic)
            {
                BentoGame.Instance.music.FadeIn();
            }

            switch (inp)
            {
                case UserInput.DOWN:
                    if (cursor.GridPosition.Y < GridMaxY -1 )
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
                    if (cursor.GridPosition.X < GridMaxX - 1)
                    {
                        cursor.GridPosition.X += 1f;
                        SelectGameBelowCursor();
                    }
                    break;
                
                case UserInput.START_EXIT:
                    isExiting = true;
                    //selectionLevel = 0;
                    Motion.ZoomTarget = PANEL_ZOOM_TARGET_QUITTING ;
                    Motion.ZoomSpeed = PANEL_ZOOM_SPEED_REGULAR ;
                    infoBox.Motion.TargetPos = INFOBOX_ALL_HIDDEN_POSITION;
                    creditsBitmap.Motion.TargetPos = CREDITS_HIDDEN_POSITION;
                    helpTextBitmap.Motion.TargetPos = HELPTEXT_HIDDEN_POSITION;
                    //Motion.ZoomCenter = cursor.Motion.PositionAbs;
                    //Motion.ZoomCenterTarget = cursor.Motion;
                    break;
                
                case UserInput.STOP_EXIT:
                    if (timeExiting < TIME_BEFORE_EXIT_CONTINUES)
                    {
                        isExiting = false;
                        selectionLevel = 0;
                        Motion.ZoomTarget = PANEL_ZOOM_REGULAR;
                        Motion.ZoomSpeed = PANEL_ZOOM_SPEED_ABORTQUITTING;
                        //Motion.ZoomCenter = cursor.Motion.PositionAbs;
                        //Motion.ZoomCenterTarget = cursor.Motion;
                    }
                    break;

                case UserInput.START_SELECT:
                    if (SelectedGame != null)
                    {
                        GameThumbnail th = null;
                        try
                        {
                            th = thumbnailsCache[SelectedGame.GameID];
                        }
                        catch (Exception) { ; }
                        if (th != null)
                        {
                            // select again - install or launch game if selection key pressed long enough.
                            isGameLaunchOngoing = true;
                        }
                    }
                    break;

                case UserInput.STOP_SELECT:
                    // if not launched long enough, reset - no action
                    if (timeLaunching < TIME_BEFORE_GAME_LAUNCH | timeLaunching > TIME_AFTER_GAME_LAUNCH_CANCELS)
                    {                        
                        isGameLaunchConfirmed = false;
                    }
                    else
                        isGameLaunchConfirmed = true;
                    isGameLaunchOngoing = false;
                    timeLaunching = 0f;                    
                    break;

                case UserInput.LAUNCH_WEBSITE:
                    isLaunchWebsite = true;
                    break;

                case UserInput.TOGGLE_MUSIC:
                    BentoGame.Instance.music.ToggleMusic();
                    break;

            } // switch(inp)

            if (!isExiting)
            {
                if (selectionLevel == 0)
                {
                    infoBox.Motion.TargetPos = INFOBOX_DESCRIPTION_HIDDEN_POSITION;
                }

                if (selectionLevel == 1 && SelectedGame != null)
                {
                    int lnCount = SelectedGame.DescriptionLineCount;
                    if (lnCount < 1)
                        lnCount = 1;
                    infoBox.Motion.TargetPos = INFOBOX_SHOWN_POSITION - new Vector2(0f, 0.015f + 0.029f * (lnCount - 1));
                }

                if (SelectedGame == null || SelectedGame.Name.Length == 0)
                {
                    infoBox.Motion.TargetPos = INFOBOX_ALL_HIDDEN_POSITION;
                }
            }

        }
    }
}

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
        const float LAYER_BACK = 1.0f;
        const float LAYER_FRONT = 0.0f;
        const float LAYER_ZOOMING_ITEM = 0.1f;
        const float LAYER_DODGING_ITEM = 0.3f;
        const float LAYER_GRID_ITEMS = 0.9f;

        const float PANEL_ZOOM_STARTUP = 0.45f;
        const float PANEL_ZOOM_REGULAR = 0.45f; //0.16f;
        const float PANEL_ZOOM_DETAILED_VIEW = 1.5f; //2.857f;
        const float PANEL_DELTA_GRID_X = 0.16f;
        const float PANEL_DELTA_GRID_Y = 0.12f;
        const float PANEL_SPEED_SHIFT = 4.2f;
        const float PANEL_SIZE_X = 1.333f;
        const float PANEL_SIZE_Y = 1.0f;
        const float PANEL_ZOOM_TARGET_QUITTING = 0.01f;
        const float PANEL_ZOOM_SPEED_QUITTING = 0.005f;
        const float PANEL_ZOOM_SPEED_REGULAR = 0.005f;
        const float PANEL_ZOOM_SPEED_ABORTQUITTING = 0.005f;
        static Vector2 PANEL_INITIAL_SHIFT_POS = new Vector2(-1.5f,-3f);

        const float CURSOR_SCALE_REGULAR = 0.8f; //5.9375f;
        public const float CURSOR_DISCOVERY_RANGE = 1.3f;
        public const float CURSOR_DESTRUCTION_RANGE = 4f;
        const float CURSOR_MARGIN_X = 0.15f;
        const float CURSOR_MARGIN_Y = 0.15f;
        static Vector2 CURSOR_INITIAL_POSITION = new Vector2(0.7f, 0.2f);

        public const float THUMBNAIL_SCALE_UNSELECTED = 0.44f; //0.6f; //0.54f; //1.5625f;
        const float THUMBNAIL_SCALE_SELECTED = 0.51f; //0.7f; //0.65f; //2f;
        public const float THUMBNAIL_MAX_WIDTH_PIXELS = 320f;
        public const float THUMBNAIL_MAX_HEIGHT_PIXELS = 240f;
        const float THUMBNAIL_FADE_SPEED = 0.3f;

        static Vector2 INFOBOX_SHOWN_POSITION = new Vector2(0.05f, 0.895f);
        static Vector2 INFOBOX_HIDDEN_POSITION = new Vector2(0.05f, 0.96f);
        static Vector2 HELPTEXT_SHOWN_POSITION = new Vector2(0.15f, 0.13f);
        static Vector2 HELPTEXT_HIDDEN_POSITION = new Vector2(0.15f, -0.2f);
        static Vector2 CREDITS_SHOWN_POSITION = new Vector2(0.4f, 0.145f);
        static Vector2 CREDITS_HIDDEN_POSITION = new Vector2(0.4f, -0.22f);
        const float CREDITS_SCALE_DEFAULT = 0.6f;
        const float INFOBOX_SPEED_MOVE = 3.8f;
        
        const float TIME_BEFORE_GAME_LAUNCH = 0.7f;
        const float TIME_BEFORE_EXIT = 1.2f;
        const float TIME_BEFORE_EXIT_CONTINUES = 0.6f;

        // maximum sizes of grid
        public double GridMaxX=128, GridMaxY=128;

        Dictionary<string, GameThumbnail> thumbnailsCache = new Dictionary<string, GameThumbnail>();

        //GameCollection gamesList;

        // cursor is the graphics selection thingy         
        GameThumbnailCursor cursor;

        // box showing info of a game such as title and download progressContributionSingleFile
        GameInfoBox infoBox;

        // showing controls help message
        Spritelet controlsHelpBitmap;

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
            cursor.Motion.Position = CURSOR_INITIAL_POSITION;

            // info box - will be added to parent upon OnNewParent() event
            infoBox = new GameInfoBox();
            infoBox.Motion.Position = INFOBOX_HIDDEN_POSITION;

            // controls help 
            controlsHelpBitmap = new Spritelet("keymap");
            controlsHelpBitmap.Motion.Scale = 0.5f;
            controlsHelpBitmap.Motion.Position = HELPTEXT_HIDDEN_POSITION;
            controlsHelpBitmap.Motion.TargetPosSpeed = INFOBOX_SPEED_MOVE;

            // credits
            creditsBitmap = new Spritelet("credits.png");
            creditsBitmap.Motion.Scale = CREDITS_SCALE_DEFAULT;
            creditsBitmap.Motion.Position = CREDITS_HIDDEN_POSITION;
            creditsBitmap.Motion.TargetPos = CREDITS_HIDDEN_POSITION;
            creditsBitmap.Motion.TargetPosSpeed = INFOBOX_SPEED_MOVE;

            // default zoom
            Motion.Zoom = PANEL_ZOOM_DETAILED_VIEW;
            Motion.ZoomTarget = PANEL_ZOOM_DETAILED_VIEW;
            Motion.ZoomSpeed = PANEL_ZOOM_SPEED_REGULAR;
            Motion.ZoomCenterTarget = cursor.Motion;
        }

        public override void OnUpdateList(GameCollection gl)
        {
            this.gl = gl;
        }

        // shorthand method to select the game currently indicated by cursor
        protected void SelectGameBelowCursor()
        {
            if (gl != null)
            {
                GardenItem g = gl.FindGameAt(cursor.GridPosition);
                SelectedGame = g;
                infoBox.ClearProgressBar();
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
            Parent.Add(controlsHelpBitmap);
            Parent.Add(creditsBitmap);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (SelectedGame == null)
            {
                infoBox.SetGameInfo(SelectedGame);
            }

            // handle download/install/launching of a game
            if (isGameLaunchOngoing && timeLaunching < TIME_BEFORE_GAME_LAUNCH)
            {
                timeLaunching += p.Dt;
                GameThumbnail th = thumbnailsCache[SelectedGame.GameID];
                float sc = (1f + timeLaunching/3f);
                th.Motion.ScaleTarget = sc; // blow up size of thumbnail while user requests launch
                //th.Motion.ScaleSpeed = 0.00005f;
                cursor.Motion.ScaleTarget = sc;
                cursor.Motion.ScaleSpeed = th.Motion.ScaleSpeed / SelectedGame.ScaleIcon; // TODO correct ScaleIcon?
            }

            // launch of a game
            if (isGameLaunchConfirmed)
            {
                cursor.Motion.ScaleTarget = CURSOR_SCALE_REGULAR;
                // check for mystery game                    
                if (SelectedGame.GameID.Equals("igg_mysterygame"))
                {
                    //GameCollection lib = GardenGame.Instance.GameLib.GetList();
                    //GardenItem grnd = lib.GetRandomInstalledGame();
                    //GardenGame.Instance.music.FadeOut();
                    //GameThumbnail thumb = thumbnailsCache[grnd.GameID];
                    //GardenGame.Instance.ActionLaunchGame(grnd, thumb);
                    throw new NotImplementedException("igg_mysterygame");
                }
                else
                {
                    GameThumbnail thumb = thumbnailsCache[SelectedGame.GameID];
                    if (SelectedGame.IsInstalled)
                    {
                        GardenGame.Instance.music.FadeOut();
                        GardenGame.Instance.ActionLaunchGame(SelectedGame, thumb);
                    }
                    else if (SelectedGame.IsWebGame)
                    {
                        GardenGame.Instance.ActionLaunchWebsitePlayGame(SelectedGame,thumb);
                    }
                    else
                    {
                        GardenGame.Instance.ActionDownloadAndInstallGame(SelectedGame);
                    }
                }
                isGameLaunchOngoing = false;
                isGameLaunchConfirmed = false;
                timeLaunching = 0f;
            }

            // handle exit key
            if (isExiting)
            {
                GardenGame.Instance.music.FadeOut();
                timeExiting += p.Dt;
                if (timeExiting > TIME_BEFORE_EXIT)
                {
                    parentMenu.background.Motion.ScaleModifier = 1f / (1f + (timeExiting-TIME_BEFORE_EXIT) / 11f);
                    if (!isExitingUnstoppable)
                    {
                        GardenGame.Instance.ExitGame();
                        isExitingUnstoppable = true;
                    }
                    return;
                }
            }
            else
            {
                if (timeExiting > 0f)
                {
                    if(GardenGame.Instance.music.UserWantsMusic)
                        GardenGame.Instance.music.FadeIn();
                    timeExiting = 0f;
                }
            }

            //-- website launch
            if (isLaunchWebsite)
            {
                if (SelectedGame != null)
                {
                    GameThumbnail thumb = thumbnailsCache[SelectedGame.GameID];
                    GardenGame.Instance.ActionLaunchWebsite(SelectedGame, thumb);
                }
                isLaunchWebsite = false;
            }

            //-- loop all nearby games adapt their display properties where needed
            if (gl == null)
                return;
            GardenItem g;

            // upd cache with possibly new items around cursor
            List<GardenItem> c = gl.GetItemsAround((int)cursor.GridPosition.X, (int)cursor.GridPosition.Y, 2);
            if (SelectedGame != null)
                c.Add(SelectedGame);
            for (int i = c.Count - 1; i >= 0; i--)
            {
                g = c[i];

                // if GameThumbnail for current game does not exist yet, create it                
                if (!thumbnailsCache.ContainsKey(g.GameID))
                {
                    // create now
                    GameThumbnail th = new GameThumbnail(g);
                    Add(0, th);
                    thumbnailsCache.Add(g.GameID, th);
                    //th.Position = new Vector2(RandomMath.RandomBetween(-0.4f,2.0f), RandomMath.RandomBetween(-0.4f,1.4f) );
                    //th.Scale = RandomMath.RandomBetween(0.01f, 0.09f); 
                    // create with new position and scale
                    th.Motion.Position = Screen.Center;
                    th.Motion.Scale = 0.05f;
                    th.Motion.ScaleTarget = 0.05f;
                    th.Motion.ScaleSpeed = 0.01f; // TODO const

                    th.DrawInfo.LayerDepth = LAYER_GRID_ITEMS + ((float)th.ID) * float.Epsilon;
                    th.Visible = false;
                    th.ColorB.Intensity = 0.0f;

                    // special case thumbnails 
                    if (g.GameID.Equals("igg_controls"))
                        th.Motion.Add(new MyFuncyModifier( delegate(float v) { return v/22.3f; }, "Rotate"));
                }
            }
                
            // visit all cached items and adjust positions, visibility, etc.
            List<GameThumbnail> toRemoveFromCache = new List<GameThumbnail>();
            foreach(GameThumbnail th in thumbnailsCache.Values)
            {
                g = th.Game;

                // check if out of range. If so, remove from cache later
                if (cursor.GameletOutOfRange(th))
                {
                    toRemoveFromCache.Add(th);
                    th.Delete = true;
                }
                else
                {
                    // check if thnail visible and in range. If so, start displaying it (fade in)
                    if (!th.Visible && cursor.GameletInRange(th))
                    {
                        th.LoadInBackground();
                        th.ColorB.Intensity = 0f;
                    }

                    if (th.IsLoaded() && cursor.GameletInRange(th))
                    {
                        if (th.Game.IsGrowable)
                            th.ColorB.FadeTarget = (0.65f + 0.35f * g.InstallProgress);
                        else
                            th.ColorB.FadeTarget = 1f;
                    }
                    else
                        th.ColorB.FadeTarget = 0f;

                }

                th.Motion.ScaleTarget = THUMBNAIL_SCALE_UNSELECTED;
                th.ColorB.FadeSpeed = THUMBNAIL_FADE_SPEED;

                // coordinate position where to move a game thumbnail to 
                Vector2 targetPos = (g.Position - PanelShiftPos) * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);
                th.Motion.TargetPos = targetPos;
                th.Motion.TargetPosSpeed = PANEL_SPEED_SHIFT;

            } // end for loop over all games
            foreach (GameThumbnail th in toRemoveFromCache)
            {
                thumbnailsCache.Remove(th.Game.GameID);
            }

            // --- for selected game only
            if (SelectedGame != null)
            {
                g = SelectedGame;
                // update text box with currently selected game info
                infoBox.SetGameInfo(g);

                //-- helpful controls text
                if (g.GameID.Equals("igg_controls"))
                {
                    controlsHelpBitmap.Motion.TargetPos = HELPTEXT_SHOWN_POSITION;
                    g.Name = GardenConfig.Instance.ServerMsg;
                }
                else
                {
                    controlsHelpBitmap.Motion.TargetPos = HELPTEXT_HIDDEN_POSITION;
                }

                //-- credits text
                if (g.GameID.Equals("igg_credits"))
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

        }

        public override void OnChangedSelectedGame(GardenItem newSel, GardenItem oldSel)
        {
            //
        }

        public override void OnUserInput(GamesPanel.UserInput inp)
        {
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
                    Motion.ZoomSpeed = PANEL_ZOOM_SPEED_QUITTING ;
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
                        GameThumbnail th = thumbnailsCache[SelectedGame.GameID];
                        if (th != null)
                        {
                            switch (selectionLevel)
                            {
                                case 0:
                                    // select once - zoom in on selected game
                                    Motion.ZoomTarget = PANEL_ZOOM_DETAILED_VIEW;
                                    Motion.ZoomSpeed = PANEL_ZOOM_SPEED_REGULAR; // 0.01f; 
                                    //Motion.ZoomCenter = cursor.Motion.PositionAbs;
                                    Motion.ZoomCenterTarget = cursor.Motion;
                                    SelectedGame.Refresh();
                                    //infoBox.Motion.TargetPos = INFOBOX_SHOWN_POSITION - new Vector2(0f,0.05f * (SelectedGame.DescriptionLineCount-1));
                                    //infoBox.Motion.TargetPosSpeed = INFOBOX_SPEED_MOVE;
                                    selectionLevel++;
                                    break;
                                case 1:
                                    // select again - install or launch game if selection key pressed long enough.
                                    isGameLaunchOngoing = true;
                                    break;
                            }


                        }
                    }
                    break;

                case UserInput.STOP_SELECT:
                    // if not launched long enough, reset - no action
                    if (timeLaunching < TIME_BEFORE_GAME_LAUNCH)
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
                    GardenGame.Instance.music.ToggleMusic();
                    break;

            } // switch(inp)

            if (selectionLevel == 0)
            {
                infoBox.Motion.TargetPos = INFOBOX_HIDDEN_POSITION;
                infoBox.Motion.TargetPosSpeed = INFOBOX_SPEED_MOVE;
            }

            if (selectionLevel == 1)
            {
                int lnCount = 1;
                if (SelectedGame != null)
                {
                    lnCount = SelectedGame.DescriptionLineCount;
                }
                infoBox.Motion.TargetPos = INFOBOX_SHOWN_POSITION - new Vector2(0f, 0.029f * (lnCount - 1));
                infoBox.Motion.TargetPosSpeed = INFOBOX_SPEED_MOVE;
            }

        }
    }
}

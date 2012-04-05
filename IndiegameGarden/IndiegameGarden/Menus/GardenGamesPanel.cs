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

        public const float PANEL_ZOOM_REGULAR = 1f; //0.16f;
        public const float PANEL_DELTA_GRID_X = 0.16f;
        public const float PANEL_DELTA_GRID_Y = 0.12f;
        public const float PANEL_SPEED_SHIFT = 2.1f;
        public const float PANEL_SIZE_X = 1.333f;
        public const float PANEL_SIZE_Y = 1.0f;
        public const float PANEL_ZOOM_TARGET_QUITTING = 0.001f;
        public const float PANEL_ZOOM_SPEED_QUITTING = 0.005f;
        public const float PANEL_ZOOM_SPEED_ABORTQUITTING = 0.05f;

        public const float CURSOR_SCALE_REGULAR = 0.8f; //5.9375f;
        public const float CURSOR_DISCOVERY_RANGE = 0.55f;
        public const float THUMBNAIL_SCALE_UNSELECTED_UNINSTALLED = 0.18f;
        public const float THUMBNAIL_SCALE_UNSELECTED = 0.44f; //0.6f; //0.54f; //1.5625f;
        public const float THUMBNAIL_SCALE_SELECTED = 0.51f; //0.7f; //0.65f; //2f;
        public const float THUMBNAIL_SCALE_SELECTED1 = 2f; //2.857f;
        static Vector2 INFOBOX_SHOWN_POSITION = new Vector2(0.05f, 0.895f);
        static Vector2 INFOBOX_HIDDEN_POSITION = new Vector2(0.05f, 0.96f);
        const float INFOBOX_SPEED_MOVE = 3.8f;
        const float TIME_BEFORE_GAME_LAUNCH = 0.7f;
        const float TIME_BEFORE_EXIT = 0.9f;

        /// <summary>
        /// my motion behavior controls
        /// </summary>
        public MotionBehavior MotionB;

        // maximum sizes of grid
        public double GridMaxX=32, GridMaxY=32;

        Dictionary<string, GameThumbnail> thumbnailsCache = new Dictionary<string, GameThumbnail>();
        
        // cursor is the graphics selection thingy         
        GameThumbnailCursor cursor;

        // box showing info of a game such as title and download progressContributionSingleFile
        GameInfoBox infoBox;

        // textbox showing controls help message
        FloatingTextMessage controlsHelpText;

        // UI related vars - related to whether user indicates to quit program or user cancelled this
        bool isExiting = false;
        bool isGameLaunchOngoing = false;
        bool isLaunchWebsite = false;
        float timeExiting = 0f;
        float timeLaunching = 0f;
        Vector2 PanelShiftPos = Vector2.Zero;
        int selectionLevel = 0;
        GameChooserMenu parentMenu;

        public GardenGamesPanel(GameChooserMenu parent)
        {
            parentMenu = parent;
            MotionB = new MotionBehavior();
            Add(MotionB);

            // cursor
            cursor = new GameThumbnailCursor();
            Add(cursor);
            cursor.Motion.Scale = CURSOR_SCALE_REGULAR;
            Motion.Zoom = PANEL_ZOOM_REGULAR;
            //cursor.Visible = false;

            // info box - will be added to parent upon OnNewParent() event
            infoBox = new GameInfoBox();
            infoBox.Motion.Position = INFOBOX_HIDDEN_POSITION;

            // controls help text
            controlsHelpText = new FloatingTextMessage();
            controlsHelpText.Motion.Position = new Vector2(0.04f, 0.04f);
            controlsHelpText.Text = "CONTROLS:\n\n" + 
                                    "ARROWs = Move cursor          ENTER = Select game\n" +
                                    "ESCAPE = Back                       Hold ESCAPE = Quit the garden\n" +
                                    "Hold ENTER = Grow game in your garden / Play game\n" +
                                    "W = Launch game's website";
        }

        public override void OnUpdateList(GameCollection gl)
        {
            // first process old list - start fading away of items
            for (int i = 0; i < gl.Count; i++)
            {
                GardenItem g = gl[i];
                if (thumbnailsCache.ContainsKey(g.GameID))
                {
                    GameThumbnail th = thumbnailsCache[g.GameID];
                    th.ColorB.FadeToTarget(0f,4f);
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
            GardenItem g = gl.FindGameAt(cursor.GridPosition);
            SelectedGame = g;
            infoBox.ClearProgressBar();
            if (g!= null)
                g.Refresh();
        }

        protected override void OnNewParent()
        {
            base.OnNewParent();

            // some items are part of the parent, to avoid scaling issues in GardenGamesPanel
            // (which get rescaled/zoomed based on user input).
            Parent.Add(infoBox);
            Parent.Add(controlsHelpText);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            GameThumbnail th = null;

            base.OnUpdate(ref p);

            // update text box with currently selected game info
            infoBox.SetGameInfo(SelectedGame);

            // handle download/install/launching of a game
            if (isGameLaunchOngoing)
            {
                timeLaunching += p.Dt;
                th = thumbnailsCache[SelectedGame.GameID];
                th.MotionB.ScaleTarget = THUMBNAIL_SCALE_SELECTED * (1 + timeLaunching); // blow up size of thumbnail while user requests launch
                th.MotionB.ScaleSpeed = 0.0004f;

                if (timeLaunching > TIME_BEFORE_GAME_LAUNCH)
                {
                    // check for mystery game
                    if (SelectedGame.GameID.Equals("igg_mysterygame"))
                    {
                        GameCollection lib = GardenGame.Instance.GameLib.GetList();
                        GardenItem grnd = lib.GetRandomInstalledGame();
                        GardenGame.Instance.music.FadeOut();
                        GardenGame.Instance.ActionLaunchGame(grnd);
                        isGameLaunchOngoing = false;
                        return;

                    }
                    else
                    {
                        if (SelectedGame.IsInstalled)
                        {
                            GardenGame.Instance.music.FadeOut();
                            GardenGame.Instance.ActionLaunchGame(SelectedGame);
                            isGameLaunchOngoing = false;
                            return;
                        }
                        else
                        {
                            GardenGame.Instance.ActionDownloadAndInstallGame(SelectedGame);
                        }
                    }
                    isGameLaunchOngoing = false;
                }
            }

            // handle exit key
            if (isExiting)
            {
                GardenGame.Instance.music.FadeOut();
                timeExiting += p.Dt;
                if (timeExiting > TIME_BEFORE_EXIT)
                {
                    GardenGame.Instance.ExitGame();
                    //isExiting = false;
                    return;
                }
            }
            else
            {
                GardenGame.Instance.music.FadeIn(); 
                timeExiting = 0f;
            }

            //-- website launch
            if (isLaunchWebsite)
            {
                if (SelectedGame != null)
                {
                    GardenGame.Instance.ActionLaunchWebsite(SelectedGame);
                }
                isLaunchWebsite = false;
            }

            //-- helpful controls text
            if (SelectedGame != null && SelectedGame.GameID.Equals("igg_controls"))
            {
                controlsHelpText.FadeIn();
                SelectedGame.Name = GardenGame.Instance.Config.ServerMsg;
            }
            else
            {
                controlsHelpText.FadeOut();
            }

            //-- loop all games adapt their display properties where needed
            if (gl == null)
                return;
            GardenItem g;
            for (int i = 0; i < gl.Count; i++)
            {
                // fetch that game from list
                g = gl[i];

                // if GameThumbnail for current game does not exist yet, create it                
                if (!thumbnailsCache.ContainsKey(g.GameID))
                {
                    // create now
                    th = new GameThumbnail(g);
                    Add(0,th);
                    thumbnailsCache.Add(g.GameID, th);
                    //th.Position = new Vector2(RandomMath.RandomBetween(-0.4f,2.0f), RandomMath.RandomBetween(-0.4f,1.4f) );
                    //th.Scale = RandomMath.RandomBetween(0.01f, 0.09f); 
                    // create with new position and scale
                    th.Motion.Position = new Vector2(0.5f, 0.5f);
                    th.Motion.Scale = 0.01f;

                    th.DrawInfo.LayerDepth = LAYER_GRID_ITEMS;
                    th.Visible = false;
                    th.ColorB.Intensity = 0.0f;
                }else{
                    // retrieve GameThumbnail from cache
                    th = thumbnailsCache[g.GameID];
                }
                
                // check if thnail visible and in range. If so, start displaying it (fade in)
                if (!th.Visible && cursor.GameletInRange(th))
                {
                    th.Enable();
                    th.ColorB.Intensity = 0f;
                }
                            
                th.ColorB.FadeTarget = (0.65f + 0.35f * g.InstallProgress);
                if (!(isGameLaunchOngoing && g == SelectedGame))
                {
                    if (g.IsInstalling)
                    {
                        th.MotionB.ScaleTarget = (0.9f + 0.35f * g.InstallProgress) *
                                                ((g == SelectedGame) ? THUMBNAIL_SCALE_SELECTED : THUMBNAIL_SCALE_UNSELECTED);
                        th.MotionB.ScaleSpeed = 0.00007f;
                    }
                    else
                    {
                        th.MotionB.ScaleTarget = 1f; // (0.85f + 0.15f * g.InstallProgress);
                        //th.MotionB.ScaleSpeed = 0.03f;
                        // displaying selected thumbnails larger
                        if (g == SelectedGame)
                        {
                            th.MotionB.ScaleTarget *= THUMBNAIL_SCALE_SELECTED * g.ScaleIcon;
                            th.MotionB.ScaleSpeed = 0.00003f;
                        }
                        else
                        {
                            if (g.IsInstalled || !g.IsGrowable)
                                th.MotionB.ScaleTarget *= THUMBNAIL_SCALE_UNSELECTED * g.ScaleIcon;
                            else
                                th.MotionB.ScaleTarget *= THUMBNAIL_SCALE_UNSELECTED_UNINSTALLED * g.ScaleIcon;
                            th.MotionB.ScaleSpeed = 0.00003f;
                        }
                    }
                }
                th.ColorB.FadeSpeed = 0.15f;// 0.15f;

                // coordinate position where to move a game thumbnail to 
                Vector2 targetPos = (g.Position - PanelShiftPos) * new Vector2(PANEL_DELTA_GRID_X,PANEL_DELTA_GRID_Y);
                th.MotionB.Target = targetPos;
                th.MotionB.TargetSpeed = 4f;

                // cursor where to move to
                cursor.MotionB.Target = (cursor.GridPosition - PanelShiftPos) * new Vector2(PANEL_DELTA_GRID_X, PANEL_DELTA_GRID_Y);

                // panel shift effect when cursor hits edges of panel
                Vector2 cp = cursor.Motion.PositionAbs;
                float chw = cursor.DrawInfo.WidthAbs / 2.0f; // cursor-half-width
                float chh = cursor.DrawInfo.HeightAbs / 2.0f; // cursor-half-height
                float dx = PANEL_SPEED_SHIFT * p.Dt;
                const float xMargin = 0.15f; // TODO into gui props
                const float yMargin = 0.15f;
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
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);

            // DEBUG
            //if (SelectedGame != null)
            //    Screen.DebugText(0f, 0f, "Selected: " + gl.IndexOf(SelectedGame) + " " + SelectedGame.GameID );
            //Screen.DebugText(0f, 0.1f, "Zoom: " + Motion.Zoom);
        }

        public override void OnChangedSelectedGame(GardenItem newSel, GardenItem oldSel)
        {
            // unselect the previous game DEBUG
            /*
            if (oldSel != null)
            {
                GameThumbnail th = thumbnailsCache[oldSel.GameID];
                if (th != null)
                {
                    th.MotionB.ScaleTarget = THUMBNAIL_SCALE_UNSELECTED * oldSel.ScaleIcon;
                    th.MotionB.ScaleSpeed = 0.01f;
                }
            }
             */
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
                    selectionLevel = 0;
                    MotionB.ZoomTarget = PANEL_ZOOM_TARGET_QUITTING ;
                    MotionB.ZoomSpeed = PANEL_ZOOM_SPEED_QUITTING ;
                    break;
                
                case UserInput.STOP_EXIT:
                    isExiting = false;
                    selectionLevel = 0;
                    MotionB.ZoomTarget = PANEL_ZOOM_REGULAR;
                    MotionB.ZoomSpeed = PANEL_ZOOM_SPEED_ABORTQUITTING ;
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
                                    MotionB.ZoomTarget = THUMBNAIL_SCALE_SELECTED1;
                                    Motion.ZoomCenter = th.Motion.PositionAbs;
                                    MotionB.ZoomSpeed = 0.05f;
                                    SelectedGame.Refresh();
                                    //infoBox.MotionB.Target = INFOBOX_SHOWN_POSITION - new Vector2(0f,0.05f * (SelectedGame.DescriptionLineCount-1));
                                    //infoBox.MotionB.TargetSpeed = INFOBOX_SPEED_MOVE;
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
                    isGameLaunchOngoing = false;
                    timeLaunching = 0f;
                    break;

                case UserInput.LAUNCH_WEBSITE:
                    isLaunchWebsite = true;
                    break;

            } // switch(inp)

            if (selectionLevel == 0)
            {
                infoBox.MotionB.Target = INFOBOX_HIDDEN_POSITION;
                infoBox.MotionB.TargetSpeed = INFOBOX_SPEED_MOVE;
            }

            if (selectionLevel == 1)
            {
                int lnCount = 1;
                if (SelectedGame != null)
                {
                    lnCount = SelectedGame.DescriptionLineCount;
                }
                infoBox.MotionB.Target = INFOBOX_SHOWN_POSITION - new Vector2(0f, 0.029f * (lnCount - 1));
                infoBox.MotionB.TargetSpeed = INFOBOX_SPEED_MOVE;
            }

        }
    }
}

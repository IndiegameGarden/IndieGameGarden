// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

using IndiegameGarden.Base;

namespace IndiegameGarden.Download
{
    /**
     * A downloader task for a thumbnail that selects either PNG or JPG depending on what's available
     */
    public class ThumbnailDownloader: BaseDownloader
    {
        IndieGame game;

        /// <summary>
        /// a lock object to ensure one download at a time takes place.
        /// </summary>
        //static Object downloadingToken = new Object();

        /// <summary>
        /// construct a new thumbnail downloader for game; downloading from default server
        /// location.
        /// </summary>
        public ThumbnailDownloader(IndieGame g)
        {
            this.game = g;
        }

        protected override void StartInternal()
        {
            string filename = game.GameIDwithVersion + ".jpg";
            string urlDl = GardenGame.Instance.Config.GetThumbnailURL(filename);
            string toLocalFolder = GardenGame.Instance.Config.ThumbnailsFolder;
            //lock (downloadingToken) // TODO check if sequentializing locks needed
            {
                InternalDoDownload(urlDl, filename, toLocalFolder, true);

                // if jpg not ok, try the png
                if (IsFinished() && !IsSuccess() && !doAbort )
                {
                    filename = game.GameIDwithVersion + ".png";
                    urlDl = GardenGame.Instance.Config.GetThumbnailURL(filename);
                    InternalDoDownload(urlDl, filename, toLocalFolder, true);
                    // status may have become successful by now.
                }
            }
        }

    }
}

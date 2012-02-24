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
     * A downloader task for a thumbnail
     */
    public class ThumbnailDownloader: BaseDownloader
    {
        IndieGame game;
        /// <summary>
        /// a lock object to ensure one download at a time takes place.
        /// </summary>
        static Object downloadingToken = new Object();

        /// <summary>
        /// construct a new thumbnail downloader for game; downloading from default server
        /// location.
        /// </summary>
        /// <param name="game">the game to download thumbnail for</param>
        public ThumbnailDownloader(IndieGame game)
        {
            this.game = game;
        }

        protected override void StartInternal()
        {
            string filename = GardenGame.Instance.Config.GetThumbnailFilename(game);
            string urlDl = GardenGame.Instance.Config.GetThumbnailURL(game);
            string toLocalFolder = GardenGame.Instance.Config.ThumbnailsFolder;
            //lock (downloadingToken) // TODO check if locks needed
            {
                InternalDoDownload(urlDl, filename, toLocalFolder, true);
            }
        }

    }
}

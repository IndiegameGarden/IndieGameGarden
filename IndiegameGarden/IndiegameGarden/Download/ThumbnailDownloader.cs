// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    /**
     * A downloader task for a thumbnail
     */
    public class ThumbnailDownloader: BaseDownloader
    {
        string gameID;

        /// <summary>
        /// construct a new thumbnail downloader for game with given ID. 
        /// </summary>
        /// <param name="GameID">ID of game to load thumbnail for</param>
        public ThumbnailDownloader(string gameID)
        {
            this.gameID = gameID;            
        }

        public override void Start()
        {
            string filename = gameID + ".jpg"; // TODO jpg or png or both?
            string url = GardenGame.Instance.Config.ThumbnailsServerURL + filename;
            InternalDoDownload(url, filename, GardenGame.Instance.Config.GetFolder(GardenGame.Instance.Config.ThumbnailsFolder), true);
        }

    }
}

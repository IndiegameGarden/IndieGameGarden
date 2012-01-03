// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    /// <summary>
    /// delegate to be used for callback if a thumbnail is downloaded OK.
    /// </summary>
    /// <param name="gameID">ID of game for which thumbnail is downloaded OK</param>
    public delegate void OnDownloadOK(string gameID);

    /**
     * A downloader for thumbnails with callback functionality upon download complete.
     */
    public class ThumbnailDownloader: BaseDownloader
    {
        string filename;
        string gameID;
        OnDownloadOK dlEndedDelegate;

        /// <summary>
        /// construct a new thumbnail downloader for game with given ID. Calls a method() when download ready and ok.
        /// </summary>
        /// <param name="gameID">ID of game to load thumbnail for</param>
        /// <param name="onDownloadEnded">optional OnDownloadOK delegate with method to be called when download ends ok</param>
        public ThumbnailDownloader(string gameID, OnDownloadOK dlEndedDelegate)
        {
            this.gameID = gameID;
            this.filename = gameID + ".jpg" ;
            this.dlEndedDelegate = dlEndedDelegate;
        }

        public override void Start()
        {
            string url = GardenGame.Instance.Config.ThumbnailsServerURL + filename;
            InternalStartDownload(url, filename, GardenGame.Instance.Config.ThumbnailsFolder, true);
        }

        public override void OnDownloadEnded(Downloader dl)
        {
            if (dlEndedDelegate != null)
            {
                dlEndedDelegate.DynamicInvoke(new object[]{gameID} );
            }
        }

    }
}

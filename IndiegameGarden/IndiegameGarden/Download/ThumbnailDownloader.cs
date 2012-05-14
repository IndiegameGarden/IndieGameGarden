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
     * A downloader task for downloading a thumbnail for a GardenItem
     */
    public class ThumbnailDownloader: BaseDownloader
    {
        GardenItem gi;

        /// <summary>
        /// construct a new thumbnail downloader for game; downloading from default server
        /// location.
        /// </summary>
        public ThumbnailDownloader(GardenItem g)
        {
            this.gi = g;
        }

        protected override void StartInternal()
        {
            string filename = gi.ThumbnailFile;
            string urlDl = gi.ThumbnailURL; 
            string toLocalFolder = GardenConfig.Instance.ThumbnailsFolder;
            InternalDoDownload(urlDl, filename, toLocalFolder, true);
        }

    }
}

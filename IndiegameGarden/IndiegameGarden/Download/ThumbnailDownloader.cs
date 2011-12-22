// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    public class ThumbnailDownloader: BaseDownloader
    {
        string filename;

        public ThumbnailDownloader(string gameID)
        {
            this.filename = gameID + ".png" ;
        }

        public override void Start()
        {
            string url = GardenMain.Instance.storageConfig.ThumbnailsServerURL + filename;
            InternalStartDownload(url, filename, GardenMain.Instance.storageConfig.ThumbnailsFolder);
        }

        public override void HandleDownloadEndedEvent(object sender, DownloaderEventArgs e)
        {
        }

    }
}

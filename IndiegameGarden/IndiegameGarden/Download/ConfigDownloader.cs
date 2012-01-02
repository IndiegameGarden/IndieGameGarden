// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    public class ConfigDownloader: BaseDownloader
    {
        string filename;

        public ConfigDownloader(string filename)
        {
            this.filename = filename ;
        }

        public override void Start()
        {
            string url = GardenMain.Instance.storageConfig.ConfigFilesServerURL + filename;
            InternalStartDownload(url, filename, GardenMain.Instance.storageConfig.ConfigFilesFolder);
        }

        public override void HandleDownloadEndedEvent(object sender, DownloaderEventArgs e)
        {
        }

    }
}

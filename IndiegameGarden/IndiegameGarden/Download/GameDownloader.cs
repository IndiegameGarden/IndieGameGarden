// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyDownloader.Core;
using IndiegameGarden.Store;

namespace IndiegameGarden.Download
{
    public class GameDownloader: BaseDownloader
    {
        IndieGame game;

        public GameDownloader(IndieGame game): base()
        {
            this.game= game;
        }

        public override void Start()
        {
            string fn = game.GameID + ". " + ExtractFileExtension(game.PackedFileURL);
            string toLocalFolder = GardenMain.Instance.storageConfig.PackedFilesFolder;
            InternalStartDownload(game.PackedFileURL, fn, toLocalFolder, game.PackedFileMirrors);
        }

        private string ExtractFileExtension(string url)
        {
            int i = url.LastIndexOf('.');
            if (i == -1)
                return "";
            return url.Substring(i + 1);
        }

        public override void HandleDownloadEndedEvent(object sender, DownloaderEventArgs e)
        {
        }
    }
}

﻿// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System.IO;
using MyDownloader.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Download
{
    /**
     * a downloader to download a game file such as .zip / .rar / .exe but also .ogg (for music tracks)
     * If file already exists, this ITask finishes successfully.
     */
    public class GameDownloader: BaseDownloader
    {
        protected GardenItem game;

        public GameDownloader(GardenItem game): base()
        {
            this.game= game;
            this.segmentsUsedInDownload = 3;
        }

        protected override void StartInternal()
        {
            status = ITaskStatus.RUNNING;
            string fn = GardenGame.Instance.Config.GetPackedFileName(game);
            string toLocalFolder = GardenGame.Instance.Config.PackedFilesFolder;
            string filePath = Path.Combine(toLocalFolder , fn);
            if (File.Exists(filePath))
            {
                // skip download step
                status = ITaskStatus.SUCCESS;
            }
            else
            {
                Settings.Default.MaxRetries = 4; // FIXME hack
                InternalDoDownload(game.PackedFileURL, fn, toLocalFolder, false, game.PackedFileMirrors);
                Settings.Default.MaxRetries = 0;
            }
        }

    }
}

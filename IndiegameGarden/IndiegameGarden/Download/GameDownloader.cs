// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

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
            this.segmentsUsedInDownload = 1;
        }

        protected override void StartInternal()
        {
            status = ITaskStatus.RUNNING;
            string fn = game.PackedFileName;
            string toLocalFolder = game.PackedFileFolder;
            string filePath = Path.Combine(toLocalFolder , fn);
            if (File.Exists(filePath))
            {
                // skip download step
                status = ITaskStatus.SUCCESS;
            }
            else
            {
                MaxRetries = 3;
                InternalDoDownload_MirrorRetry(game.PackedFileURL, fn, toLocalFolder, false, game.PackedFileMirrors);
            }
        }

    }
}

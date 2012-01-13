// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System.IO;
using MyDownloader.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Download
{
    /**
     * a downloader to download a game file such as .zip / .rar
     * If file already exists, this ITask finishes successfully.
     */
    public class GameDownloader: BaseDownloader
    {
        protected IndieGame game;

        public GameDownloader(IndieGame game): base()
        {
            this.game= game;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;
            string fn = GardenGame.Instance.Config.GetPackedFileName(game);
            string toLocalFolder = GardenGame.Instance.Config.PackedFilesFolder;
            string filePath = toLocalFolder + "\\" + fn;
            if (File.Exists(filePath))
            {
                // skip download step
                status = ITaskStatus.SUCCESS;
            }
            else
            {
                InternalDoDownload(game.PackedFileURL, fn, toLocalFolder, false, game.PackedFileMirrors);
            }
        }

    }
}

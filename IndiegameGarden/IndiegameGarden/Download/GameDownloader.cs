// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using MyDownloader.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Download
{
    /**
     * a downloader to download a game file such as .zip / .rar
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
            string fn = game.PackedFileName; 
            string toLocalFolder = GardenGame.Instance.Config.GetFolder(GardenGame.Instance.Config.PackedFilesFolder); 
            InternalDoDownload(game.PackedFileURL, fn, toLocalFolder, false, game.PackedFileMirrors);
        }

    }
}

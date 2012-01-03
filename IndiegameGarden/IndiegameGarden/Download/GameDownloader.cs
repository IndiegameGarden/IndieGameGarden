// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using MyDownloader.Core;
using IndiegameGarden.Store;

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
            string fn = game.PackedFileName; 
            string toLocalFolder = GardenGame.Instance.Config.PackedFilesFolder;
            InternalStartDownload(game.PackedFileURL, fn, toLocalFolder, game.PackedFileMirrors);
        }

        public override void OnDownloadEnded(Downloader dl)
        {
            // TODO
        }
    }
}

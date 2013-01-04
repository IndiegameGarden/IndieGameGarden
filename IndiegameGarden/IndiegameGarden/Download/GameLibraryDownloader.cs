// (c) 2010-2013 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using IndiegameGarden.Base;
using IndiegameGarden.Install;

namespace IndiegameGarden.Download
{
    /**
     * A downloader that retrieves a game library (consisting of one or more files in a zip)
     */
    public class GameLibraryDownloader: GameDownloadAndInstallTask
    {
        public GameLibraryDownloader(int version)
            : base(GardenItem.ConstructGameLibItem(version))
        {
        }
    }
}

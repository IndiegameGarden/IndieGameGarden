// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

namespace IndiegameGarden.Download
{
    /**
     * A ConfigDownloader that retrieves a game library (consisting of one or more files)
     */
    public class GameLibraryDownloader: ConfigDownloader
    {
        const string DEFAULT_GAMELIBCONFIG_FILENAME = "gamelib.json";

        public GameLibraryDownloader()
            : base(DEFAULT_GAMELIBCONFIG_FILENAME)
        {
        }
    }
}

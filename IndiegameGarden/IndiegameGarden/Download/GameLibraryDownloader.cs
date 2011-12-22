// (c) 2010-2011 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Download
{
    public class GameLibraryDownloader: ConfigDownloader
    {
        const string DEFAULT_GAMELIBCONFIG_FILENAME = "gamelib.json";

        public GameLibraryDownloader()
            : base(DEFAULT_GAMELIBCONFIG_FILENAME)
        {
        }
    }
}

// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using MyDownloader.Core;

namespace IndiegameGarden.Download
{
    /**
     * A downloader of configuration files
     */
    public class ConfigDownloader: BaseDownloader
    {
        string filename;

        public ConfigDownloader(string filename)
        {
            this.filename = filename ;
        }

        public override void Start()
        {
            string url = GardenGame.Instance.Config.ConfigFilesServerURL + filename;
            InternalDoDownload(url, filename, GardenGame.Instance.Config.ConfigFilesFolder, true);
        }
    }
}

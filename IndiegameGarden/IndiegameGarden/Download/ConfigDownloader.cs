// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using MyDownloader.Core;
using IndiegameGarden.Base;

namespace IndiegameGarden.Download
{
    /**
     * A downloader of configuration files
     */
    public class ConfigDownloader: BaseDownloader
    {
        GardenConfig config;

        /// <summary>
        /// retrieve the new, downloaded config after this task has completed successfully (Status == ITaskStatus.SUCCESS)
        /// </summary>
        public GardenConfig NewConfig { get { return config; } }

        /// <summary>
        /// start a new config downloader task, based on information in supplied previous config. Typically this new
        /// config will be replaced after this task has completed.
        /// </summary>
        /// <param name="lastConfig">last i.e. previous known config object. Can't be null.</param>
        public ConfigDownloader(GardenConfig lastConfig)
        {
            config = lastConfig ;
        }

        public bool IsDownloadNeeded()
        {
            long timeLastUpdate = 0;
            if (config.HasKey("tmLstUpd"))
                timeLastUpdate = (long)config.GetValue("tmLstUpd");
            long timeCurrent = System.DateTime.Now.Ticks;
            long intervalUpdate = 5 * System.TimeSpan.TicksPerMinute;
            if (config.HasKey("dtChk"))
                intervalUpdate = ((long)config.GetValue("dtChk")) * System.TimeSpan.TicksPerMinute;

            return (timeCurrent > timeLastUpdate + intervalUpdate);
        }

        protected override void StartInternal()
        {
            string filenameOnServer = config.ConfigFilename;
            string filenameLocalSave = "new_" + filenameOnServer;
            string url = config.ConfigFilesServerURL + filenameOnServer + "?auth=" + GardenConfig.IGG_CLIENT_AUTH_KEY + "&clid="+config.GardenID ;
            string downloadedConfigPath = config.ConfigFilesFolder + "\\" + filenameLocalSave;
            
            // delete any leftover local file
            File.Delete(downloadedConfigPath);

            // download config to local
            InternalDoDownload(url, filenameLocalSave, config.ConfigFilesFolder, true);
            if (status == ITaskStatus.FAIL)
                return;

            // config downloaded ok - now replace the original config file by copying
            string gardenConfigPath = config.ConfigFilesFolder + "\\" + config.ConfigFilename;
            if (File.Exists(downloadedConfigPath))
            {
                try
                {
                    File.Delete(gardenConfigPath);
                    File.Copy(downloadedConfigPath, gardenConfigPath);

                    // reload config from the new file
                    config.Reload();

                    // put in the time of download
                    config.PutValue("tmLstUpd", System.DateTime.Now.Ticks);

                    // save the updated config
                    config.Save();
                    config.Reload();

                    // check for config validity
                    if (!config.IsValid())
                    {
                        status = ITaskStatus.FAIL;
                        statusMsg = "Config file is invalid";
                        return;
                    }

                }
                catch (Exception ex)
                {
                    status = ITaskStatus.FAIL;
                    statusMsg = ex.ToString();
                    return;
                }
            }
            else
            {
                status = ITaskStatus.FAIL;
                return;
            }


            status = ITaskStatus.SUCCESS;
        }
    }
}

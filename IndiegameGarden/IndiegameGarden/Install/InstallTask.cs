// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IndiegameGarden.Base;
using IndiegameGarden.Download;
using IndiegameGarden.Unpack;

namespace IndiegameGarden.Install
{
    /// <summary>
    /// a task to install a game or program from a compressed file format
    /// </summary>
    public class InstallTask: Task
    {
        IndieGame game;
        UnpackerTask unpacker;

        public InstallTask(IndieGame game)
        {
            this.game = game;
        }

        public override void Start()
        {
            status = ITaskStatus.RUNNING;
            string destFolder = GardenGame.Instance.Config.GetGameFolder(game);
            unpacker = new UnpackerTask(GardenGame.Instance.Config.GetPackedFilepath(game), 
                                        destFolder,
                                        game.ExeFile);
            if (File.Exists(unpacker.Filename))
            {                
                unpacker.Start();
                status = unpacker.Status();
                statusMsg = unpacker.StatusMsg();
            }
            else
            {
                status = ITaskStatus.FAIL;
                statusMsg = "Missing file " + unpacker.Filename;
            }
            game.Refresh();
        }

        public override void Abort()
        {
            if (status == ITaskStatus.SUCCESS || status == ITaskStatus.FAIL)
                return;
            if (unpacker != null)
                unpacker.Abort();
            status = ITaskStatus.FAIL;
        }

        public override double Progress()
        {
            if (status == ITaskStatus.CREATED)
                return 0;
            if (status == ITaskStatus.SUCCESS || status== ITaskStatus.FAIL)
                return 1;
            if (unpacker == null)
                return 0;
            return unpacker.Progress();
        }

    }
}

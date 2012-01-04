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
            status = ITaskStatus.STARTED;
            string destFolder = GardenGame.Instance.Config.GetGameFolder(game.GameID, game.Version);
            unpacker = new UnpackerTask(GardenGame.Instance.Config.GetPackedFilepath(game.PackedFileName), 
                                        destFolder );
            Thread t = new Thread(new ThreadStart(StartInstallingThread));
            t.Start();

        }

        // entry point for installation thread
        void StartInstallingThread()
        {
            if (File.Exists(unpacker.Filename))
            {                
                unpacker.Start();
                status = unpacker.Status();
                errorMsg = unpacker.ErrorMessage;
            }
            else
            {
                status = ITaskStatus.FAILED;
                // error, file not there!
                throw new NotImplementedException("file missing handler");
            }
            game.Refresh();
        }

        public override void Abort()
        {
            status = ITaskStatus.FAILED;
            throw new NotImplementedException("Abort() not impl in InstallTask()");
        }

        public override double Progress()
        {
            // TODO more fine grained info
            if (status == ITaskStatus.IDLE)
                return 0;
            if (status == ITaskStatus.FINISHED)
                return 1;
            if (status == ITaskStatus.FAILED)
                return 1;
            if (unpacker == null)
                return 0;
            return unpacker.Progress();
        }

    }
}

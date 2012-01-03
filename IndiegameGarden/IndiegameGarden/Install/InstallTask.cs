// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IndiegameGarden.Store;
using IndiegameGarden.Download;
using IndiegameGarden.Unpack;

namespace IndiegameGarden.Install
{
    /// <summary>
    /// a task to install a game or program from a compressed file format
    /// </summary>
    public class InstallTask: ITask
    {
        bool isStarted = false;
        bool isDone = false;
        IndieGame game;
        UnpackerTask unpacker;

        public InstallTask(IndieGame game)
        {
            this.game = game;
        }

        public void Start()
        {
            string destFolder = GardenGame.Instance.Config.CreateGameFolder(game.GameID);
            unpacker = new UnpackerTask(GardenGame.Instance.Config.CreatePackedFilepath(game.PackedFileName), 
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
            }
            else
            {
                // error, file not there!
                throw new NotImplementedException("file missing handler");
            }
            isDone = true;
        }

        public void Abort()
        {
            throw new NotImplementedException("Abort() not impl in InstallTask()");
        }

        public double Progress()
        {
            // TODO more fine grained info
            if (isDone)
                return 1;
            return 0;
        }

        public bool IsStarted()
        {
            return isStarted;
        }

        public bool IsFinished()
        {
            return isDone;
        }
    }
}

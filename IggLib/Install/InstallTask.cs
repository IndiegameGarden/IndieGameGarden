// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IggLib.Base;
using IggLib.Download;
using IggLib.Unpack;

namespace IggLib.Install
{
    /// <summary>
    /// a task to install a game, program or content item typically from a compressed file format.
    /// If there's only one file to install (which is equal to the downloaded file) then the
    /// decompression step is replaced by mere copying.
    /// </summary>
    public class InstallTask: Task
    {
        GardenItem game;
        UnpackerTask unpacker;

        public InstallTask(GardenItem game)
        {
            this.game = game;
        }

        protected override void StartInternal()
        {
            string destFolder = game.GameFolder;
            if (Directory.Exists(destFolder)) // assume it's already there
            {
                status = ITaskStatus.SUCCESS;
            }
            else
            {
                unpacker = new UnpackerTask(GardenConfig.Instance.GetPackedFilepath(game),
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
            }
            game.Refresh();
        }

        protected override void AbortInternal()
        {
            if (unpacker != null)
                unpacker.Abort();
        }

        public override double Progress()
        {
            if (!IsRunning())
                return 0;
            if (IsFinished())
                return 1;
            if (unpacker == null)
                return 0;
            else
                return unpacker.Progress();
        }

    }
}

// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Unpack
{    
    public class PackedFile
    {
        private enum PackedFileType { RAR, ZIP, UNKNOWN } ;
        string filename = null;
        PackedFileType filetype;


        public PackedFile(string filename)
        {
            this.filename = filename;
            DetectFileType();
        }

        private void DetectFileType()
        {
            if (filename.EndsWith(".rar"))
                filetype = PackedFileType.RAR;
            else if (filename.EndsWith(".zip"))
                filetype = PackedFileType.ZIP;
            else
                filetype = PackedFileType.UNKNOWN;
        }

        public void UnpackTo(string destFolder)
        {
            // TODO error handling e.g. incomplete archives, overwriting files etc.
            switch (filetype)
            {
                case PackedFileType.ZIP:
                    Unzip.UnzipFile(filename,destFolder);
                    break;
                case PackedFileType.RAR:
                    Unrar r = new Unrar(filename);
                    r.Open();
                    while(r.ReadHeader())
                    {
                        r.ExtractToDirectory(destFolder);
                    }
                    r.Close();
                    break;
                default:
                    // TODO throw?
                    break;

            }
        }
    }
}

// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;

namespace IndiegameGardenLauncher
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (IGGLauncherApp game = new IGGLauncherApp())
            {
                game.Run();
            }
        }
    }
#endif
}


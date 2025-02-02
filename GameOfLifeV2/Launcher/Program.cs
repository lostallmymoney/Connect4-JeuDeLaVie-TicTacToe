﻿using JeuDeLaVie;
using System.Diagnostics;

namespace D22
{

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary
        [STAThread]
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.RealTime;

            if (args.GetLength(0) > 0)
            {
                Console.WriteLine("Hi");
                Console.ReadLine();
                Console.ReadLine();
            }
            else
            {
                Game1? game = null;
                try
                {
                    Environment.SetEnvironmentVariable("FNA_AUDIO_DISABLE_SOUND", "1");
                    game = new Game1
                        ();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exeption 2.");
                    Console.WriteLine(e);
                }

                if (game != null)
                {
                    try
                    {
                        game.Run();
                    }
                    finally
                    {
                        game.Dispose();
                    }
                }
            }
        }
    }
}

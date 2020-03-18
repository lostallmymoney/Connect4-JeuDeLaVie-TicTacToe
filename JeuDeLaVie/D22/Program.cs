using System;

namespace D22
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary
        [STAThread]
        static void Main(string[] args)
        {
            Game1 game = null;
            try
            {
                Environment.SetEnvironmentVariable("FNA_AUDIO_DISABLE_SOUND", "1");
                game = new Game1();
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

using JeuDeLaVie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;

namespace JeuDeLaVieConsole
{
    static class Imports
    {
        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }
            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
    
        public static IntPtr HWND_BOTTOM = (IntPtr)1;
        public static IntPtr HWND_TOP = (IntPtr)0;

        public static uint SWP_NOSIZE = 1;
        public static uint SWP_NOZORDER = 4;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);
    }
    static class DisableConsoleQuickEdit
    {

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal static bool Go()
        {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }
    }

    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            DisableConsoleQuickEdit.Go();
            bool affichageChangement = true, recommencer, autoDefaults = false, RTPressed = false, autoRepeat = true, staleProof = false, staleByPass = false, skipToNext = false;
            string input;
            char keyInput;
            double probabilite = 0.1;
            int tailleX = 236, tailleY = 62, msDelay = 0, nbCyclesScan = 20, autoRepeatDelay=0;
            var consoleWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            Imports.SetWindowPos(consoleWnd, 0, 0, 0, 0, 0, Imports.SWP_NOSIZE | Imports.SWP_NOZORDER);
            JeuDeLaVieTable leJeu = JeuDeLaVieTable.Instance;

            //todo : manage when entered input is negative then empty (bypasses the current while test)
            do
            {
                Console.CursorVisible = false;
                Console.Clear();
                input = "";
                if (!autoDefaults) { 
                    Console.Write("Affichage des naissances ou morts ? true or false (ou enter pour tous les defauts) (default true) : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && !bool.TryParse(input, out affichageChangement));
                }
                if (!autoDefaults && input != "")
                {
                    Console.Write("Stale proof ? true or false (default false) : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && !bool.TryParse(input, out staleProof));

                    Console.Write("Auto repeat ? true or false (default true) : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && !bool.TryParse(input, out autoRepeat));

                    if (autoRepeat)
                    {
                        Console.Write("Auto repeat delay ? [0, inf[ en int (default 0) : ");
                        do
                        {
                            input = Console.ReadLine();
                        } while ((input != "" || autoRepeatDelay < 0) && (!int.TryParse(input, out autoRepeatDelay) || autoRepeatDelay < 0));
                    }

                    Console.Write("Probabilite de depart ? ]0, 1[ en double (default 0.1) : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && (!double.TryParse(input, out probabilite) || probabilite >= 1 || probabilite <= 0));

                    Console.Write("Taille X (largeur) ? ]0, 236] en int (default 238) : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && (!int.TryParse(input, out tailleX) || tailleX > 236 || tailleX <= 0));

                    Console.Write("Taille Y (largeur) ? ]0, 62] en int (default 62) : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && (!int.TryParse(input, out tailleY) || tailleY > 62 || tailleY <= 0));

                    Console.Write("Delay par frame en millisecondes en int (default 0) [0, infini[ : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && (!int.TryParse(input, out msDelay) || msDelay < 0));

                    Console.Write("Nombre de cycles a scanner pour la repetition en int (default 20) [1, infini[ : ");
                    do
                    {
                        input = Console.ReadLine();
                    } while (input != "" && (!int.TryParse(input, out nbCyclesScan) || nbCyclesScan < 1));
                }
                leJeu.GenerateNew(
                    cycleMemory: nbCyclesScan, affichageChangement: affichageChangement,
                    probabilite: probabilite, tailleX: tailleX, tailleY: tailleY, 
                    staleProof: staleProof);


                Console.SetWindowSize(tailleX + 1, tailleY + 1);
                string affichageBuffer;
                do
                {
                    staleByPass = false;
                    Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                    while ((staleProof || !leJeu.Stale) && !RTPressed && !skipToNext)
                    {
                        affichageBuffer = leJeu.ToString();
                        Console.SetCursorPosition(0, 0);
                        Console.Write(affichageBuffer);
                        Console.Write("Press R + T to stop, F + G to change wait time, U + I to skip : ");
                        leJeu.CalculerCycle();
                        for (int e = 0; e <= msDelay && !RTPressed; e++)
                        {
                            if (e < msDelay)
                            {
                                Thread.Sleep(1);
                            }
                            if (e % 20 == 0 && Imports.ApplicationIsActivated())
                            {
                                if (Keyboard.IsKeyDown(Key.T) && Keyboard.IsKeyDown(Key.R))
                                {
                                    RTPressed = true;
                                }else if (Keyboard.IsKeyDown(Key.F) && Keyboard.IsKeyDown(Key.G))
                                {
                                    do
                                    {
                                        input = Console.ReadLine().Replace("f", "").Replace("g", "").Replace("u", "").Replace("i", "").Replace("r", "").Replace("t", "");
                                    } while (!int.TryParse(input, out msDelay) || msDelay < 0);
                                    Console.Clear();
                                    Console.Write(affichageBuffer);
                                }else if (Keyboard.IsKeyDown(Key.U) && Keyboard.IsKeyDown(Key.I))
                                {
                                    autoRepeat = true;
                                    skipToNext = true;
                                }
                            }
                        }
                    }
                    
                    if (!autoRepeat || RTPressed) {
                        do
                        {
                            Console.CursorTop--;
                            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                            Console.Write("FINIT APRES UNE LOOP DE " + leJeu.StaleCycle + " CYCLES ! RECOMMENCER ? Y / N (s pour les memes valeurs, j pour staleproof, k pour desactiver (staleproof : " + (staleProof ? "ON" : "OFF") + ")) : ");
                            keyInput = Console.ReadKey(true).KeyChar;
                            if(keyInput == 'k')
                            {
                                staleProof = false;
                                leJeu.StaleProof = false;
                            }
                        } while (keyInput != 'y' && keyInput != 'n' && keyInput != 's' && keyInput != 'j' && keyInput != 'p');
                        recommencer = keyInput != 'n';
                        autoDefaults = keyInput == 's';
                        staleByPass = keyInput == 'p';
                        if(keyInput == 'j')
                        {
                            staleProof = true;
                            leJeu.StaleProof = true;
                            staleByPass = true;
                        }
                    } else if (skipToNext)
                    {
                        skipToNext = false;
                        autoDefaults = true;
                        recommencer = true;
                    } else
                    {
                        recommencer = true;
                        autoDefaults = true;
                        Console.CursorTop--;
                        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                        Console.Write("FINIT APRES UNE LOOP DE " + leJeu.StaleCycle + " CYCLES ! RECOMMENCER ? Y / N (s pour les memes valeurs, j pour staleproof, k pour desactiver (staleproof : " + (staleProof ? "ON" : "OFF") + ")) AUTO RESTARTING IN " + autoRepeatDelay +" ms : ");
                        Console.Write("AUTO RESTARTING IN " + autoRepeatDelay +" ms");
                        Thread.Sleep(autoRepeatDelay);
                    }
                    RTPressed = false;
                } while (staleByPass);
            } while (recommencer);
        }
    }
}

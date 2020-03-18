using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Threading;

namespace JeuDeLaVie
{
    public class JeuDeLaVieTable
    {
        private static bool stale = false;
        private static int cycleSummary = 0;
        private static int nbCells;
        private static bool[,,] tableauDeLaVie;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static bool _affichageChangement, staleProof = false;
        private static int _tailleX, _tailleY, _cycleMemory, staledAtCycle;
        private static char symboleVie = 'o', symboleMourant = 'x', symboleNaissant = '.', symboleMort = ' ';
        private static Random generateur;
        private static Thread staleThread, thread1, thread2, thread3;

        public static char SymboleVie { get => symboleVie; set => symboleVie = value; }
        public static char SymboleMort { get => symboleMort; set => symboleMort = value; }
        public static char SymboleMourant { get => symboleMourant; set => symboleVie = value; }
        public static char SymboleNaissant { get => symboleNaissant; set => symboleMort = value; }
        public static bool Stale { get => stale; }
        public static bool[,,] TableauDeLaVie { get => tableauDeLaVie; }
        public static bool StaleProof { set => staleProof = value; }
        public static int StaleCycle { get => staledAtCycle; }

        //1 constructeur, toutes les façons de l'appeler sont bonne
        static JeuDeLaVieTable()
        {
            generateur = new Random();
        }

        public static void GenerateNew(int cycleMemory = 6, bool affichageChangement = true, double probabilite = 0.06, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            _cycleMemory = cycleMemory + 2;
            ArrayGPS.cycleReset(cycleMemory + 2);
            _affichageChangement = affichageChangement;
            tableauDeLaVie = new bool[tailleX, tailleY, cycleMemory + 2];
            cycleSummaries = new int[cycleMemory + 2];
            cycleRowSummaries = new int[cycleMemory + 2, tailleY];
            _tailleX = tailleX;
            _tailleY = tailleY;

            while (staleThread != null && staleThread.IsAlive)
            {
                Thread.Sleep(10);
            }
            stale = false;

            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNewB()] = (generateur.NextDouble() <= probabilite);
                }
            }
            //crée une copie initiale pour pas que l'affichage initial crée un erreur
            ArrayGPS.BackupTablesNumbers();
        }

        public static void staleTestThreadF()
        {
            for (int o = 0; !staleProof && o < _cycleMemory; o++)
            {
                if (o != ArrayGPS.GetSwapTablesNewB() && o != ArrayGPS.cycleEmulateNew())
                {
                    //if there's a match, looks deeper into it
                    if (cycleSummaries[o] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()])
                    {
                        bool cancelledLookup = false;

                        //look every 6th
                        for (int i = 0; !cancelledLookup && i < _tailleY; i+=6)
                        {
                            if (cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i])
                            {
                                cancelledLookup = true;
                            }
                        }

                        //look every 6th #2
                        for (int i = 3; !cancelledLookup && i < _tailleY; i += 6)
                        {
                            if (cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i])
                            {
                                cancelledLookup = true;
                            }
                        }

                        //look every other
                        for (int i = 1; !cancelledLookup && i < _tailleY; i+=2)
                        {
                            if (cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i])
                            {
                                cancelledLookup = true;
                            }
                            i++;
                            if (i < _tailleY && cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i])
                            {
                                cancelledLookup = true;
                            }
                        }

                        for (int y = 0; !cancelledLookup && y < _tailleY; y++)
                        {
                            for (int x = 0; x < _tailleX && !cancelledLookup; x++)
                            {
                                if (tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNewB()] != tableauDeLaVie[x, y, o])
                                {
                                    cancelledLookup = true;
                                }
                            }
                        }

                        if (!cancelledLookup)
                        {
                            stale = true;
                            staledAtCycle = 0;
                            for (int oToNew = o; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                staledAtCycle++;
                            }
                        }
                    }
                }
            }
        }


        private static bool TestCell(int x, int y, bool yMax, bool xMax, bool yZero, bool xZero)
        {
            int cellSummary;
            cellSummary = 0;
            //teste si c'est sur le cote et si non regarde si il y a une cellule en vie
            if (!yZero)
            {
                if (tableauDeLaVie[x, y - 1, ArrayGPS.GetSwapTablesOld()])
                {
                    cellSummary++;
                }

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else//infinitemap
                {
                    if (tableauDeLaVie[_tailleX - 1, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else//infinitemap
                {
                    if (tableauDeLaVie[0, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }
            else//infinitemap
            {
                if (tableauDeLaVie[x, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[_tailleX - 1, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[0, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }

            if (!yMax)
            {
                if (tableauDeLaVie[x, y + 1, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[_tailleX - 1, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[0, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }
            else//infinitemap
            {
                if (tableauDeLaVie[x, 0, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[_tailleX - 1, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[0, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }

            if (!xZero)
            {
                if (tableauDeLaVie[x - 1, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }
            else
            {
                if (tableauDeLaVie[_tailleX - 1, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }

            if (!xMax)
            {
                if (tableauDeLaVie[x + 1, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }
            else
            {
                if (tableauDeLaVie[0, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }

            //choice
            if (cellSummary < 2)
            {
                tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
            }
            else if (cellSummary == 3)
            {
                tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
            }
            else if (cellSummary >= 4)
            {
                tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
            }
            else if (cellSummary == 2)
            {
                tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()];
            }

            return tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()];
        }

        public static void CalculerCycle()
        {
            //thread calcule old
            ArrayGPS.BackupTablesNumbers();

            staleThread = new Thread(staleTestThreadF);
            staleThread.Priority = ThreadPriority.Highest;
            staleThread.Start();

            //translate les tables utilises vers le haut
            ArrayGPS.cycleAdd();


            nbCells = _tailleX * _tailleY;
            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 2 thread
            thread1 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = 0; y < _tailleY / 3; y++)
                {
                    int cycleXRowSummary = 0;
                    yMax = (y == _tailleY - 1);
                    yZero = (y == 0);
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, yMax, (x == _tailleX - 1), yZero, (x == 0)))
                        {
                            cycleSummary++;
                            cycleXRowSummary++;
                        }
                    }
                    cycleRowSummaries[ArrayGPS.GetSwapTablesNew(), y] = cycleXRowSummary;
                }
            });
            thread1.Priority = ThreadPriority.Highest;
            thread1.Start();

            thread2 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = _tailleY / 3; y < (_tailleY / 3) * 2; y++)
                {
                    int cycleXRowSummary = 0;
                    yMax = (y == _tailleY - 1);
                    yZero = (y == 0);
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, yMax, (x == _tailleX - 1), yZero, (x == 0)))
                        {
                            cycleSummary++;
                            cycleXRowSummary++;
                        }
                    }
                    cycleRowSummaries[ArrayGPS.GetSwapTablesNew(), y] = cycleXRowSummary;
                }
            });
            thread2.Priority = ThreadPriority.Highest;
            thread2.Start();

            thread3 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = (_tailleY / 3) * 2; y < _tailleY; y++)
                {
                    int cycleXRowSummary = 0;
                    yMax = (y == _tailleY - 1);
                    yZero = (y == 0);
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, yMax, (x == _tailleX - 1), yZero, (x == 0)))
                        {
                            cycleSummary++;
                            cycleXRowSummary++;
                        }
                    }
                    cycleRowSummaries[ArrayGPS.GetSwapTablesNew(), y] = cycleXRowSummary;
                }
            });
            thread3.Priority = ThreadPriority.Highest;
            thread3.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();

            //look up the cycle summaries in case there's a match
            cycleSummaries[ArrayGPS.GetSwapTablesNew()] = cycleSummary;
            staleThread.Join();
        }
        public override string ToString()
        {
            StringBuilder affichageTotal = new StringBuilder();
            if (_affichageChangement)
            {
                //affichage avec le changement
                for (int y = 0; y < _tailleY; y++)
                {
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        {
                            if (tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                            {
                                affichageTotal.Append(symboleVie);
                            }
                            else
                            {
                                affichageTotal.Append(symboleMourant);
                            }
                        }
                        else
                        {
                            if (tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                            {
                                affichageTotal.Append(symboleNaissant);
                            }
                            else
                            {
                                affichageTotal.Append(symboleMort);
                            }
                        }
                    }
                    affichageTotal.Append(Environment.NewLine);
                }
            }
            else
            {
                //affichange sans le changement
                for (int y = 0; y < _tailleY; y++)
                {
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (tableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                        {
                            affichageTotal.Append(symboleVie);
                        }
                        else
                        {
                            affichageTotal.Append(symboleMort);
                        }
                    }
                    affichageTotal.Append(Environment.NewLine);
                }
            }
            return affichageTotal.ToString();
        }
    }
}

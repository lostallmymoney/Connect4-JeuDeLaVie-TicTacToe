using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Threading;

namespace JeuDeLaVie
{
    public class JeuDeLaVieTable
    {
        private static int _tailleX, _tailleY, _cycleMemory, cycleSummary = 0;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static Random generateur;
        private static Thread staleThread, thread1, thread2, thread3, thread4;

        public static Color[] DonneeTables { get; private set; }
        public static char SymboleVie { get; set; } = 'o';
        public static char SymboleMort { get; set; } = ' ';
        public static char SymboleMourant { get; set; } = 'x';
        public static char SymboleNaissant { get; set; } = '.';
        public static bool Stale { get; private set; } = false;
        public static bool[,,] TableauDeLaVie { get; private set; }
        public static bool StaleProof { get; set; } = false;
        public static int StaleCycle { get; private set; }
        public static bool AffichageChangement { get; set; }

        static JeuDeLaVieTable()
        {
            generateur = new Random();
        }

        public static void GenerateNew(int cycleMemory = 6, bool affichageChangement = false, double probabilite = 0.05, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            _cycleMemory = cycleMemory + 2;
            ArrayGPS.CycleReset(cycleMemory + 2);
            AffichageChangement = affichageChangement;
            TableauDeLaVie = new bool[tailleX, tailleY, cycleMemory + 2];
            cycleSummaries = new int[cycleMemory + 2];
            cycleRowSummaries = new int[cycleMemory + 2, tailleY];
            _tailleX = tailleX;
            _tailleY = tailleY;

            while (staleThread != null && staleThread.IsAlive)
            {
                Thread.Sleep(10);
            }
            Stale = false;

            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = (generateur.NextDouble() <= probabilite);
                }
            }
            //crée une copie initiale pour pas que l'affichage initial crée un erreur
            ArrayGPS.BackupTablesNumbers();
        }

        public static void GenerateInitialImg()
        {
            DonneeTables = new Color[_tailleX * _tailleY];
            for(int y = 0; y < _tailleY; y++)
            {
                for(int x=0; x < _tailleX; x++)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                        DonneeTables[y * _tailleX + x] = Color.Black;
                }
            }
        }

        public static void StaleTestThreadF()
        {
            for (int o = 0; !StaleProof && o < _cycleMemory; o++)
            {
                if (o != ArrayGPS.GetSwapTablesNewB() && o != ArrayGPS.CycleEmulateNew())
                {
                    //if there's a match, looks deeper into it
                    if (cycleSummaries[o] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()])
                    {
                        bool cancelledLookup = false;

                        //look every 6th
                        for (int i = 0; !cancelledLookup && i < _tailleY; i += 6)
                        {
                            cancelledLookup |= cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i];
                        }

                        //look every 6th #2
                        for (int i = 3; !cancelledLookup && i < _tailleY; i += 6)
                        {
                            cancelledLookup |= cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i];
                        }

                        //look every other
                        for (int i = 1; !cancelledLookup && i < _tailleY; i += 2)
                        {
                            cancelledLookup |= cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i];
                            i++;
                            cancelledLookup |= (i < _tailleY && cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i]);
                        }

                        for (int y = 0; !cancelledLookup && y < _tailleY; y++)
                        {
                            for (int x = 0; x < _tailleX && !cancelledLookup; x++)
                            {
                                cancelledLookup |= TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNewB()] != TableauDeLaVie[x, y, o];
                            }
                        }

                        if (!cancelledLookup)
                        {
                            Stale = true;
                            StaleCycle = 0;
                            for (int oToNew = o; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                StaleCycle++;
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
                if (TableauDeLaVie[x, y - 1, ArrayGPS.GetSwapTablesOld()])
                {
                    cellSummary++;
                }

                if (!xZero)
                {
                    if (TableauDeLaVie[x - 1, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else//infinitemap
                {
                    if (TableauDeLaVie[_tailleX - 1, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (TableauDeLaVie[x + 1, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else//infinitemap
                {
                    if (TableauDeLaVie[0, y - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }
            else//infinitemap
            {
                if (TableauDeLaVie[x, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (TableauDeLaVie[x - 1, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (TableauDeLaVie[_tailleX - 1, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (TableauDeLaVie[x + 1, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (TableauDeLaVie[0, _tailleY - 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }

            if (!yMax)
            {
                if (TableauDeLaVie[x, y + 1, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (TableauDeLaVie[x - 1, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (TableauDeLaVie[_tailleX - 1, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (TableauDeLaVie[x + 1, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (TableauDeLaVie[0, y + 1, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }
            else//infinitemap
            {
                if (TableauDeLaVie[x, 0, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (TableauDeLaVie[x - 1, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (TableauDeLaVie[_tailleX - 1, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (TableauDeLaVie[x + 1, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (TableauDeLaVie[0, 0, ArrayGPS.GetSwapTablesOld()])
                        cellSummary++;
                }
            }

            if (!xZero)
            {
                if (TableauDeLaVie[x - 1, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }
            else
            {
                if (TableauDeLaVie[_tailleX - 1, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }

            if (!xMax)
            {
                if (TableauDeLaVie[x + 1, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }
            else
            {
                if (TableauDeLaVie[0, y, ArrayGPS.GetSwapTablesOld()])
                    cellSummary++;
            }

            //choice
            if (AffichageChangement)
            {
                if (cellSummary < 2)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[y * _tailleX + x] = Color.DarkRed;
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                }
                else if (cellSummary == 3)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                    if (!TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[y * _tailleX + x] = Color.DarkGreen;
                    else
                        DonneeTables[y * _tailleX + x] = Color.Black;
                }
                else if (cellSummary >= 4)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[y * _tailleX + x] = Color.DarkRed;
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                }
                else if (cellSummary == 2)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                    {
                        TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                        DonneeTables[y * _tailleX + x] = Color.Black;
                    }
                    else
                    {
                        if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                            DonneeTables[y * _tailleX + x] = Color.DarkRed;
                        TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                    }

                }
            }
            else
            {
                if (cellSummary < 2)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                }
                else if (cellSummary == 3)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                    DonneeTables[y * _tailleX + x] = Color.Black;
                }
                else if (cellSummary >= 4)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                }
                else if (cellSummary == 2)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()];
                }
            }

            return TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] == true;
        }

        public static void setLife(int x, int y, bool value = true)
        {
            if(x<0)
                if(y<0)
                    TableauDeLaVie[_tailleX + x, _tailleY + y, ArrayGPS.GetSwapTablesNew()] = value;
                else
                    if(y>=_tailleY)
                        TableauDeLaVie[_tailleX + x, y - _tailleY, ArrayGPS.GetSwapTablesNew()] = value;
                    else
                        TableauDeLaVie[_tailleX + x,y, ArrayGPS.GetSwapTablesNew()] = value;
            else
                if(x>=_tailleX)
                    if (y < 0)
                        TableauDeLaVie[x - _tailleX, _tailleY + y, ArrayGPS.GetSwapTablesNew()] = value;
                     else
                        if (y >= _tailleY)
                            TableauDeLaVie[x - _tailleX, y - _tailleY, ArrayGPS.GetSwapTablesNew()] = value;
                        else
                            TableauDeLaVie[x - _tailleX, y, ArrayGPS.GetSwapTablesNew()] = value;
                else
                    if (y < 0)
                        TableauDeLaVie[x , _tailleY + y, ArrayGPS.GetSwapTablesNew()] = value;
                     else
                        if (y >= _tailleY)
                            TableauDeLaVie[x, y - _tailleY, ArrayGPS.GetSwapTablesNew()] = value;
                        else
                            TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = value;
        }

        public static void CalculerCycle()
        {
            DonneeTables = new Color[_tailleX * _tailleY];
            //thread calcule old
            ArrayGPS.BackupTablesNumbers();

            staleThread = new Thread(StaleTestThreadF)
            {
                Priority = ThreadPriority.Highest
            };
            staleThread.Start();

            //translate les tables utilises vers le haut
            ArrayGPS.CycleAdd();

            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 2 thread
            thread1 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = 0; y < _tailleY / 4; y++)
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
            })
            {
                Priority = ThreadPriority.Highest
            };
            thread1.Start();

            thread2 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = _tailleY / 4; y < (_tailleY / 4) * 2; y++)
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
            })
            {
                Priority = ThreadPriority.Highest
            };
            thread2.Start();

            thread3 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = (_tailleY / 4) *2; y < (_tailleY/4)*3; y++)
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
            })
            {
                Priority = ThreadPriority.Highest
            };
            thread3.Start();

            thread4 = new Thread(() =>
            {
                bool yMax, yZero;
                for (int y = (_tailleY / 4) * 3; y < _tailleY; y++)
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
            })
            {
                Priority = ThreadPriority.Highest
            };
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            //look up the cycle summaries in case there's a match
            cycleSummaries[ArrayGPS.GetSwapTablesNew()] = cycleSummary;
            staleThread.Join();
        }
        public override string ToString()
        {
            StringBuilder affichageTotal = new StringBuilder();
            if (AffichageChangement)
            {
                //affichage avec le changement
                for (int y = 0; y < _tailleY; y++)
                {
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        {
                            if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                            {
                                affichageTotal.Append(SymboleVie);
                            }
                            else
                            {
                                affichageTotal.Append(SymboleMourant);
                            }
                        }
                        else
                        {
                            if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                            {
                                affichageTotal.Append(SymboleNaissant);
                            }
                            else
                            {
                                affichageTotal.Append(SymboleMort);
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
                        if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                        {
                            affichageTotal.Append(SymboleVie);
                        }
                        else
                        {
                            affichageTotal.Append(SymboleMort);
                        }
                    }
                    affichageTotal.Append(Environment.NewLine);
                }
            }
            return affichageTotal.ToString();
        }
    }
}

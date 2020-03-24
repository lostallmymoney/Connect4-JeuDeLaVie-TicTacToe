using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Threading;

namespace JeuDeLaVie
{
    public class JeuDeLaVieTable
    {
        private static int _tailleX, _tailleY, _cycleMemory, _nbCyclesCheckedLive = 0, _cycleStateAncient = 0, cycleSummary = 0, _memoryDistance;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static Random generateur;
        private static Thread thread1, thread2, thread3, thread4, lThread1, lThread2, lThread3, lThread4;
        private static bool structureNature = true;
        private static StructureManager StructureMgr;
        private static int tailleY4, tailleY2, tailleY75, tailleY4X, tailleY2X, tailleY75X;
        private static int cycleMemory4, cycleMemory2, cycleMemory75;
        private static bool[,,] TableauDeLaVie;

        public static Color[] DonneeTables { get; private set; }

        public static char SymboleVie { get; set; } = 'o';
        public static char SymboleMort { get; set; } = ' ';
        public static char SymboleMourant { get; set; } = 'x';
        public static char SymboleNaissant { get; set; } = '.';
        public static bool Stale { get; private set; } = false;
        public static bool StaleProof { get; set; } = false;
        public static int StaleCycle { get; private set; }
        public static bool AffichageChangement { get; set; }

        static JeuDeLaVieTable()
        {
            StructureMgr = new StructureManager();
            generateur = new Random();
        }

        public static void GenerateNew(int memoryDistance = 250, int nbAncientSummaries = 16, bool affichageChangement = false, double probabilite = 0.00002, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            if (nbAncientSummaries < 1)
                nbAncientSummaries = 1;
            _memoryDistance = memoryDistance;
            _cycleMemory = nbAncientSummaries + _nbCyclesCheckedLive + 2;
            ArrayGPS.CycleReset(nbAncientSummaries + _nbCyclesCheckedLive + 2, nbAncientSummaries);
            AffichageChangement = affichageChangement;
            TableauDeLaVie = new bool[tailleX, tailleY, nbAncientSummaries + _nbCyclesCheckedLive + 2];
            cycleSummaries = new int[nbAncientSummaries + _nbCyclesCheckedLive + 2];
            cycleRowSummaries = new int[nbAncientSummaries + _nbCyclesCheckedLive + 2, tailleY];
            _tailleX = tailleX;
            _tailleY = tailleY;
            DonneeTables = new Color[_tailleX * _tailleY];

            //testcell threads maths
            tailleY4 = _tailleY / 4;
            tailleY2 = tailleY4 * 2;
            tailleY75 = tailleY4 * 3;
            tailleY4X = tailleY4 * _tailleX;
            tailleY2X = tailleY2 * _tailleX;
            tailleY75X = tailleY75 * _tailleX;

            //test threads math
            cycleMemory4 = _cycleMemory / 4;
            cycleMemory2 = _cycleMemory / 2;
            cycleMemory75 = cycleMemory4 * 3;

            Stale = false;

            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    if (structureNature)
                    {
                        if (generateur.NextDouble() <= probabilite)
                        {
                            bool r = (generateur.NextDouble() <= 0.5);
                            int direction = generateur.Next(0, 4);
                            int selectedIndex = generateur.Next(0, StructureMgr.StructureTemplatesNature.Count);

                            if (StructureMgr.StructureTemplatesNature[selectedIndex] != null)
                            {
                                for (int f = 0; f < StructureMgr.StructureTemplatesNature[selectedIndex].getHeight(direction); f++)
                                {
                                    for (int g = 0; g < StructureMgr.StructureTemplatesNature[selectedIndex].getWidth(direction); g++)
                                    {
                                        if (StructureMgr.StructureTemplatesNature[selectedIndex].getValue(direction, g, f, r) ?? false)
                                            setLife(g + x - StructureMgr.StructureTemplatesNature[selectedIndex].getWidth(direction) / 2, f + y - StructureMgr.StructureTemplatesNature[selectedIndex].getHeight(direction) / 2);
                                    }
                                }
                            }
                        }
                    }
                    else
                        TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = (generateur.NextDouble() <= probabilite);
                }
            }
            //crée une copie initiale pour pas que l'affichage initial crée un erreur
            ArrayGPS.BackupTablesNumbers();
        }

        //crée une copie initiale pour pas que l'affichage initial crée un erreur
        public static void GenerateInitialImg()
        {
            for (int y = 0, yByXtotal = 0; y < _tailleY; y++, yByXtotal += _tailleX)
            {
                for (int x = 0; x < _tailleX; x++)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                        DonneeTables[yByXtotal + x] = Color.Black;
                }
            }
        }

        public static void SingleTestThread(int oStart, int oEnd)
        {
            for (;oStart < oEnd && !StaleProof; oStart++)
            {
                if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()] && oStart != ArrayGPS.GetSwapTablesNewB() && oStart != ArrayGPS.CycleEmulateNew())
                {
                    //if there's a match, looks deeper into it
                    bool cancelledLookup = false;

                    for (int i = 0; i < _tailleY; i++)
                    {
                        if (cycleRowSummaries[oStart, i] != cycleRowSummaries[ArrayGPS.GetSwapTablesNewB(), i])
                        {
                            cancelledLookup = true;
                            break;
                        }
                    }

                    if (cancelledLookup)
                    {
                        continue;
                    }
                    else
                    {
                        for (int y = 0; y < _tailleY; y++)
                        {
                            for (int x = 0; x < _tailleX; x++)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNewB()] != TableauDeLaVie[x, y, oStart])
                                {
                                    cancelledLookup = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!cancelledLookup)
                    {
                        Stale = true;
                        StaleCycle = 0;
                        for (int oToNew = oStart; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++)
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

        public static void StaleTestThreadF()
        {
            Thread lThread1 = new Thread(() => { SingleTestThread(0, cycleMemory4); })
            {
                Priority = ThreadPriority.Highest
            };
            Thread lThread2 = new Thread(() => { SingleTestThread(cycleMemory4, cycleMemory2); })
            {
                Priority = ThreadPriority.Highest
            };
            Thread lThread3 = new Thread(() => { SingleTestThread(cycleMemory2, cycleMemory75); })
            {
                Priority = ThreadPriority.Highest
            };
            Thread lThread4 = new Thread(() => { SingleTestThread(cycleMemory75, _cycleMemory); })
            {
                Priority = ThreadPriority.Highest
            };
            lThread1.Start();
            lThread2.Start();
            lThread3.Start();
            lThread4.Start();
            lThread1.Join();
            lThread2.Join();
            lThread3.Join();
            lThread4.Join();
        }

        private static bool TestCell(int x, int y, bool yMax, bool xMax, bool yZero, bool xZero, int nbYBackY)
        {

            int cellSummary = 0;
            if (TableauDeLaVie[x, yZero ? _tailleY - 1 : y - 1, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[xZero ? _tailleX - 1 : x - 1, yZero ? _tailleY - 1 : y - 1, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[xMax ? 0 : x + 1, yZero ? _tailleY - 1 : y - 1, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[x, yMax ? 0 : y + 1, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[xZero ? _tailleX - 1 : x - 1, yMax ? 0 : y + 1, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[xMax ? 0 : x + 1, yMax ? 0 : y + 1, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[xZero ? _tailleX - 1 : x - 1, y, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            if (TableauDeLaVie[xMax ? 0 : x + 1, y, ArrayGPS.GetSwapTablesOld()])
                cellSummary++;

            //choice
            if (AffichageChangement)
            {
                //color
                if (cellSummary < 2)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                    else
                        DonneeTables[nbYBackY + x] = Color.Transparent;
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                    return false;
                }
                else if (cellSummary == 3)
                {
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                    if (!TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[nbYBackY + x] = Color.DarkGreen;
                    else
                        DonneeTables[nbYBackY + x] = Color.Black;
                    return true;
                }
                else if (cellSummary >= 4)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                    return false;
                }
                else if (cellSummary == 2)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                    {
                        DonneeTables[nbYBackY + x] = Color.Black;
                        TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                        return true;
                    }
                    else
                    {
                        if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                            DonneeTables[nbYBackY + x] = Color.DarkRed;
                        else
                            DonneeTables[nbYBackY + x] = Color.Transparent;
                        TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                        return false;
                    }
                }
                else
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                    else
                        DonneeTables[nbYBackY + x] = Color.Transparent;
                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                    return false;
                }
            }
            else
            {
                //black
                if (cellSummary == 3)
                {
                    DonneeTables[nbYBackY + x] = Color.Black;
                    return true | (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true);
                }
                else if (cellSummary == 2 && TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                    return true | (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true);

                DonneeTables[nbYBackY + x] = Color.Transparent;
                return false | (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false);
            }
        }

        public static void setLife(int x, int y, bool value = true)
        {
            if (x < 0)
                if (y < 0)
                    TableauDeLaVie[_tailleX + x, _tailleY + y, ArrayGPS.GetSwapTablesNew()] = value;
                else
                    if (y >= _tailleY)
                        TableauDeLaVie[_tailleX + x, y - _tailleY, ArrayGPS.GetSwapTablesNew()] = value;
                    else
                        TableauDeLaVie[_tailleX + x, y, ArrayGPS.GetSwapTablesNew()] = value;
            else
                if (x >= _tailleX)
                    if (y < 0)
                        TableauDeLaVie[x - _tailleX, _tailleY + y, ArrayGPS.GetSwapTablesNew()] = value;
                    else
                        if (y >= _tailleY)
                            TableauDeLaVie[x - _tailleX, y - _tailleY, ArrayGPS.GetSwapTablesNew()] = value;
                        else
                            TableauDeLaVie[x - _tailleX, y, ArrayGPS.GetSwapTablesNew()] = value;
            else
                if (y < 0)
                    TableauDeLaVie[x, _tailleY + y, ArrayGPS.GetSwapTablesNew()] = value;
                else
                    if (y >= _tailleY)
                        TableauDeLaVie[x, y - _tailleY, ArrayGPS.GetSwapTablesNew()] = value;
                    else
                        TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = value;
        }

        public static void CalculerCycle()
        {
            //thread calcule old
            ArrayGPS.BackupTablesNumbers();

            _cycleStateAncient++;
            if (_cycleStateAncient == _memoryDistance)
            {
                ArrayGPS.pushAncient1();
                _cycleStateAncient = 0;
            }

            lThread1 = new Thread(() => { SingleTestThread(0, cycleMemory4); })
            {
                Priority = ThreadPriority.Highest
            };
            lThread2 = new Thread(() => { SingleTestThread(cycleMemory4, cycleMemory2); })
            {
                Priority = ThreadPriority.Highest
            };
            lThread3 = new Thread(() => { SingleTestThread(cycleMemory2, cycleMemory75); })
            {
                Priority = ThreadPriority.Highest
            };
            lThread4 = new Thread(() => { SingleTestThread(cycleMemory75, _cycleMemory); })
            {
                Priority = ThreadPriority.Highest
            };
            lThread1.Start();
            lThread2.Start();
            lThread3.Start();
            lThread4.Start();

            //translate les tables utilises vers le haut
            ArrayGPS.CycleAdd();

            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 4 thread

            thread1 = new Thread(() =>
            {
                for (int y = 0, cycleXRowSummary, nbYBacky = 0; y < tailleY4; y++, nbYBacky += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, y == _tailleY - 1, x == _tailleX - 1, y == 0, x == 0, nbYBacky))
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

            thread2 = new Thread(() =>
            {
                for (int y = tailleY4, cycleXRowSummary, nbYBacky = tailleY4X; y < tailleY2; y++, nbYBacky += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, y == _tailleY - 1, x == _tailleX - 1, y == 0, x == 0, nbYBacky))
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

            thread3 = new Thread(() =>
            {
                for (int y = tailleY2, cycleXRowSummary, nbYBacky = tailleY2X; y < tailleY75; y++, nbYBacky += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, y == _tailleY - 1, x == _tailleX - 1, y == 0, x == 0, nbYBacky))
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

            thread4 = new Thread(() =>
            {
                for (int y = tailleY75, cycleXRowSummary, nbYBacky = tailleY75X; y < _tailleY; y++, nbYBacky += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        if (TestCell(x, y, y == _tailleY - 1, x == _tailleX - 1, y == 0, x == 0, nbYBacky))
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
            thread2.Start();
            thread3.Start();
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            //set the cycle summaries in case there's a match
            cycleSummaries[ArrayGPS.GetSwapTablesNew()] = cycleSummary;

            lThread1.Join();
            lThread2.Join();
            lThread3.Join();
            lThread4.Join();
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

using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JeuDeLaVie
{

    public class JeuDeLaVieTable
    {
        private static int _tailleX, _tailleY, _cycleMemory, _cycleStateAncient = 0, cycleSummary = 0, _memoryDistance, tailleY4, tailleY2, tailleY75, tailleY4X, tailleY2X, tailleY75X, tailleYMinus, tailleXMinus;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static Random generateur;
        private static Thread thread1, thread2, thread3, thread4, lThread1, lThread2, lThread3, lThread4;
        private static bool structureNature = true;
        private static StructureManager StructureMgr;
        private static int cycleMemory4, cycleMemory2, cycleMemory75;
        private static bool[,,] TableauDeLaVie;

        public static Color[] DonneeTables { get; private set; }
        public static bool Stale { get; private set; } = false;
        public static int StaleCycle { get; private set; } = 0;
        public static bool AffichageChangement { get; set; }

        static JeuDeLaVieTable()
        {
            StructureMgr = new StructureManager();
            generateur = new Random();
        }

        public static void GenerateNew(int memoryDistance = 500, int nbAncientSummaries = 16, bool affichageChangement = false, double probabilite = 0.00002, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            if (nbAncientSummaries < 1)
                nbAncientSummaries = 1;
            _memoryDistance = memoryDistance;
            _cycleMemory = nbAncientSummaries + 2;
            ArrayGPS.CycleReset(nbAncientSummaries + 2, nbAncientSummaries);
            AffichageChangement = affichageChangement;
            TableauDeLaVie = new bool[tailleX, tailleY, nbAncientSummaries + 2];
            cycleSummaries = new int[nbAncientSummaries + 2];
            cycleRowSummaries = new int[nbAncientSummaries + 2, tailleY];
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

            tailleYMinus = _tailleY - 1;
            tailleXMinus = _tailleX - 1;

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
            for (int y = 0, yByXtotal = 0; y < _tailleY; y++, yByXtotal += _tailleX)
            {
                for (int x = 0; x < _tailleX; x++)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()])
                        DonneeTables[yByXtotal + x] = Color.Black;
                }
            }
        }

        public static void setLife(int x, int y, bool value = true)
        {
            int rX = x < 0 ? _tailleX + x : (x >= _tailleX ? x - _tailleX : x),
                rY = y < 0 ? _tailleY + y : (y >= _tailleY ? y - _tailleY : y);
            TableauDeLaVie[rX, rY, ArrayGPS.GetSwapTablesNew()] = value;

            if(value)
                DonneeTables[rY * _tailleX + x] = Color.Black;
        }

        public static void CalculerCycle()
        {
            //thread calcule old
            ArrayGPS.BackupTablesNumbers();
            //translate les tables utilises vers le haut
            ArrayGPS.CycleAdd();

            _cycleStateAncient++;
            if (_cycleStateAncient == _memoryDistance)
            {
                ArrayGPS.pushAncient1();
                _cycleStateAncient = 0;
            }

            lThread1 = new Thread(() => {
                for (int oStart = 0; oStart < cycleMemory4; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()] && oStart != ArrayGPS.GetSwapTablesNewB() && oStart != ArrayGPS.GetSwapTablesNew())
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
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++, i++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                i++;
                            }
                            StaleCycle = _memoryDistance * (2 ^ (i - 3)) + _cycleStateAncient;//bad calcul doing later
                        }
                    }
                }
            })
            {
                Priority = ThreadPriority.Highest
            };
            lThread2 = new Thread(() => {
                for (int oStart = cycleMemory4; oStart < cycleMemory2; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()] && oStart != ArrayGPS.GetSwapTablesNewB() && oStart != ArrayGPS.GetSwapTablesNew())
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
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++, i++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                i++;
                            }
                            StaleCycle = _memoryDistance * (2 ^ (i - 3)) + _cycleStateAncient;//bad calcul doing later
                        }
                    }
                }
            })
            {
                Priority = ThreadPriority.Highest
            };
            lThread3 = new Thread(() => {
                for (int oStart = cycleMemory2; oStart < cycleMemory75; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()] && oStart != ArrayGPS.GetSwapTablesNewB() && oStart != ArrayGPS.GetSwapTablesNew())
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
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++, i++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                i++;
                            }
                            StaleCycle = _memoryDistance * (2 ^ (i - 3)) + _cycleStateAncient;//bad calcul doing later
                        }
                    }
                }
            })
            {
                Priority = ThreadPriority.Highest
            };
            lThread4 = new Thread(() => {
                for (int oStart = cycleMemory75; oStart < _cycleMemory; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.GetSwapTablesNewB()] && oStart != ArrayGPS.GetSwapTablesNewB() && oStart != ArrayGPS.GetSwapTablesNew())
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
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.GetSwapTablesNewB(); oToNew++, i++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                i++;
                            }
                            StaleCycle = _memoryDistance * (2 ^ (i - 3)) + _cycleStateAncient;//bad calcul doing later
                        }
                    }
                }
            })
            {
                Priority = ThreadPriority.Highest
            };
            lThread1.Start();
            lThread2.Start();
            lThread3.Start();
            lThread4.Start();

            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 4 thread

            thread1 = new Thread(() =>
            {
                for (int y = 0, cycleXRowSummary, nbYBackY = 0; y < tailleY4; y++, nbYBackY += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int cellSummary = 0, yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;
                        //en bas
                        if (TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut
                        if (TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.GetSwapTablesOld()])
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
                            }
                            else if (cellSummary == 3)
                            {
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                if (!TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkGreen;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                cycleSummary++;
                                cycleXRowSummary++;
                            }
                            else if (cellSummary >= 4)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                            else if (cellSummary == 2)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                                    else
                                        DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                        }
                        else
                        {
                            //black
                            if (cellSummary == 3)
                            {
                                cycleSummary++;
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                            {
                                if (cellSummary == 2) { 
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
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
                for (int y = tailleY4, cycleXRowSummary, nbYBackY = tailleY4X; y < tailleY2; y++, nbYBackY += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int cellSummary = 0, yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;
                        //en bas
                        if (TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut
                        if (TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.GetSwapTablesOld()])
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
                            }
                            else if (cellSummary == 3)
                            {
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                if (!TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkGreen;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                cycleSummary++;
                                cycleXRowSummary++;
                            }
                            else if (cellSummary >= 4)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                            else if (cellSummary == 2)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                                    else
                                        DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                        }
                        else
                        {
                            //black
                            if (cellSummary == 3)
                            {
                                cycleSummary++;
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                            {
                                if (cellSummary == 2)
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
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
                for (int y = tailleY2, cycleXRowSummary, nbYBackY = tailleY2X; y < tailleY75; y++, nbYBackY += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int cellSummary = 0, yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;
                        //en bas
                        if (TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut
                        if (TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.GetSwapTablesOld()])
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
                            }
                            else if (cellSummary == 3)
                            {
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                if (!TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkGreen;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                cycleSummary++;
                                cycleXRowSummary++;
                            }
                            else if (cellSummary >= 4)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                            else if (cellSummary == 2)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                                    else
                                        DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                        }
                        else
                        {
                            //black
                            if (cellSummary == 3)
                            {
                                cycleSummary++;
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                            {
                                if (cellSummary == 2)
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
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
                for (int y = tailleY75, cycleXRowSummary, nbYBackY = tailleY75X; y < _tailleY; y++, nbYBackY += _tailleX)
                {
                    cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int cellSummary = 0, yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;

                        //en bas
                        if (TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut
                        if (TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a gauche
                        if (TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en bas a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //en haut a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.GetSwapTablesOld()])
                            cellSummary++;
                        //a droite
                        if (TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.GetSwapTablesOld()])
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
                            }
                            else if (cellSummary == 3)
                            {
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                if (!TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkGreen;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                cycleSummary++;
                                cycleXRowSummary++;
                            }
                            else if (cellSummary >= 4)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                            else if (cellSummary == 2)
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                        DonneeTables[nbYBackY + x] = Color.DarkRed;
                                    else
                                        DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                            {
                                if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                                    DonneeTables[nbYBackY + x] = Color.DarkRed;
                                else
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                            }
                        }
                        else
                        {
                            //black
                            if (cellSummary == 3)
                            {
                                cycleSummary++;
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesOld()])
                            {
                                if (cellSummary == 2)
                                {
                                    cycleSummary++;
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = true;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
                                }
                            }
                            else
                                TableauDeLaVie[x, y, ArrayGPS.GetSwapTablesNew()] = false;
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
    }
}

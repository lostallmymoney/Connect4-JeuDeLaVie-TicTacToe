using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Threading;

namespace JeuDeLaVie
{
    public class JeuDeLaVieTable
    {
        private static int _tailleX, _tailleY, _cycleMemory, _cycleStateAncient = 0, cycleSummary = 0, _memoryDistance, tailleY4, tailleY2, tailleY75, tailleY4X, tailleY2X, tailleY75X, tailleYMinus, tailleXMinus;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static Random generateur;
        private static Thread thread1, thread2, thread3, thread4;
        private static bool structureNature = true;
        private static StructureManager StructureMgr;
        private static int cycleMemory25, cycleMemory50, cycleMemory75;
        private static byte[,,] TableauDeLaVie;
        private static int[,,] cycleMapTable;
        //cmaptable MAP
        //1 == TOP LEFT corner X
        //2 == TOP LEFT corner Y
        //3 == TOP X
        //4 == TOP Y
        //5 == TOP RIGHT X
        //6 == TOP RIGHT Y
        //7 == LEFT X
        //8 == LEFT Y
        //9 == RIGHT X
        //10 == RIGHT Y
        //11 == BOT LEFT X
        //12 == BOT LEFT Y
        //13 == BOT X
        //14 == BOT Y
        //15 == BOT RIGHT X
        //16 == BOT RIGHT Y
        //17 EXTRA DATA
        //18 EXTRA DATA

        public static Color[] DonneeTables { get; private set; }
        public static bool Stale { get; private set; } = false;
        public static int StaleCycle { get; private set; } = 0;
        public static bool AffichageChangement { get; set; }

        static JeuDeLaVieTable()
        {
            StructureMgr = new StructureManager();
            generateur = new Random();
        }

        public static void GenerateNew(GraphicsDevice graphicDevice, int memoryDistance = 1000, int nbAncientSummaries = 9, bool affichageChangement = false, double probabilite = 0.02, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            if (nbAncientSummaries < 1)
                nbAncientSummaries = 1;
            _memoryDistance = memoryDistance;
            _cycleMemory = nbAncientSummaries + 2;
            ArrayGPS.CycleReset(nbAncientSummaries + 2, nbAncientSummaries);
            AffichageChangement = affichageChangement;
            TableauDeLaVie = new byte[tailleX, tailleY, nbAncientSummaries + 2];
            cycleMapTable = new int[tailleX, tailleY, 18];
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
            cycleMemory25 = _cycleMemory / 4;
            cycleMemory50 = _cycleMemory / 2;
            cycleMemory75 = cycleMemory25 * 3;

            Stale = false;

            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {

                    bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                    int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;

                    //cellSummary = TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                        //TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                        //TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                        //TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld], ArrayGPS.SwapTablesOld] +
                        //TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                        //TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld];// +
                        //TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.SwapTablesOld];// +
                        //TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.SwapTablesOld];

                    cycleMapTable[x, y, 0] = xZero ? tailleXMinus : xMinus;
                    cycleMapTable[x, y, 1] = yZero ? tailleYMinus : yMinus;

                    cycleMapTable[x, y, 2] = x; //useless
                    cycleMapTable[x, y, 3] = yMax ? 0 : yPlus;

                    cycleMapTable[x, y, 4] = xMax ? 0 : xPlus;
                    cycleMapTable[x, y, 5] = yZero ? tailleYMinus : yMinus;

                    cycleMapTable[x, y, 6] = xZero ? tailleXMinus : xMinus;
                    cycleMapTable[x, y, 7] = y; //useless

                    cycleMapTable[x, y, 8] = xZero ? tailleXMinus : xMinus;
                    cycleMapTable[x, y, 9] = y; //useless

                    cycleMapTable[x, y, 10] = xZero ? tailleXMinus : xMinus;
                    cycleMapTable[x, y, 11] = yMax ? 0 : yPlus;

                    cycleMapTable[x, y, 12] = x; //useless
                    cycleMapTable[x, y, 13] = yMax ? 0 : yPlus;

                    cycleMapTable[x, y, 14] = xMax ? 0 : xPlus;
                    cycleMapTable[x, y, 15] = yMax ? 0 : yPlus;

                    cycleMapTable[x, y, 16] = 1;
                    cycleMapTable[x, y, 17] = 1;

                    if (structureNature)
                    {
                        bool r = (generateur.NextDouble() <= 0.5);
                        int direction = generateur.Next(0, 4);
                        double selectedIndex = ((double)generateur.Next(0,100000))/100000, templatesSum = StructureMgr.StructureTemplatesNature.Sum(i => i.percentageChance);

                        if (selectedIndex < templatesSum)
                        {
                            double summaryT = 0;
                            StructureTemplateNature i=null;

                            for (int g=0; i == null && g < StructureMgr.StructureTemplatesNature.Count; g++)
                            {
                                summaryT += StructureMgr.StructureTemplatesNature[g].percentageChance;
                                if (summaryT > selectedIndex)
                                {
                                    i = StructureMgr.StructureTemplatesNature[g];
                                }
                            }

                            if (i != null && i.percentageChance > selectedIndex)
                            {
                                for (int f = 0; f < i.getHeight(direction); f++)
                                {
                                    for (int g = 0; g < i.getWidth(direction); g++)
                                    {
                                        if (i.getValue(direction, g, f, r) > 0)
                                            setLife(g + x - i.getWidth(direction) / 2, f + y - i.getHeight(direction) / 2, i.getValue(direction, g, f, r) ?? 0);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        //todo
                    }
                }
            }
            //crée une copie initiale pour pas que l'affichage initial crée un erreur
            ArrayGPS.BackupTablesNumbers();
            generateImage();
        }


        public static void generateImage()
        {
            for (int y = 0, yByXtotal = 0; y < _tailleY; y++, yByXtotal += _tailleX)
            {
                for (int x = 0; x < _tailleX; x++)
                {
                    if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] == 1)
                        DonneeTables[yByXtotal + x] = Color.Black;
                    if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] == 2)
                        DonneeTables[yByXtotal + x] = Color.Yellow;
                }
            }
        }

        public static void setLife(int x, int y, byte value = 1)
        {
            int rX = x < 0 ? _tailleX + x : (x >= _tailleX ? x - _tailleX : x),
                rY = y < 0 ? _tailleY + y : (y >= _tailleY ? y - _tailleY : y),
                nbYBackY = rY * _tailleX;
            if (value == 253)
            {
                TableauDeLaVie[rX, rY, ArrayGPS.SwapTablesNew] = 0;
            }
            else
            {
                TableauDeLaVie[rX, rY, ArrayGPS.SwapTablesNew] = value;
            }
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

            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 4 thread

            thread1 = new Thread(() =>
            {
                for (int y = 0, nbYBackY = 0; y < tailleY4; y++, nbYBackY += _tailleX)
                {
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1, cellSummary = TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.SwapTablesOld];
                        //choice                            

                        //black
                        if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 2)
                        {
                            if ((cellSummary >= 12 && cellSummary < 63) || cellSummary < 4)
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                        }
                        else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                            else
                            if (cellSummary == 3)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] > 0)
                            {
                                if (cellSummary == 2)
                                {
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                }
                            }
                            else
                            {
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                            }
                        }
                    }
                    cycleSummary += cycleXRowSummary;
                    cycleRowSummaries[ArrayGPS.SwapTablesNew, y] = cycleXRowSummary;
                }

                //checkout
                for (int oStart = 0; oStart < cycleMemory25; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.SwapTablesNewB] && oStart != ArrayGPS.SwapTablesNewB && oStart != ArrayGPS.SwapTablesNew)
                    {
                        //if there's a match, looks deeper into it
                        bool cancelledLookup = false;


                        for (int i = 0; i < _tailleY; i++)
                        {
                            if (cycleRowSummaries[oStart, i] != cycleRowSummaries[ArrayGPS.SwapTablesNewB, i])
                            {
                                cancelledLookup = true;
                                break;
                            }
                        }

                        if (!cancelledLookup)
                        {
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesNewB] != TableauDeLaVie[x, y, oStart])
                                    {
                                        cancelledLookup = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (cancelledLookup)
                        {
                            continue;
                        }
                        else
                        {
                            Stale = true;
                            StaleCycle = 0;
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.SwapTablesNewB; oToNew++, i++)
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

            thread2 = new Thread(() =>
            {
                for (int y = tailleY4, nbYBackY = tailleY4X; y < tailleY2; y++, nbYBackY += _tailleX)
                {
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1, cellSummary = TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.SwapTablesOld];
                        //choice                            

                        //black
                        if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 2)
                        {
                            if ((cellSummary >= 12 && cellSummary < 63) || cellSummary < 4)
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                        }
                        else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                            else
                            if (cellSummary == 3)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] > 0)
                            {
                                if (cellSummary == 2)
                                {
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                }
                            }
                            else
                            {
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                            }
                        }
                    }
                    cycleSummary += cycleXRowSummary;
                    cycleRowSummaries[ArrayGPS.SwapTablesNew, y] = cycleXRowSummary;
                }
                

                //checkout
                for (int oStart = cycleMemory25; oStart < cycleMemory50; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.SwapTablesNewB] && oStart != ArrayGPS.SwapTablesNewB && oStart != ArrayGPS.SwapTablesNew)
                    {
                        //if there's a match, looks deeper into it
                        bool cancelledLookup = false;

                        for (int i = 0; i < _tailleY; i++)
                        {
                            if (cycleRowSummaries[oStart, i] != cycleRowSummaries[ArrayGPS.SwapTablesNewB, i])
                            {
                                cancelledLookup = true;
                                break;
                            }
                        }

                        if (!cancelledLookup)
                        {
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesNewB] != TableauDeLaVie[x, y, oStart])
                                    {
                                        cancelledLookup = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (cancelledLookup)
                        {
                            continue;
                        }
                        else
                        {
                            Stale = true;
                            StaleCycle = 0;
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.SwapTablesNewB; oToNew++, i++)
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

            thread3 = new Thread(() =>
            {
                for (int y = tailleY2, nbYBackY = tailleY2X; y < tailleY75; y++, nbYBackY += _tailleX)
                {
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1, cellSummary = TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.SwapTablesOld];
                        //choice

                        //black
                        if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 2)
                        {
                            if ((cellSummary >= 12 && cellSummary<63) || cellSummary < 4)
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                        }
                        else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                            else
                            if (cellSummary == 3)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] > 0)
                            {
                                if (cellSummary == 2)
                                {
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                }
                            }
                            else
                            {
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                            }
                        }
                    }
                    cycleSummary += cycleXRowSummary;
                    cycleRowSummaries[ArrayGPS.SwapTablesNew, y] = cycleXRowSummary;
                }

                //checkout
                for (int oStart = cycleMemory50; oStart < cycleMemory75; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.SwapTablesNewB] && oStart != ArrayGPS.SwapTablesNewB && oStart != ArrayGPS.SwapTablesNew)
                    {
                        //if there's a match, looks deeper into it
                        bool cancelledLookup = false;


                        for (int i = 0; i < _tailleY; i++)
                        {
                            if (cycleRowSummaries[oStart, i] != cycleRowSummaries[ArrayGPS.SwapTablesNewB, i])
                            {
                                cancelledLookup = true;
                                break;
                            }
                        }

                        if (!cancelledLookup)
                        {
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesNewB] != TableauDeLaVie[x, y, oStart])
                                    {
                                        cancelledLookup = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (cancelledLookup)
                        {
                            continue;
                        }
                        else
                        {
                            Stale = true;
                            StaleCycle = 0;
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.SwapTablesNewB; oToNew++, i++)
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

            thread4 = new Thread(() =>
            {
                for (int y = tailleY75, nbYBackY = tailleY75X; y < _tailleY; y++, nbYBackY += _tailleX)
                {
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                        int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1, cellSummary = TableauDeLaVie[x, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yZero ? tailleYMinus : yMinus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[x, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, yMax ? 0 : yPlus, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xZero ? tailleXMinus : xMinus, y, ArrayGPS.SwapTablesOld] +
                            TableauDeLaVie[xMax ? 0 : xPlus, y, ArrayGPS.SwapTablesOld];
                        //choice

                        //black
                        if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 2)
                        {
                            if ((cellSummary >= 12 && cellSummary < 63) || cellSummary < 4)
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                        }
                        else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 2;
                            }
                            else
                            if (cellSummary == 3)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Black;
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                            }
                            else if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesOld] > 0)
                            {
                                if (cellSummary == 2)
                                {
                                    cycleXRowSummary++;
                                    DonneeTables[nbYBackY + x] = Color.Black;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 1;
                                }
                                else
                                {
                                    DonneeTables[nbYBackY + x] = Color.Transparent;
                                    TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                }
                            }
                            else
                            {
                                TableauDeLaVie[x, y, ArrayGPS.SwapTablesNew] = 0;
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                            }
                        }
                    }
                    cycleSummary += cycleXRowSummary;
                    cycleRowSummaries[ArrayGPS.SwapTablesNew, y] = cycleXRowSummary;
                }

                //checkout
                for (int oStart = cycleMemory75; oStart < _cycleMemory; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.SwapTablesNewB] && oStart != ArrayGPS.SwapTablesNewB && oStart != ArrayGPS.SwapTablesNew)
                    {
                        //if there's a match, looks deeper into it
                        bool cancelledLookup = false;


                        for (int i = 0; i < _tailleY; i++)
                        {
                            if (cycleRowSummaries[oStart, i] != cycleRowSummaries[ArrayGPS.SwapTablesNewB, i])
                            {
                                cancelledLookup = true;
                                break;
                            }
                        }

                        if (!cancelledLookup)
                        {
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVie[x, y, ArrayGPS.SwapTablesNewB] != TableauDeLaVie[x, y, oStart])
                                    {
                                        cancelledLookup = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (cancelledLookup)
                        {
                            continue;
                        }
                        else
                        {
                            Stale = true;
                            StaleCycle = 0;
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.SwapTablesNewB; oToNew++, i++)
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

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            //set the cycle summaries in case there's a match
            cycleSummaries[ArrayGPS.SwapTablesNew] = cycleSummary;
        }
    }
}

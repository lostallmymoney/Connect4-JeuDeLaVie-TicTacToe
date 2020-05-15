using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Threading;

namespace JeuDeLaVie
{
    public static class JeuDeLaVieTable
    {
        private static int _tailleX, _tailleY, _cycleMemory, _cycleStateAncient = 0, cycleSummary = 0, _memoryDistance, tailleY4, tailleY2, tailleY75, tailleY4X, tailleY2X, tailleY75X, tailleYMinus, tailleXMinus;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static Random generateur;
        private static Thread thread1, thread2, thread3, thread4;
        private static bool structureNature = true;
        private static StructureManager StructureMgr;
        private static int cycleMemory25, cycleMemory50, cycleMemory75;
        private static byte[][,] TableauDeLaVie;
        private static int[][][] cycleMapTable1;
        private static int IndexStorageSwapNew = 4, IndexStorageSwapOld = 5;

        public static Color[] DonneeTables { get; private set; }
        public static bool Stale { get; private set; } = false;
        public static int StaleCycle { get; private set; } = 0;
        public static bool AffichageChangement { get; set; }
        public static Color[] teamColors = { Color.Blue, Color.Red, Color.Chocolate, Color.White};

        static JeuDeLaVieTable()
        {
            StructureMgr = new StructureManager();
            generateur = new Random();
        }
        public static void GenerateNew(int memoryDistance = 1000, int nbAncientSummaries = 9, bool affichageChangement = false, double probabilite = 0.02, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            if (nbAncientSummaries < 1)
                nbAncientSummaries = 1;
            _memoryDistance = memoryDistance;
            _cycleMemory = nbAncientSummaries + 2;
            ArrayGPS.CycleReset(nbAncientSummaries + 2, nbAncientSummaries);
            AffichageChangement = affichageChangement;
            TableauDeLaVie = new byte[nbAncientSummaries + 2][,];
            for(int i=0; i < nbAncientSummaries + 2; i++)
            {
                TableauDeLaVie[i] = new byte[tailleX, tailleY];
            }
            cycleMapTable1 = new int[tailleY][][];
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
                cycleMapTable1[y] = new int[tailleX][];
                for (int x = 0; x < tailleX; x++)
                {

                    bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                    int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;

                    cycleMapTable1[y][x] = new int[6];

                    cycleMapTable1[y][x][0] = xZero ? tailleXMinus : xMinus;
                    cycleMapTable1[y][x][1] = yZero ? tailleYMinus : yMinus;
                    cycleMapTable1[y][x][2] = xMax ? 0 : xPlus;
                    cycleMapTable1[y][x][3] = yMax ? 0 : yPlus;

                    if (structureNature)
                    {
                        bool r = (generateur.NextDouble() <= 0.5);
                        int direction = generateur.Next(0, 4);
                        double selectedIndex = ((double)generateur.Next(0, 100000)) / 100000, templatesSum = StructureMgr.StructureTemplatesNature.Sum(i => i.percentageChance);

                        if (selectedIndex < templatesSum)
                        {
                            double summaryT = 0;
                            StructureTemplateNature i = null;

                            for (int g = 0; i == null && g < StructureMgr.StructureTemplatesNature.Count; g++)
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
                    if (TableauDeLaVie[ArrayGPS.SwapTablesNew][x, y] == 1)
                        DonneeTables[yByXtotal + x] = Color.Black;
                    if (TableauDeLaVie[ArrayGPS.SwapTablesNew][x, y] == 2)
                        DonneeTables[yByXtotal + x] = Color.Yellow;
                    if (TableauDeLaVie[ArrayGPS.SwapTablesNew][x, y] == 63)
                        DonneeTables[yByXtotal + x] = Color.Purple;
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
                TableauDeLaVie[ArrayGPS.SwapTablesNew][rX, rY] = 0;
            }else if(value>=100 && value < 200)
            {
                cycleMapTable1[rY][rX][IndexStorageSwapNew] = value;
                cycleMapTable1[rY][rX][IndexStorageSwapOld] = value;
                TableauDeLaVie[ArrayGPS.SwapTablesNew][rX, rY] = 255;
            }
            else
            {
                TableauDeLaVie[ArrayGPS.SwapTablesNew][rX, rY] = value;
            }
        }

        public static void CalculerCycle()
        {
            //thread calcule old
            ArrayGPS.BackupTablesNumbers();
            //translate les tables utilises vers le haut
            ArrayGPS.CycleAdd();

            int tempSwap = IndexStorageSwapNew;
            IndexStorageSwapNew = IndexStorageSwapOld;
            IndexStorageSwapOld = tempSwap;

            _cycleStateAncient++;
            if (_cycleStateAncient == _memoryDistance)
            {
                ArrayGPS.pushAncient1();
                _cycleStateAncient = 0;
            }

            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 4 thread

            byte[,] tempTableauDeLaVieOld = TableauDeLaVie[ArrayGPS.SwapTablesOld], tempTableauDeLaVieNew = TableauDeLaVie[ArrayGPS.SwapTablesNew];

            thread1 = new Thread(() =>
            {
                for (int y = 0, nbYBackY = 0; y < tailleY4; y++, nbYBackY += _tailleX)
                {
                    int[][] tempTableY = cycleMapTable1[y];
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        int[] tempTableX = tempTableY[x];
                        int cellSummary = tempTableauDeLaVieOld[tempTableX[0], tempTableX[1]] +
                                        tempTableauDeLaVieOld[x, tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[0], y] +
                                        tempTableauDeLaVieOld[tempTableX[2], y] +
                                        tempTableauDeLaVieOld[tempTableX[0], tempTableX[3]] +
                                        tempTableauDeLaVieOld[x, tempTableX[3]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[3]], cellSummary2 = cellSummary % 255 + cellSummary / 255;
                        //choice                            

                        //black
                        Color c = Color.Black;
                        byte intWeight = 1;
                        if (tempTableX[IndexStorageSwapOld] > 0 && tempTableX[IndexStorageSwapOld] >= 100 && tempTableX[IndexStorageSwapOld] < 200)
                        {
                            intWeight = 255;
                            c = teamColors[tempTableX[IndexStorageSwapOld] - 100];
                            tempTableX[IndexStorageSwapNew] = tempTableX[IndexStorageSwapOld];
                        }
                        int oldTableL = tempTableauDeLaVieOld[x, y];
                        if (oldTableL == 2)
                        {
                            if (cellSummary2 < 4 || (cellSummary2 >= 12 && cellSummary2 < 17))
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                        }
                        else if (oldTableL == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            tempTableauDeLaVieNew[x, y] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary2 == 3 || (cellSummary2 == 2 && oldTableL > 0))
                            {
                                if (cellSummary > 255)
                                {
                                    int[][] triageTable = new int[2][];
                                    triageTable[0] = new int[8] {
                                    cycleMapTable1[tempTableX[1]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[2]][IndexStorageSwapOld]
                                    };
                                    triageTable[1] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                                    for (int k = 0; k < 8; k++)
                                    {
                                        if (triageTable[0][k] > 0)
                                        {
                                            for (int l = 0; l < 8; l++)
                                            {
                                                if (triageTable[0][l] == triageTable[0][k] && l != k)
                                                {
                                                    triageTable[1][k]++;
                                                    triageTable[1][l] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            triageTable[1][k] = 0;
                                        }
                                        if (k > 0)
                                        {
                                            if (triageTable[1][k] > 0)
                                            {
                                                if (triageTable[1][0] < triageTable[1][k])
                                                {
                                                    triageTable[1][0] = triageTable[1][k];
                                                    triageTable[0][0] = triageTable[0][k];
                                                    triageTable[1][k] = 0;
                                                    triageTable[0][k] = 0;
                                                }
                                                else if (triageTable[1][0] == triageTable[1][k])
                                                {
                                                    triageTable[1][1] = triageTable[1][k];
                                                    triageTable[0][1] = triageTable[0][k];
                                                    if (k != 1)
                                                    {
                                                        triageTable[1][k] = 0;
                                                        triageTable[0][k] = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (triageTable[1][1] != triageTable[1][0])
                                    {
                                        tempTableX[IndexStorageSwapNew] = triageTable[0][0];
                                        intWeight = 255;
                                        c = teamColors[triageTable[0][0] - 100];
                                    }
                                }
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = c;
                                tempTableauDeLaVieNew[x, y] = intWeight;
                            }else
                            if (cellSummary2 > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                            else
                            {
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
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
                            byte[,] TableauDeLaVieoStart = TableauDeLaVie[oStart], TableauDeLaVieNewB = TableauDeLaVie[ArrayGPS.SwapTablesNewB];
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVieNewB[x, y] != TableauDeLaVieoStart[x, y])
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
                    int[][] tempTableY = cycleMapTable1[y];
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        int[] tempTableX = tempTableY[x];
                        int cellSummary = tempTableauDeLaVieOld[tempTableX[0], tempTableX[1]] +
                                        tempTableauDeLaVieOld[x, tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[0], y] +
                                        tempTableauDeLaVieOld[tempTableX[2], y] +
                                        tempTableauDeLaVieOld[tempTableX[0], tempTableX[3]] +
                                        tempTableauDeLaVieOld[x, tempTableX[3]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[3]], cellSummary2 = cellSummary % 255 + cellSummary / 255;
                        //choice                            

                        //black
                        Color c = Color.Black;
                        byte intWeight = 1;
                        if (tempTableX[IndexStorageSwapOld] > 0 && tempTableX[IndexStorageSwapOld] >= 100 && tempTableX[IndexStorageSwapOld] < 200)
                        {
                            intWeight = 255;
                            c = teamColors[tempTableX[IndexStorageSwapOld] - 100];
                            tempTableX[IndexStorageSwapNew] = tempTableX[IndexStorageSwapOld];
                        }
                        int oldTableL = tempTableauDeLaVieOld[x, y];
                        if (oldTableL == 2)
                        {
                            if (cellSummary2 < 4 || (cellSummary2 >= 12 && cellSummary2 < 17))
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                        }
                        else if (oldTableL == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            tempTableauDeLaVieNew[x, y] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary2 == 3 || (cellSummary2 == 2 && oldTableL > 0))
                            {
                                if (cellSummary > 255)
                                {
                                    int[][] triageTable = new int[2][];
                                    triageTable[0] = new int[8] {
                                    cycleMapTable1[tempTableX[1]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[2]][IndexStorageSwapOld]
                                    };
                                    triageTable[1] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                                    for (int k = 0; k < 8; k++)
                                    {
                                        if (triageTable[0][k] > 0)
                                        {
                                            for (int l = 0; l < 8; l++)
                                            {
                                                if (triageTable[0][l] == triageTable[0][k] && l != k)
                                                {
                                                    triageTable[1][k]++;
                                                    triageTable[1][l] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            triageTable[1][k] = 0;
                                        }
                                        if (k > 0)
                                        {
                                            if (triageTable[1][k] > 0)
                                            {
                                                if (triageTable[1][0] < triageTable[1][k])
                                                {
                                                    triageTable[1][0] = triageTable[1][k];
                                                    triageTable[0][0] = triageTable[0][k];
                                                    triageTable[1][k] = 0;
                                                    triageTable[0][k] = 0;
                                                }
                                                else if (triageTable[1][0] == triageTable[1][k])
                                                {
                                                    triageTable[1][1] = triageTable[1][k];
                                                    triageTable[0][1] = triageTable[0][k];
                                                    if (k != 1)
                                                    {
                                                        triageTable[1][k] = 0;
                                                        triageTable[0][k] = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (triageTable[1][1] != triageTable[1][0])
                                    {
                                        tempTableX[IndexStorageSwapNew] = triageTable[0][0];
                                        intWeight = 255;
                                        c = teamColors[triageTable[0][0] - 100];
                                    }
                                }
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = c;
                                tempTableauDeLaVieNew[x, y] = intWeight;
                            }
                            else
                            if (cellSummary2 > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                            else
                            {
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
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
                            byte[,] TableauDeLaVieoStart = TableauDeLaVie[oStart], TableauDeLaVieNewB = TableauDeLaVie[ArrayGPS.SwapTablesNewB];
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVieNewB[x, y] != TableauDeLaVieoStart[x, y])
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
                    int[][] tempTableY = cycleMapTable1[y];
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        int[] tempTableX = tempTableY[x];
                        int cellSummary = tempTableauDeLaVieOld[tempTableX[0], tempTableX[1]] +
                                        tempTableauDeLaVieOld[x, tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[0], y] +
                                        tempTableauDeLaVieOld[tempTableX[2], y] +
                                        tempTableauDeLaVieOld[tempTableX[0], tempTableX[3]] +
                                        tempTableauDeLaVieOld[x, tempTableX[3]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[3]], cellSummary2 = cellSummary % 255 + cellSummary / 255;
                        //choice                            

                        //black
                        Color c = Color.Black;
                        byte intWeight = 1;
                        if (tempTableX[IndexStorageSwapOld] > 0 && tempTableX[IndexStorageSwapOld] >= 100 && tempTableX[IndexStorageSwapOld] < 200)
                        {
                            intWeight = 255;
                            c = teamColors[tempTableX[IndexStorageSwapOld] - 100];
                            tempTableX[IndexStorageSwapNew] = tempTableX[IndexStorageSwapOld];
                        }
                        int oldTableL = tempTableauDeLaVieOld[x, y];
                        if (oldTableL == 2)
                        {
                            if (cellSummary2 < 4 || (cellSummary2 >= 12 && cellSummary2 < 17))
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                        }
                        else if (oldTableL == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            tempTableauDeLaVieNew[x, y] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary2 == 3 || (cellSummary2 == 2 && oldTableL > 0))
                            {
                                if (cellSummary > 255)
                                {
                                    int[][] triageTable = new int[2][];
                                    triageTable[0] = new int[8] {
                                    cycleMapTable1[tempTableX[1]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[2]][IndexStorageSwapOld]
                                    };
                                    triageTable[1] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                                    for (int k = 0; k < 8; k++)
                                    {
                                        if (triageTable[0][k] > 0)
                                        {
                                            for (int l = 0; l < 8; l++)
                                            {
                                                if (triageTable[0][l] == triageTable[0][k] && l != k)
                                                {
                                                    triageTable[1][k]++;
                                                    triageTable[1][l] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            triageTable[1][k] = 0;
                                        }
                                        if (k > 0)
                                        {
                                            if (triageTable[1][k] > 0)
                                            {
                                                if (triageTable[1][0] < triageTable[1][k])
                                                {
                                                    triageTable[1][0] = triageTable[1][k];
                                                    triageTable[0][0] = triageTable[0][k];
                                                    triageTable[1][k] = 0;
                                                    triageTable[0][k] = 0;
                                                }
                                                else if (triageTable[1][0] == triageTable[1][k])
                                                {
                                                    triageTable[1][1] = triageTable[1][k];
                                                    triageTable[0][1] = triageTable[0][k];
                                                    if (k != 1)
                                                    {
                                                        triageTable[1][k] = 0;
                                                        triageTable[0][k] = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (triageTable[1][1] != triageTable[1][0])
                                    {
                                        tempTableX[IndexStorageSwapNew] = triageTable[0][0];
                                        intWeight = 255;
                                        c = teamColors[triageTable[0][0] - 100];
                                    }
                                }
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = c;
                                tempTableauDeLaVieNew[x, y] = intWeight;
                            }
                            else
                            if (cellSummary2 > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                            else
                            {
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
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
                            byte[,] TableauDeLaVieoStart = TableauDeLaVie[oStart], TableauDeLaVieNewB = TableauDeLaVie[ArrayGPS.SwapTablesNewB];
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVieNewB[x, y] != TableauDeLaVieoStart[x, y])
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
                    int[][] tempTableY = cycleMapTable1[y];
                    int cycleXRowSummary = 0;
                    for (int x = 0; x < _tailleX; x++)
                    {
                        int[] tempTableX = tempTableY[x];
                        int cellSummary = tempTableauDeLaVieOld[tempTableX[0], tempTableX[1]] +
                                        tempTableauDeLaVieOld[x, tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[1]] +
                                        tempTableauDeLaVieOld[tempTableX[0], y] +
                                        tempTableauDeLaVieOld[tempTableX[2], y] +
                                        tempTableauDeLaVieOld[tempTableX[0], tempTableX[3]] +
                                        tempTableauDeLaVieOld[x, tempTableX[3]] +
                                        tempTableauDeLaVieOld[tempTableX[2], tempTableX[3]], cellSummary2 = cellSummary % 255 + cellSummary / 255;
                        //choice                            

                        //black
                        Color c = Color.Black;
                        byte intWeight = 1;
                        if (tempTableX[IndexStorageSwapOld] > 0 && tempTableX[IndexStorageSwapOld] >= 100 && tempTableX[IndexStorageSwapOld] < 200)
                        {
                            intWeight = 255;
                            c = teamColors[tempTableX[IndexStorageSwapOld] - 100];
                            tempTableX[IndexStorageSwapNew] = tempTableX[IndexStorageSwapOld];
                        }
                        int oldTableL = tempTableauDeLaVieOld[x, y];
                        if (oldTableL == 2)
                        {
                            if (cellSummary2 < 4 || (cellSummary2 >= 12 && cellSummary2 < 17))
                            {
                                DonneeTables[nbYBackY + x] = Color.Transparent;
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
                            }
                            else
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                        }
                        else if (oldTableL == 63)
                        {
                            DonneeTables[nbYBackY + x] = Color.Purple;
                            tempTableauDeLaVieNew[x, y] = 63;
                            cycleXRowSummary++;
                        }
                        else
                        {
                            if (cellSummary2 == 3 || (cellSummary2 == 2 && oldTableL > 0))
                            {
                                if (cellSummary > 255)
                                {
                                    int[][] triageTable = new int[2][];
                                    triageTable[0] = new int[8] {
                                    cycleMapTable1[tempTableX[1]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[1]][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[y][tempTableX[2]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[0]][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][x][IndexStorageSwapOld],
                                    cycleMapTable1[tempTableX[3]][tempTableX[2]][IndexStorageSwapOld]
                                    };
                                    triageTable[1] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                                    for (int k = 0; k < 8; k++)
                                    {
                                        if (triageTable[0][k] > 0)
                                        {
                                            for (int l = 0; l < 8; l++)
                                            {
                                                if (triageTable[0][l] == triageTable[0][k] && l != k)
                                                {
                                                    triageTable[1][k]++;
                                                    triageTable[1][l] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            triageTable[1][k] = 0;
                                        }
                                        if (k > 0)
                                        {
                                            if (triageTable[1][k] > 0)
                                            {
                                                if (triageTable[1][0] < triageTable[1][k])
                                                {
                                                    triageTable[1][0] = triageTable[1][k];
                                                    triageTable[0][0] = triageTable[0][k];
                                                    triageTable[1][k] = 0;
                                                    triageTable[0][k] = 0;
                                                }
                                                else if (triageTable[1][0] == triageTable[1][k])
                                                {
                                                    triageTable[1][1] = triageTable[1][k];
                                                    triageTable[0][1] = triageTable[0][k];
                                                    if (k != 1)
                                                    {
                                                        triageTable[1][k] = 0;
                                                        triageTable[0][k] = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (triageTable[1][1] != triageTable[1][0])
                                    {
                                        tempTableX[IndexStorageSwapNew] = triageTable[0][0];
                                        intWeight = 255;
                                        c = teamColors[triageTable[0][0] - 100];
                                    }
                                }
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = c;
                                tempTableauDeLaVieNew[x, y] = intWeight;
                            }
                            else
                            if (cellSummary2 > 10)
                            {
                                cycleXRowSummary++;
                                DonneeTables[nbYBackY + x] = Color.Yellow;
                                tempTableauDeLaVieNew[x, y] = 2;
                            }
                            else
                            {
                                tempTableauDeLaVieNew[x, y] = 0;
                                tempTableX[IndexStorageSwapNew] = 0;
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
                            byte[,] TableauDeLaVieoStart = TableauDeLaVie[oStart], TableauDeLaVieNewB = TableauDeLaVie[ArrayGPS.SwapTablesNewB];
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVieNewB[x, y] != TableauDeLaVieoStart[x, y])
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
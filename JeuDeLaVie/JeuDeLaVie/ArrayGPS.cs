using System;
using System.Collections.Generic;
using System.Linq;

namespace JeuDeLaVie
{
    public static class ArrayGPS
    {
        private static int swapTablesNew = 1, swapTablesOld = 0, swapTablesNewB = 1, swapTablesOldB = 0, cMem, cMemAncient, ancientSummariesNew = 1, ancientSummariesOld = 0;
        private static Queue<int> archivedAncientTablesIndex = new Queue<int>();
        private static int[] ancientMemPyramid;
        private static bool[] ancientMemPyramidBool;

        public static void IncrementAncientSummariesIndex()
        {
            pushAncient(swap: swapTablesNewB);
        }

        public static int GetAncientSummariesNew()
        {
            return ancientSummariesNew;
        }

        public static int GetAncientSummariesOld()
        {
            return ancientSummariesOld;
        }

        public static void BackupTablesNumbers()
        {
            swapTablesNewB = swapTablesNew;
            swapTablesOldB = swapTablesOld;
        }

        public static int GetCellMemmory()
        {
            return cMem;
        }

        public static int GetSwapTablesNew()
        {
            return swapTablesNew;
        }

        public static int GetSwapTablesOld()
        {
            return swapTablesOld;
        }

        public static int GetSwapTablesNewB()
        {
            return swapTablesNewB;
        }

        public static int GetSwapTablesOldB()
        {
            return swapTablesOldB;
        }

        public static void CycleAdd()
        {
            swapTablesOld = swapTablesNew;
            do
            {
                swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
            } while (ancientMemPyramid.Contains(swapTablesNew));
        }

        public static void pushAncient(int swap, int id = 0)
        {
            if (ancientMemPyramidBool[id])
            {
                ancientMemPyramidBool[id] = false;
                if (id < cMemAncient - 1) { 
                    pushAncient(ancientMemPyramid[id], id + 1);
                    ancientMemPyramid[id] = swap;
                }
                else
                {
                    ancientMemPyramid[id] = swap;
                }
            }
            else
            {
                ancientMemPyramid[id] = swap;
                ancientMemPyramidBool[id] = true;
            }
        }

        public static void CycleReset(int cycleMemory, int nbAncient)
        {
            swapTablesNew = 1;
            swapTablesOld = 0;
            ancientSummariesOld = 0;
            ancientSummariesNew = 1;
            BackupTablesNumbers();
            cMemAncient = nbAncient;
            cMem = cycleMemory;

            ancientMemPyramid = new int[nbAncient];
            ancientMemPyramidBool = new bool[nbAncient];
        }

        public static int CycleEmulateNew()
        {
            int r = swapTablesNewB;
            do {
                r = (r >= cMem - 1) ? 0 : r + 1;
            } while (ancientMemPyramid.Contains((r >= cMem - 1) ? 0 : r + 1)) ;
            return r;
        }

        public static int CycleEmulateOld()
        {
            int r = swapTablesOldB;
            do {
                r = (r >= cMem - 1) ? 0 : r + 1;
            } while (ancientMemPyramid.Contains((swapTablesOldB >= cMem - 1) ? 0 : swapTablesOldB + 1));
            return r;
        }
    }
}

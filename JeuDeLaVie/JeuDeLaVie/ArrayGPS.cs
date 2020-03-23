using System.Collections.Generic;

namespace JeuDeLaVie
{
    public static class ArrayGPS
    {
        private static int swapTablesNew = 1, swapTablesOld = 0, swapTablesNewB = 1, swapTablesOldB = 0, cMem, cMemAncient, ancientSummariesNew = 1, ancientSummariesOld = 0;
        private static Queue<int> archivedAncientTablesIndex = new Queue<int>();

        public static void IncrementAncientSummariesIndex()
        {
            if (archivedAncientTablesIndex.Count > cMemAncient-1)
            {
                archivedAncientTablesIndex.Dequeue();
            }

            archivedAncientTablesIndex.Enqueue(swapTablesNewB);            
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
            swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;

            while (archivedAncientTablesIndex.Contains(swapTablesNew))
            {
                swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
            }
        }

        public static void CycleReset(int cycleMemory, int nbAncient)
        {
            archivedAncientTablesIndex.Clear();
            swapTablesNew = 1;
            swapTablesOld = 0;
            ancientSummariesOld = 0;
            ancientSummariesNew = 1;
            BackupTablesNumbers();
            cMemAncient = nbAncient;
            cMem = cycleMemory;
        }

        public static int CycleEmulateNew()
        {
            int r = swapTablesNewB;
            
            do {
                r = (r >= cMem - 1) ? 0 : r + 1;
            } while (archivedAncientTablesIndex.Contains((r >= cMem - 1) ? 0 : r + 1)) ;
            return r;
        }

        public static int CycleEmulateOld()
        {
            int r = swapTablesOldB;

            do {
                r = (r >= cMem - 1) ? 0 : r + 1;
            } while (archivedAncientTablesIndex.Contains((swapTablesOldB >= cMem - 1) ? 0 : swapTablesOldB + 1));
            return r;
        }
    }
}

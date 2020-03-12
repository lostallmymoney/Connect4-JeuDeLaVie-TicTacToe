using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie
{
    static class ArrayGPS
    {
        private static int swapTablesNew = 1, swapTablesOld = 0, swapTablesNewB = 1, swapTablesOldB = 0, cMem;

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

        public static void cycleAdd()
        {
            swapTablesOld = (swapTablesOld >= cMem - 1) ? 0 : swapTablesOld + 1;
            swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
        }

        public static void cycleReset(int cycleMemory)
        {
            swapTablesNew = 1;
            swapTablesOld = 0;
            cMem = cycleMemory;
        }

        public static int cycleEmulateNew()
        {
            return (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
        }
        public static int cycleEmulateOld()
        {
            return (swapTablesOld >= cMem - 1) ? 0 : swapTablesOld + 1;
        }
    }
}

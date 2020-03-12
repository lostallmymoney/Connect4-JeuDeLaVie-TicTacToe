using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie
{
    public class ArrayGPS
    {
        public static ArrayGPS ArrayGPSInstance;
        public static ArrayGPS Instance
        {
            get
            {
                if (ArrayGPSInstance == null)
                    ArrayGPSInstance = new ArrayGPS();
                return ArrayGPSInstance;
            }
        }


        private static int swapTablesNew = 1, swapTablesOld = 0, swapTablesNewB = 1, swapTablesOldB = 0, cMem;

        public void BackupTablesNumbers()
        {
            swapTablesNewB = swapTablesNew;
            swapTablesOldB = swapTablesOld;
        }

        public int GetCellMemmory()
        {
            return cMem;
        }

        public int GetSwapTablesNew()
        {
            return swapTablesNew;
        }

        public int GetSwapTablesOld()
        {
            return swapTablesOld;
        }

        public int GetSwapTablesNewB()
        {
            return swapTablesNewB;
        }

        public int GetSwapTablesOldB()
        {
            return swapTablesOldB;
        }

        public void cycleAdd()
        {
            swapTablesOld = (swapTablesOld >= cMem - 1) ? 0 : swapTablesOld + 1;
            swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
        }

        public void cycleReset(int cycleMemory)
        {
            swapTablesNew = 1;
            swapTablesOld = 0;
            cMem = cycleMemory;
        }

        public int cycleEmulateNew()
        {
            return (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
        }
        public static int cycleEmulateOld()
        {
            return (swapTablesOld >= cMem - 1) ? 0 : swapTablesOld + 1;
        }
    }
}

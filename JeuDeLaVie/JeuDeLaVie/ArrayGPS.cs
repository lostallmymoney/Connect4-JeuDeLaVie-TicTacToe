using System.Linq;
using System.Runtime.CompilerServices;

namespace JeuDeLaVie
{
    public static class ArrayGPS
    {
        private static int swapTablesNew = 1, swapTablesOld = 0, swapTablesNewB = 1,cMem, cMemAncient;
        private static int[] ancientMemPyramid;
        private static bool[] ancientMemPyramidBool;

        public static void BackupTablesNumbers()
        {
            swapTablesNewB = swapTablesNew;
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

        public static void CycleAdd()
        {
            swapTablesOld = swapTablesNew;
            do
            {
                swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
            } while (ancientMemPyramid.Contains(swapTablesNew));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void pushAncient1()
        {
            if (ancientMemPyramidBool[0])
            {
                pushAncient(ancientMemPyramid[0], 1);
                ancientMemPyramid[0] = swapTablesNewB;
                ancientMemPyramidBool[0] = false;
            }
            else
            {
                ancientMemPyramid[0] = swapTablesNewB;
                ancientMemPyramidBool[0] = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void pushAncient(int swap, int id = 0)
        {
            if (ancientMemPyramidBool[id])
            {
                ancientMemPyramidBool[id] = false;
                if (id < cMemAncient - 1)
                {
                    pushAncient(ancientMemPyramid[id], id + 1);
                }
                ancientMemPyramid[id] = swap;
            }
            else
            {
                ancientMemPyramid[id] = swap;
                ancientMemPyramidBool[id] = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CycleReset(int cycleMemory, int nbAncient)
        {
            swapTablesNew = 1;
            swapTablesOld = 0;
            BackupTablesNumbers();
            cMemAncient = nbAncient;
            cMem = cycleMemory;

            ancientMemPyramid = new int[nbAncient];
            ancientMemPyramidBool = new bool[nbAncient];
            for(int i=0; i<nbAncient; i++)
            {
                ancientMemPyramidBool[i] = false;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CycleEmulateNew()
        {
            int r = swapTablesNewB;
            do {
                r = (r >= cMem - 1) ? 0 : r + 1;
            } while (ancientMemPyramid.Contains((r >= cMem - 1) ? 0 : r + 1)) ;
            return r;
        }
    }
}

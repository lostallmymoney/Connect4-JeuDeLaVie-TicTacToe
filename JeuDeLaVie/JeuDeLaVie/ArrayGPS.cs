namespace JeuDeLaVie
{
    public static class ArrayGPS
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

        public static void CycleAdd()
        {
            swapTablesOld = (swapTablesOld >= cMem - 1) ? 0 : swapTablesOld + 1;
            swapTablesNew = (swapTablesNew >= cMem - 1) ? 0 : swapTablesNew + 1;
        }

        public static void CycleReset(int cycleMemory)
        {
            swapTablesNew = 1;
            swapTablesOld = 0;
            BackupTablesNumbers();
            cMem = cycleMemory;
        }

        public static int CycleEmulateNew()
        {
            return (swapTablesNewB >= cMem - 1) ? 0 : swapTablesNewB + 1;
        }

        public static int CycleEmulateOld()
        {
            return (swapTablesOldB >= cMem - 1) ? 0 : swapTablesOldB + 1;
        }
    }
}

using System;
using System.Text;
using System.Threading;

namespace JeuDeLaVie
{
    public class JeuDeLaVieTable
    {
        public static JeuDeLaVieTable JeuDeLaVieIstance;
        public static JeuDeLaVieTable Instance
        {
            get
            {
                if (JeuDeLaVieIstance == null)
                    JeuDeLaVieIstance = new JeuDeLaVieTable();
                return JeuDeLaVieIstance;
            }
        }

        private static bool stale = false;
        private static int cycleSummary = 0;
        private static int nbCells;

        private static bool[,,] tableauDeLaVie;
        private int[] cycleSummaries;
        private int[,] cycleRowSummaries;
        private bool affichageChangement, staleProof = false;
        private int tailleX, tailleY, cycleMemory, staledAtCycle;
        private char symboleVie = 'o', symboleMourant = 'x', symboleNaissant = '.', symboleMort = ' ';
        private Random generateur;
        private Thread staleThread, thread1, thread2;

        public char SymboleVie { get => symboleVie; set => symboleVie = value; }
        public char SymboleMort { get => symboleMort; set => symboleMort = value; }
        public char SymboleMourant { get => symboleMourant; set => symboleVie = value; }
        public char SymboleNaissant { get => symboleNaissant; set => symboleMort = value; }
        public bool Stale { get => stale; }
        public bool[,,] TableauDeLaVie { get => tableauDeLaVie; }
        public bool StaleProof { set => staleProof = value; }
        public int StaleCycle { get => staledAtCycle; }

        //1 constructeur, toutes les façons de l'appeler sont bonne
        private JeuDeLaVieTable()
        {
            generateur = new Random();
        }

        public void GenerateNew(int cycleMemory = 6, bool affichageChangement = true, double probabilite = 0.06, int tailleX = 800, int tailleY = 600, bool staleProof = false)
        {
            this.cycleMemory = cycleMemory + 2;
            ArrayGPS.Instance.cycleReset(cycleMemory + 2);
            this.affichageChangement = affichageChangement;
            tableauDeLaVie = new bool[tailleX, tailleY, cycleMemory + 2];
            cycleSummaries = new int[cycleMemory + 2];
            cycleRowSummaries = new int[cycleMemory + 2, tailleY];
            this.tailleX = tailleX;
            this.tailleY = tailleY;

            while (staleThread != null && staleThread.IsAlive)
            {
                Thread.Sleep(10);
            }
            stale = false;

            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNewB()] = (generateur.NextDouble() <= probabilite);
                }
            }
            //crée une copie initiale pour pas que l'affichage initial crée un erreur
            ArrayGPS.Instance.BackupTablesNumbers();
        }

        public void staleTestThreadF()
        {
            for (int o = 0; !staleProof && o < cycleMemory; o++)
            {
                if (o != ArrayGPS.Instance.GetSwapTablesNewB() && o != ArrayGPS.Instance.cycleEmulateNew())
                {
                    //if there's a match, looks deeper into it
                    if (cycleSummaries[o] == cycleSummaries[ArrayGPS.Instance.GetSwapTablesNewB()])
                    {
                        bool cancelledLookup = false;
                        for (int i = 0; !cancelledLookup && i < tailleY; i++)
                        {
                            if (cycleRowSummaries[o, i] != cycleRowSummaries[ArrayGPS.Instance.GetSwapTablesNewB(), i])
                            {
                                cancelledLookup = true;
                            }
                        }

                        for (int y = 0; !cancelledLookup && y < tailleY; y++)
                        {
                            for (int x = 0; x < tailleX && !cancelledLookup; x++)
                            {
                                if (tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNewB()] != tableauDeLaVie[x, y, o])
                                {
                                    cancelledLookup = true;
                                }
                            }
                        }

                        if (!cancelledLookup)
                        {
                            stale = true;
                            staledAtCycle = 0;
                            for (int oToNew = o; oToNew != ArrayGPS.Instance.GetSwapTablesNew(); oToNew++)
                            {
                                if (oToNew == cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                staledAtCycle++;
                            }
                        }
                    }
                }
            }
        }

        //divide by 2 thread
        public void CalculerCycleThread1()
        {
            bool yMax, yZero;
            for (int y = 0; tailleY / 2 > y; y++)
            {
                int cycleXRowSummary = 0;
                yMax = (y == tailleY - 1);
                yZero = (y == 0);
                for (int x = 0; x < tailleX; x++)
                {
                    if (TestCell(x, y, yMax, (x == tailleX - 1), yZero, (x == 0)))
                    {
                        cycleSummary++;
                        cycleXRowSummary++;
                    }
                }
                cycleRowSummaries[ArrayGPS.Instance.GetSwapTablesNew(), y] = cycleXRowSummary;
            }
        }
        //divide by 2 thread
        public void CalculerCycleThread2()
        {
            bool yMax, yZero;
            for (int y = tailleY / 2; y < tailleY; y++)
            {
                int cycleXRowSummary = 0;
                yMax = (y == tailleY - 1);
                yZero = (y == 0);
                for (int x = 0; x < tailleX; x++)
                {
                    if (TestCell(x, y, yMax, (x == tailleX - 1), yZero, (x == 0)))
                    {
                        cycleSummary++;
                        cycleXRowSummary++;
                    }
                }
                cycleRowSummaries[ArrayGPS.Instance.GetSwapTablesNew(), y] = cycleXRowSummary;
            }
        }

        private bool TestCell(int x, int y, bool yMax, bool xMax, bool yZero, bool xZero)
        {
            int cellSummary;
            cellSummary = 0;
            //teste si c'est sur le cote et si non regarde si il y a une cellule en vie
            if (!yZero)
            {
                if (tableauDeLaVie[x, y - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                {
                    cellSummary++;
                }

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, y - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else//infinitemap
                {
                    if (tableauDeLaVie[tailleX - 1, y - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, y - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else//infinitemap
                {
                    if (tableauDeLaVie[0, y - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
            }
            else//infinitemap
            {
                if (tableauDeLaVie[x, tailleY - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, tailleY - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[tailleX - 1, tailleY - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, tailleY - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[0, tailleY - 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
            }

            if (!yMax)
            {
                if (tableauDeLaVie[x, y + 1, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, y + 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[tailleX - 1, y + 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, y + 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[0, y + 1, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
            }
            else//infinitemap
            {
                if (tableauDeLaVie[x, 0, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;

                if (!xZero)
                {
                    if (tableauDeLaVie[x - 1, 0, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[tailleX - 1, 0, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }

                if (!xMax)
                {
                    if (tableauDeLaVie[x + 1, 0, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
                else
                {
                    if (tableauDeLaVie[0, 0, ArrayGPS.Instance.GetSwapTablesOld()])
                        cellSummary++;
                }
            }

            if (!xZero)
            {
                if (tableauDeLaVie[x - 1, y, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;
            }
            else
            {
                if (tableauDeLaVie[tailleX - 1, y, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;
            }

            if (!xMax)
            {
                if (tableauDeLaVie[x + 1, y, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;
            }
            else
            {
                if (tableauDeLaVie[0, y, ArrayGPS.Instance.GetSwapTablesOld()])
                    cellSummary++;
            }

            //choice
            if (cellSummary < 2)
            {
                tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()] = false;
            }
            else if (cellSummary == 3)
            {
                tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()] = true;
            }
            else if (cellSummary >= 4)
            {
                tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()] = false;
            }
            else if (cellSummary == 2)
            {
                tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()] = tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesOld()];
            }

            return tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()];
        }

        public void CalculerCycle()
        {
            //thread calcule old
            ArrayGPS.Instance.BackupTablesNumbers();
            
            while (staleThread != null && staleThread.IsAlive)
            {
                Thread.Sleep(2);
            }

            staleThread = new Thread(staleTestThreadF);
            staleThread.Priority = ThreadPriority.Highest;
            staleThread.Start();

            //translate les tables utilises vers le haut
            ArrayGPS.Instance.cycleAdd();


            nbCells = tailleX * tailleY;
            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            thread1 = new Thread(CalculerCycleThread1);
            thread1.Priority = ThreadPriority.Highest;
            thread1.Start();
            thread2 = new Thread(CalculerCycleThread2);
            thread2.Priority = ThreadPriority.Highest;
            thread2.Start();
            while (thread1.IsAlive || thread2.IsAlive)
            {
                Thread.Sleep(2);
            }
            //look up the cycle summaries in case there's a match
            cycleSummaries[ArrayGPS.Instance.GetSwapTablesNew()] = cycleSummary;
        }

        public override string ToString()
        {
            StringBuilder affichageTotal = new StringBuilder();
            if (affichageChangement)
            {
                //affichage avec le changement
                for (int y = 0; y < tailleY; y++)
                {
                    for (int x = 0; x < tailleX; x++)
                    {
                        if (tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesOld()])
                        {
                            if (tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()])
                            {
                                affichageTotal.Append(symboleVie);
                            }
                            else
                            {
                                affichageTotal.Append(symboleMourant);
                            }
                        }
                        else
                        {
                            if (tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()])
                            {
                                affichageTotal.Append(symboleNaissant);
                            }
                            else
                            {
                                affichageTotal.Append(symboleMort);
                            }
                        }
                    }
                    affichageTotal.Append(Environment.NewLine);
                }
            }
            else
            {
                //affichange sans le changement
                for (int y = 0; y < tailleY; y++)
                {
                    for (int x = 0; x < tailleX; x++)
                    {
                        if (tableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNew()])
                        {
                            affichageTotal.Append(symboleVie);
                        }
                        else
                        {
                            affichageTotal.Append(symboleMort);
                        }
                    }
                    affichageTotal.Append(Environment.NewLine);
                }
            }
            return affichageTotal.ToString();
        }

    }
}

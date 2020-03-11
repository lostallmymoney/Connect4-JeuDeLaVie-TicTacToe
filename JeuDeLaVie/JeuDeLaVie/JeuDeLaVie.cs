using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie
{
    public class JeuDeLaVieTable
    {
        public static JeuDeLaVieTable JeuDeLaVieIstance;
        public static JeuDeLaVieTable Instance {
            get
            {
                if (JeuDeLaVieIstance == null)
                    JeuDeLaVieIstance = new JeuDeLaVieTable();
                return JeuDeLaVieIstance;
            }
        }

        private bool[,,] tableauDeLaVie;
        private int[] cycleSummaries;
        private bool stale = false, affichageChangement, staleProof = false;
        private int tailleX, tailleY, swapTablesNew = 1, swapTablesOld = 0, cycleMemory, staledAtCycle;
        private char symboleVie = 'o', symboleMourant = 'x', symboleNaissant = '.', symboleMort = ' ';
        private Random generateur;
        public char SymboleVie { get => symboleVie; set => symboleVie = value; }
        public char SymboleMort { get => symboleMort; set => symboleMort = value; }
        public char SymboleMourant { get => symboleMourant; set => symboleVie = value; }
        public char SymboleNaissant { get => symboleNaissant; set => symboleMort = value; }
        public bool Stale { get => stale; }
        public bool StaleProof { set => staleProof = value; }
        public int StaleCycle { get => staledAtCycle; }


        //1 constructeur, toutes les façons de l'appeler sont bonne
        private JeuDeLaVieTable()
        {
            generateur = new Random();
        }

        public void GenerateNew(int cycleMemory = 20, bool affichageChangement = true, double probabilite = 0.08, int tailleX = 50, int tailleY = 50, bool staleProof = false)
        {
            this.cycleMemory = cycleMemory + 1;
            this.affichageChangement = affichageChangement;
            tableauDeLaVie = new bool[tailleX, tailleY, cycleMemory + 1];
            cycleSummaries = new int[cycleMemory + 1];
            this.tailleX = tailleX;
            this.tailleY = tailleY;

            this.swapTablesNew = 1;
            this.swapTablesOld = 0;
            this.stale = false;


            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    tableauDeLaVie[x, y, swapTablesNew] = (generateur.NextDouble() <= probabilite);
                }
            }
            //crée une copie initiale du tableau pour pas que l'affichage initial crée un erreur
        }

        public void CalculerCycle()
        {
            //translate les tables utilises vers le haut
            if (swapTablesOld >= cycleMemory - 1)
            {
                swapTablesOld = 0;
            }
            else
            {
                swapTablesOld++;
            }

            if (swapTablesNew >= cycleMemory - 1)
            {
                swapTablesNew = 0;
            }
            else
            {
                swapTablesNew++;
            }

            //calcule le nombre de cellule adjascent
            int cellSummary, cycleSummary = 0;
            stale = false;
            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    cellSummary = 0;
                    //teste si c'est sur le cote et si non regarde si il y a une cellule en vie
                    if (y > 0)
                    {
                        if (tableauDeLaVie[x, y - 1, swapTablesOld])
                        {
                            cellSummary++;
                        }

                        if (x > 0)
                        {
                            if(tableauDeLaVie[x - 1, y - 1, swapTablesOld])
                                cellSummary++;
                        }
                        else//infinitemap
                        {
                            if (tableauDeLaVie[tailleX-1, y - 1, swapTablesOld])
                                cellSummary++;
                        }

                        if (x < tailleX - 1)
                        {
                            if(tableauDeLaVie[x + 1, y - 1, swapTablesOld])
                                cellSummary++;
                        } else//infinitemap
                        {
                            if (tableauDeLaVie[0, y - 1, swapTablesOld])
                                cellSummary++;
                        }
                    }
                    else//infinitemap
                    {
                        if (tableauDeLaVie[x, tailleY - 1, swapTablesOld])
                        {
                            cellSummary++;
                        }

                        if (x > 0)
                        {
                            if (tableauDeLaVie[x - 1, tailleY - 1, swapTablesOld])
                                cellSummary++;
                        }
                        else
                        {
                            if (tableauDeLaVie[tailleX - 1, tailleY - 1, swapTablesOld])
                                cellSummary++;
                        }

                        if (x < tailleX - 1)
                        {
                            if(tableauDeLaVie[x + 1, tailleY - 1, swapTablesOld])
                                cellSummary++;
                        } else
                        {
                            if (tableauDeLaVie[0, tailleY - 1, swapTablesOld])
                                cellSummary++;
                        }
                    }

                    if (y < tailleY -1)
                    {
                        if (tableauDeLaVie[x, y + 1, swapTablesOld])
                        {
                            cellSummary++;
                        }

                        if (x > 0)
                        {
                            if (tableauDeLaVie[x - 1, y + 1, swapTablesOld])
                                cellSummary++;
                        } else
                        {
                            if (tableauDeLaVie[tailleX-1, y + 1, swapTablesOld])
                                cellSummary++;
                        }

                        if (x < tailleX - 1)
                        {
                            if (tableauDeLaVie[x + 1, y + 1, swapTablesOld])
                                cellSummary++;
                        }
                        else
                        {
                            if (tableauDeLaVie[0, y + 1, swapTablesOld])
                                cellSummary++;
                        }
                    }
                    else//infinitemap
                    {
                        if (tableauDeLaVie[x, 0, swapTablesOld])
                        {
                            cellSummary++;
                        }

                        if (x > 0)
                        {
                            if(tableauDeLaVie[x - 1, 0, swapTablesOld])
                                cellSummary++;
                        } else
                        {
                            if (tableauDeLaVie[tailleX-1, 0, swapTablesOld])
                                cellSummary++;
                        }

                        if (x < tailleX - 1)
                        {
                            if(tableauDeLaVie[0, 0, swapTablesOld])
                                cellSummary++;
                        }
                    }

                    if (x > 0)
                    {
                        if(tableauDeLaVie[x - 1, y, swapTablesOld])
                            cellSummary++;
                    }else
                    {
                        if (tableauDeLaVie[tailleX-1, y, swapTablesOld])
                            cellSummary++;
                    }

                    if (x < tailleX - 1)
                    {
                        if(tableauDeLaVie[x + 1, y, swapTablesOld])
                        cellSummary++;
                    }
                    else
                    {
                        if (tableauDeLaVie[0, y, swapTablesOld])
                            cellSummary++;
                    }



                    if (cellSummary < 2)
                    {
                        tableauDeLaVie[x, y, swapTablesNew] = false;
                    }
                    else if (cellSummary == 3)
                    {
                        tableauDeLaVie[x, y, swapTablesNew] = true;
                    }
                    else if (cellSummary >= 4)
                    {
                        tableauDeLaVie[x, y, swapTablesNew] = false;
                    }
                    else if(cellSummary == 2)
                    {
                        tableauDeLaVie[x, y, swapTablesNew] = tableauDeLaVie[x, y, swapTablesOld];
                    }

                    if(tableauDeLaVie[x, y, swapTablesNew])
                    {
                        cycleSummary++;
                    }
                }
            }
            //look up the cycle summaries in case there's a match
            cycleSummaries[swapTablesNew] = cycleSummary;

            for(int o = 0; !staleProof && o<cycleMemory; o++)
            {
                if (o != swapTablesNew)
                {
                    //if there's a match, looks deeper into it
                    if(cycleSummaries[o] == cycleSummary)
                    {
                        bool cancelledLookup = false;
                        for(int y = 0; y < tailleY && !cancelledLookup; y++)
                        {
                            for(int x = 0; x < tailleX && !cancelledLookup; x++)
                            {
                                if (tableauDeLaVie[x, y, swapTablesNew] != tableauDeLaVie[x, y, o])
                                {
                                    cancelledLookup = true;
                                }
                            }
                        }

                        if (!cancelledLookup)
                        {
                            stale = true;
                            staledAtCycle = 0;
                            for(int oToNew = o; oToNew != swapTablesNew; oToNew++)
                            {
                                if(oToNew == cycleMemory-1)
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

        public override string ToString()
        {
            StringBuilder affichageTotal = new StringBuilder();
            if (affichageChangement) {
                //affichage avec le changement
                for (int y = 0; y < tailleY; y++)
                {
                    for (int x = 0; x < tailleX; x++)
                    {
                        if (tableauDeLaVie[x, y, swapTablesOld])
                        {
                            if(tableauDeLaVie[x, y, swapTablesNew])
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
                            if (tableauDeLaVie[x, y, swapTablesNew])
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
            } else
            {
                //affichange sans le changement
                for (int y = 0; y < tailleY; y++)
                {
                    for (int x = 0; x < tailleX; x++)
                    {
                        if (tableauDeLaVie[x, y, swapTablesNew])
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

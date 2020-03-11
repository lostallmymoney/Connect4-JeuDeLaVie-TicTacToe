using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    public class Connect4Game
    {
        public static Connect4Game JeuDeLaVieIstance;
        public static Connect4Game Instance
        {
            get
            {
                if (JeuDeLaVieIstance == null)
                    JeuDeLaVieIstance = new Connect4Game();
                return JeuDeLaVieIstance;
            }
        }

        private int[,] leJeu;
        private int tailleX, tailleY, nbPlayer, playerRound = 0, winner;
        private char[] playerChars;
        private bool isWon = false;

        public int PlayerRound { get => playerRound; }
        public int Winner { get => winner; }
        public bool IsWon { get => isWon; }

        private Connect4Game(){ }

        //static constructor
        public void GenerateNew(int tailleX = 7, int tailleY = 6, int nbPlayer = 2)
        {
            playerRound = 0;
            if (nbPlayer <= 8)
            {
                playerChars = new char[] { 'o', 'x', 'I', '@', 'W', '+', '#', 'Y' };
            }
            else
            {
                if (nbPlayer > 73) nbPlayer = 73;
                playerChars = new char[nbPlayer];
                string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
                Random rand = new Random();
                for (int n = 0; n < nbPlayer; n++)
                {
                    playerChars[n] = chars[rand.Next(0, chars.Length - 1)];
                    chars = chars.Replace(playerChars[n].ToString(), string.Empty);
                }
            }

            leJeu = new int[tailleY, tailleX];
            this.tailleY = tailleY;
            this.tailleX = tailleX;
            this.nbPlayer = nbPlayer;

            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    leJeu[y, x] = -1;
                }
            }
        }

        public bool play(int cordX)
        {
            if (cordX >= tailleX) return false;
            bool success = false;
            for(int y=tailleY-1; !success && y >= 0; y--)
            {
                if (leJeu[y, cordX] == -1)
                {
                    leJeu[y, cordX] = PlayerRound;
                    success = true;

                    testVictory(playerRound, y, cordX);

                    if (PlayerRound < nbPlayer - 1)
                    {
                        playerRound++;
                    }
                    else
                    {
                        playerRound = 0;
                    }
                }
            }
            return success;
        }

        public override string ToString()
        {
            StringBuilder affichageTotal = new StringBuilder();
            for(int y =0; y<tailleY; y++)
            {
                //affichagegrille
                affichageTotal.Append('-');
                for (int x =0; x<tailleX; x++)
                {
                    affichageTotal.Append("---");
                }
                affichageTotal.Append(Environment.NewLine);

                for(int x=0; x<tailleX; x++)
                {
                    //affichagegrille
                    affichageTotal.Append('|');

                    if(leJeu[y, x] != -1)
                    {
                        affichageTotal.Append(" " + playerChars[leJeu[y,x]]);
                    }
                    else
                    {
                        affichageTotal.Append("  ");
                    }
                }
                affichageTotal.Append('|' + Environment.NewLine);
            }

            //affichagegrille
            affichageTotal.Append('-');
            for (int x = 0; x < tailleX; x++)
            {
                affichageTotal.Append((x>9 ? "" : "-") + x + "-");
            }

            affichageTotal.Append(Environment.NewLine);

            return affichageTotal.ToString();
        }


        private void testVictory(int player, int y, int x)
        {
            int horizontalCount=0, verticalCount=0, diagInc=0, diagDec=0;
            bool broken;
            //test horizontal
            //left
            broken = false;
            for(int testX = 1; testX<4 && x-testX >=0 && !broken; testX++){
                if(leJeu[y, x-testX] == player)
                {
                    horizontalCount++;
                }
                else
                {
                    broken = true;
                }
            }

            //right
            broken = false;
            for (int testX = 1; testX < 4 && x + testX < tailleX && !broken; testX++)
            {
                if (leJeu[y, x + testX] == player)
                {
                    horizontalCount++;
                }
                else
                {
                    broken = true;
                }
            }

            //test Vertical
            //up
            broken = false;
            for (int testY = 1; testY < 4 && y - testY >= 0 && !broken; testY++)
            {
                if (leJeu[y - testY, x] == player)
                {
                    verticalCount++;
                }
                else
                {
                    broken = true;
                }
            }

            //down
            broken = false;
            for (int testY = 1; testY < 4 && y + testY < tailleY && !broken; testY++)
            {
                if (leJeu[y+testY, x] == player)
                {
                    verticalCount++;
                }
                else
                {
                    broken = true;
                }
            }

            //tests diagonaux
            //en haut a gauche
            broken = false;
            for (int testOffset = 1; testOffset < 4 && y - testOffset >= 0 && x - testOffset >= 0 && !broken; testOffset++)
            {
                if (leJeu[y - testOffset, x - testOffset] == player)
                {
                    diagDec++;
                }
                else
                {
                    broken = true;
                }
            }
            //en bas a droite
            broken = false;
            for (int testOffset = 1; testOffset < 4 && y + testOffset < tailleY && x + testOffset < tailleX && !broken; testOffset++)
            {
                if (leJeu[y + testOffset, x + testOffset] == player)
                {
                    diagDec++;
                }
                else
                {
                    broken = true;
                }
            }
            //en haut a droite
            broken = false;
            for (int testOffset = 1; testOffset < 4 && y - testOffset >= 0 && x + testOffset < tailleX && !broken; testOffset++)
            {
                if (leJeu[y - testOffset, x + testOffset] == player)
                {
                    diagInc++;
                }
                else
                {
                    broken = true;
                }
            }
            //en bas a gauche
            broken = false;
            for (int testOffset = 1; testOffset < 4 && y + testOffset < tailleY && x - testOffset >= 0 && !broken; testOffset++)
            {
                if (leJeu[y + testOffset, x - testOffset] == player)
                {
                    diagInc++;
                }
                else
                {
                    broken = true;
                }
            }

            //tests
            if (horizontalCount >= 3 || verticalCount >= 3 || diagDec >= 3 || diagInc >= 3)
            {
                isWon = true;
                winner = player;
            }

        }

        public char getPlayerCharacter(int player)
        {
            return playerChars[player];
        }

    }
}

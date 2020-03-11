using Connect4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4Console
{
    class Program
    {
        static void Main(string[] args)
        {
            int tailleY = 6, tailleX = 7, gameInput;
            bool parseSuccess;
            //max nb of players 73
            Connect4Game theGame = Connect4Game.Instance;
            theGame.GenerateNew(tailleY, tailleX, 2);

            Console.Write(theGame.ToString());
            while (!theGame.IsWon)
            {
                do
                {
                    Console.Write("Player "+ (theGame.PlayerRound + 1) + " (Character : '" + theGame.getPlayerCharacter(theGame.PlayerRound) + "') : ");
                    parseSuccess = int.TryParse(Console.ReadLine(), out gameInput);
                    Console.CursorTop--;
                    Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                } while (!parseSuccess || gameInput < 0 || gameInput >= tailleX);
                theGame.play(gameInput);
                Console.SetCursorPosition(0, 0);
                Console.Write(theGame.ToString());
            }
            Console.WriteLine("PLAYER "+(theGame.Winner + 1) + " WON !!!");
            Console.ReadKey();
        }
    }
}

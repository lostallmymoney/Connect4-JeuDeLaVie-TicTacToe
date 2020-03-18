using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacClass;

namespace TicTacToeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToe t = TicTacToe.Instance;

            while (!t.GameDone)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(t.ToString() + 
                    Environment.NewLine + " Player " + t.PlayerId + " :         ");
                Console.CursorLeft -= 8;
                t.nextStep(Console.ReadLine());
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(t.ToString() + Environment.NewLine);
            if (t.Winner != 0)
            {
                Console.WriteLine("Player : " + t.Winner + " has won !!!       ");
            } else
            {
                Console.WriteLine("STALEMATE              ");
            }

            Console.ReadKey();
        }
    }
}

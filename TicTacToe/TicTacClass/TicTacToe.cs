using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacClass
{
    public class TicTacToe
    {
        public static TicTacToe instance;

        private string formattedInput;
        private int testAnswer, turn = 0, playerInt = 1, winner = 0;
        private bool gameDone = false;
        private int[,] ticTacAr = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

        public static TicTacToe Instance
        {
            get
            {
                if (instance == null)
                    instance = new TicTacToe();
                return instance;
            }
        }
        public int PlayerId
        {
            get
            {
                return playerInt;
            }
        }
        public int Turn
        {
            get
            {
                return turn;
            }
        }
        public int Winner
        {
            get
            {
                return winner;
            }
        }
        public bool GameDone
        {
            get
            {
                return gameDone;
            }
        }

        private TicTacToe()
        {   }

        public bool nextStep(string coordsInput)
        {
            bool valableInput = false;
            if (!gameDone)
            {
                string position = null;
                if ((coordsInput.Length >= 2) && (coordsInput[0] == '1' || coordsInput[0] == '2' || coordsInput[0] == '3')
                    && (coordsInput[1] == '1' || coordsInput[1] == '2' || coordsInput[1] == '3')
                    && (ticTacAr[int.Parse(coordsInput[0].ToString()) - 1, int.Parse(coordsInput[1].ToString()) - 1] == 0))
                {
                    turn++;
                    ticTacAr[int.Parse(coordsInput[0].ToString()) - 1, int.Parse(coordsInput[1].ToString()) - 1] = playerInt;
                    this.ticTacTest();
                    playerInt = playerInt == 1 ? 2 : 1;
                    valableInput = true;
                    position = coordsInput.Substring(0, 2);
                }
            }
            return valableInput;
        }

        public override string ToString() //prints the tic tac toe
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                for (int e = 0; e < 3; e++)
                {
                    if (e != 0) b.Append('|');
                    b.Append(ticTacAr[i, e] == 1 ? 'X' : ticTacAr[i, e] == 2 ? 'O' : ' ');
                }
                b.Append(Environment.NewLine);
                if (i < 2) b.Append("-----"+Environment.NewLine);
            }
            return b.ToString();
        }

        public void ticTacTest() //verify win
        {
            for (int o = 0; o < 3; o++)
            {
                if (ticTacAr[o, 0] != 0 && ticTacAr[o, 0] == ticTacAr[o, 1] && ticTacAr[o, 0] == ticTacAr[o, 2]) winner = ticTacAr[o, 0];
            }
            for (int o = 0; o < 3; o++)
            {
                if (ticTacAr[0, o] != 0 && ticTacAr[0, o] == ticTacAr[1, o] && ticTacAr[0, o] == ticTacAr[2, o]) winner = ticTacAr[0, o];
            }
            if ((ticTacAr[0, 0] != 0 && ticTacAr[0, 0] == ticTacAr[1, 1] && ticTacAr[0, 0] == ticTacAr[2, 2])) winner = ticTacAr[0, 0];
            if (ticTacAr[0, 2] != 0 && ticTacAr[0, 2] == ticTacAr[1, 1] && ticTacAr[0, 2] == ticTacAr[2, 0]) winner = ticTacAr[0, 2];
            if (this.winner != 0)
            {
                gameDone = true;
            } else 
            if (turn >= 9)
            {
                gameDone = true;
            }
        }

         
    }
}

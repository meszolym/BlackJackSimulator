using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator
{
    enum Action
    {
        Hit,
        Stand,
        DoubleHit,
        DoubleStand,
        Split
    }



    internal class Strategy
    {

        string[,] HardTotalMatrix;
        string[,] SoftTotalMatrix;
        string[,] PairMatrix;

        public Strategy(string[,] htmatrix, string[,] stmatrix, string[,] pairmatrix)
        {
            if (htmatrix.GetLength(0) != 19
                || htmatrix.GetLength(1) != 11)
            {
                throw new ArgumentException("Hard total matrix must be 19x11");
            }

            if (stmatrix.GetLength(0) != 9
                || stmatrix.GetLength(1) != 11)
            {
                throw new ArgumentException("Soft total matrix must be 9x11");
            }

            if (pairmatrix.GetLength(0) != 11
                || pairmatrix.GetLength(1) != 11)
            {
                throw new ArgumentException("Pair matrix must be 11x11");
            }

            HardTotalMatrix = htmatrix;
            SoftTotalMatrix = stmatrix;
            PairMatrix = pairmatrix;

        }

        internal Action GetAction(Hand hand, Card dealerUpCard)
        {
            var val = hand.GetValue();
            var dealerVal = dealerUpCard.GetValue();
            if (val.IsBlackJack)
            {
                return Action.Stand;
            }
            
            if (val.IsSoft)
            {
                //search soft total matrix
                return getActionFromMatrix(SoftTotalMatrix, val, dealerVal);
            }

            if (hand.Cards[0] == hand.Cards[1])
            {
                //search pair matrix
                return getActionFromMatrix(PairMatrix, val, dealerVal);
            }

            //search hard total matrix
            return getActionFromMatrix(HardTotalMatrix, val, dealerVal);
        }

        Action getActionFromMatrix(string[,] matrix, HandValue playerVal, int dealerVal)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, 0] == playerVal.ToString())
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[0, j] == dealerVal.ToString())
                        {
                            switch (matrix[i, j])
                            {
                                case "H":
                                    return Action.Hit;
                                case "S":
                                    return Action.Stand;
                                case "D":
                                    return Action.DoubleHit;
                                case "DS":
                                    return Action.DoubleStand;
                                default:
                                    throw new Exception("Should never get here");
                            }
                        }
                    }
                }
            }
            throw new Exception("Should never get here");
        }
    }
}

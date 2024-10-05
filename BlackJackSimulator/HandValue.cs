using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;

namespace BlackJackSimulator
{
    internal class HandValue
    {
        internal readonly int Value;
        internal readonly bool IsSoft;
        internal bool IsBlackJack;
        internal bool IsPair;

        internal HandValue(int value, bool isSoft, bool isBlackJack, bool isPair)
        {
            Value = value;
            IsSoft = isSoft;
            IsBlackJack = isBlackJack;
            IsPair = isPair;
        }

        /// <summary>
        /// Gets the value of a hand
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        internal static HandValue GetValue(Hand hand)
        {
            int value = 0;
            bool isSoft = false;
            bool isBlackJack = false;
            bool isPair = false;

            foreach (var card in hand.Cards)
            {
                if (card == Card.Jack || card == Card.Queen || card == Card.King)
                {
                    value += 10;
                    //if the card is a face card, add 10 to the value
                    continue;
                }
                value += (int)card;
                //if the card is not a face card, add the value of the card to the value
                //the value of ace is 1 for now.
            }

            //if the hand has an ace and can be counted as 11 without busting, it's soft and the value is increased by 10
            //remember that the value of ace was counted as 1 before
            if (hand.Cards.Count(x => x == Card.Ace) > 0 && value <= 11)
            {
                value += 10;
                isSoft = true;
            }

            if (hand.Cards.Count == 2 && value == 21)
            {
                isBlackJack = true;
                isSoft = false;
            }

            if (hand.Cards.Count == 2 && hand.Cards[0] == hand.Cards[1] && !hand.IsSplit)
            {
                isPair = true;
            }

            return new HandValue(value, isSoft, isBlackJack, isPair);
        }

        /// <summary>
        /// Gets the value of a card
        /// </summary>
        /// <param name="card"></param>
        internal static int GetValue(Card card)
        {
            if (card == Card.Jack || card == Card.Queen || card == Card.King)
            {
                return 10;
            }
            else if (card == Card.Ace)
            {
                return 11;
            }
            else
            {
                return (int)card;
            }
        }

        public override string ToString()
        {
            if (IsBlackJack)
            {
                return "BJ";
            }
            if (IsSoft && IsPair)
            {
                return "P11"; //Pair of Aces
            }
            if (IsSoft)
            {
                return $"S{Value}";
            }
            if (IsPair)
            {
                return $"P{Value/2}";
            }
            return Value.ToString();
        }

        public static bool operator <(HandValue left, HandValue right)
        {
            return left.Value < right.Value || (!left.IsBlackJack && right.IsBlackJack);
        }

        public static bool operator >(HandValue left, HandValue right)
        {
            return left.Value > right.Value || (left.IsBlackJack && !right.IsBlackJack);
        }

        public static bool operator ==(HandValue left, HandValue right)
        {
            return !(left < right || left > right);
        }

        public static bool operator !=(HandValue left, HandValue right)
        {
            return left < right || left > right;
        }
    }
}

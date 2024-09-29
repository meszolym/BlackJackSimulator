using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator
{
    internal class HandValue
    {
        internal readonly int Value;
        internal readonly bool IsSoft;
        internal bool IsBlackJack
        {
            get
            {
                //if the value of the hand is 21 and it's soft, it's a blackjack
                return Value == 21 && IsSoft;
            }
        }

        internal HandValue(int value, bool isSoft)
        {
            Value = value;
            IsSoft = isSoft;
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

            foreach (var card in hand.Cards)
            {
                if (card == Card.Jack || card == Card.Queen || card == Card.King)
                {
                    value += 10;
                    //if the card is a face card, add 10 to the value
                }
                else
                {
                    value += (int)card;
                    //if the card is not a face card, add the value of the card to the value
                    //the value of ace is 1 for now.
                }
            }

            //if the hand has an ace and can be counted as 11 without busting, it's soft and the value is increased by 10
            //remember that the value of ace was counted as 1 before
            if (hand.Cards.Count(x => x == Card.Ace) > 0 && value <= 11)
            {
                value += 10;
                isSoft = true;
            }
            return new HandValue(value, isSoft);
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;

namespace BlackJackSimulator
{
    internal class Hand
    {

        internal List<Card> Cards = new();
        internal double Bet; //the bet placed on the hand
        internal bool InPlay; //tells if the hand is in play
        internal bool IsSplit; //tells if the hand comes from a split

        internal HandValue GetValue()
        {
            return HandValue.GetValue(this);
        }

        public override string ToString()
        {
            return this.GetValue().ToString();
        }

    }
}

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
        internal Action GetAction(Hand hand, Card dealerUpCard)
        {
            //mimic the dealer
            if (hand.GetValue().Value < 17)
            {
                return Action.Hit;
            }
            return Action.Stand;
        }
    }
}

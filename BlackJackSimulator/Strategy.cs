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

    internal struct ActionKey
    {
        internal string PlayerHandValue;
        internal string DealerUpCardValue;
    }

    internal class Strategy
    {

        internal Dictionary<ActionKey, Action> StrategySteps = new();

        internal Action GetAction(Hand hand, Card dealerUpCard)
        {
            string handval = hand.GetValue().ToString();
            string dealerval = dealerUpCard.GetValue().ToString();
            
            return StrategySteps.First(
                x => x.Key.PlayerHandValue == handval
                && x.Key.DealerUpCardValue == dealerval).Value;
        }

    }
}

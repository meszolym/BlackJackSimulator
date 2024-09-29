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

    internal static class Strategy
    {

        internal static Dictionary<ActionKey, Action> StrategySteps = new();

        internal static Action GetAction(Hand hand, Card dealerUpCard, bool canSplit = false)
        {
            string handval = hand.GetValue().ToString();
            string dealerval = dealerUpCard.GetValue().ToString();
            
            Action a = StrategySteps.Single(
                x => x.Key.PlayerHandValue == handval
                && x.Key.DealerUpCardValue == dealerval).Value;

            
            return a;
        }

    }
}

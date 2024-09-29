using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator
{
    internal abstract class Agent
    {
        internal abstract Hand[] Hands { get; set; }
        internal abstract void PlayHands(Card? dealerUpCard);
        internal virtual void ClearHand()
        {
            foreach (var hand in Hands)
            {
                hand.Cards.Clear();
                hand.InPlay= true;
                hand.Bet = 1;
                hand.IsSplit = false;
            }
        }
        

    }
}

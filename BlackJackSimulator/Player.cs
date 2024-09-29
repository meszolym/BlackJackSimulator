using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator
{
    internal class Player : Agent
    {

        internal override Hand[] Hands { get; set; }
        internal double Balance;

        internal bool InPlay { 
            get
            {
                return Hands.Any(h => h.InPlay);
            }
        }

        public Player()
        {
            Hands = new Hand[2];
            Hands[0] = new Hand();
            Hands[1] = new Hand();
        }

        internal override void ClearHand()
        {
            base.ClearHand();
            Hands[1].InPlay = false;
        }

        internal override void PlayHands(Card? dealerUpCard)
        {
            if (dealerUpCard == null)
            {
                throw new ArgumentNullException(nameof(dealerUpCard), "Dealer upcard cannot be null when playing player's hand!");
            }

            //play each hand
        }
    }
}

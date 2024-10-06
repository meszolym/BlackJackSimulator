using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;
using BlackJackSimulator.Models.Abstracts;

namespace BlackJackSimulator
{
    internal class Dealer : Agent
    {

        protected override Hand[] HandsMutable { get; set; }
        internal Hand Hand { get { return HandsMutable[0]; } }
        public Dealer()
        {
            HandsMutable = new Hand[1];
            HandsMutable[0] = new Hand();
        }

        public override void PlayHands(Shoe shoe, Card dealerUpCard)
        {
            while (Hand.GetValue().Value < 17)
            {
                Hand.Hit(shoe);
            }
        }
    }
}

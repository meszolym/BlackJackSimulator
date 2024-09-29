using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator
{
    internal class Dealer : Agent
    {

        internal override Hand[] Hands { get; set; }
        internal Hand Hand { get { return Hands[0]; } }
        public Dealer()
        {
            Hands = new Hand[1];
            Hands[0] = new Hand();
        }

        internal override void PlayHands(Game game)
        {
            while (Hand.GetValue().Value < 17)
            {
                game.Hit(Hand);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models.Abstracts;
using BlackJackSimulator.Models.Enums;

namespace BlackJackSimulator.Models
{
    public class Dealer : Agent
    {
        protected override Hand[] HandsMutable
        {
            get => [_hand];
            set => _hand = value[0];
        }

        public Hand Hand => _hand;

        private Hand _hand = new();

        public override void PlayHands(Shoe shoe, Card dealerUpCard)
        {
            //dealer hits until reaching any type of 17
            while (_hand.GetValue().Value < 17)
            {
                _hand.Hit(shoe);
            }
        }
    }
}

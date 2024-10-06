using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;

namespace BlackJackSimulator
{
    internal abstract class Agent
    {
        public IReadOnlyCollection<Hand> Hands
        {
            get
            {
                return HandsMutable.AsReadOnly();
            }
        }
        protected abstract Hand[] HandsMutable { get; set; }
        public abstract void PlayHands(Shoe shoe, Card dealerUpCard);

        public virtual void ClearHand()
        {
            foreach (var hand in HandsMutable)
            {
                hand.Reset();
            }
        }
        

    }
}

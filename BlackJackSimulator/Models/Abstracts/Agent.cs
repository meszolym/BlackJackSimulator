using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models.Enums;

namespace BlackJackSimulator.Models.Abstracts
{
    internal abstract class Agent
    {
        public IReadOnlyCollection<Hand> Hands => HandsMutable.AsReadOnly();
        protected abstract Hand[] HandsMutable { get; set; }
        public abstract void PlayHands(Shoe shoe, Card dealerUpCard);
        public virtual void ClearHand() => HandsMutable.Iter(hand => hand.Reset());

    }
}

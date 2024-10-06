using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace BlackJackSimulator.Models
{

    public record HandError(Option<string> Message, Option<Exception> InnerException);

    public class Hand
    {
        private List<Card> CardsMutable;
        public ReadOnlyCollection<Card> Cards => CardsMutable.AsReadOnly();
        public double Bet { get; private set; } //the bet placed on the hand
        public HandState State { get; private set; } //tells if the hand is in play
        public bool IsSplit { get; private set; } //tells if the hand comes from a split

        private Hand(List<Card> cardsMutable, double bet, HandState state, bool isSplit)
        {
            CardsMutable = cardsMutable;
            Bet = bet;
            State = state;
            IsSplit = isSplit;
        }

        public Hand()
        {
            CardsMutable = new List<Card>();
            Bet = 1;
            State = HandState.InPlay;
            IsSplit = false;
        }

        public Either<HandError, Unit> Double(Shoe shoe)
        {
            if (GetPossibleActions().CanDouble)
            {
                Bet *= 2;
                CardsMutable.Add(shoe.DrawCard());
                return unit;
            }
            return new HandError("Cannot double this hand!", Option<Exception>.None);
        }

        public Either<HandError, Unit> Hit(Shoe shoe)
        {
            if (GetPossibleActions().CanHit || CardsMutable.Count == 1)
            {
                CardsMutable.Add(shoe.DrawCard());
                return unit;
            }
            return new HandError("Cannot hit this hand!", Option<Exception>.None);
        }

        public Either<HandError, Hand[]> Split() //validate
        {
            if (GetPossibleActions().CanSplit)
            {
                Hand[] hands = new Hand[2];
                var tempC = this.CardsMutable[1];
                this.CardsMutable.RemoveAt(1);
                this.IsSplit = true;
                hands[0] = this;
                hands[1] = new Hand(new List<Card> { tempC }, this.Bet, HandState.InPlay, true);
                return hands;
            }
            return new HandError("Cannot split this hand!", Option<Exception>.None);
        }

        public void Lose() => State = HandState.Lost;
        public void Win() => State = HandState.Won;
        public void RemoveFromGame() => State = HandState.NotPlaying;
        public HandValue GetValue() => HandValue.GetValue(this);
        public override string ToString() => GetValue().ToString();

        public void Reset()
        {
            CardsMutable.Clear();
            Bet = 1;
            State = HandState.InPlay;
            IsSplit = false;
        }

        /// <summary>
        /// Gets the possible actions on a hand
        /// </summary>
        /// <returns></returns>
        internal virtual PossibleActions GetPossibleActions()
        {

            var possibleActions = new PossibleActions();


            if (this.GetValue().Value >= 21)
            {
                //busted hands & 21 can only stand
                return possibleActions;
            }

            if (this.Cards.Count == 2)
            {
                //check for split aces
                if (this.Cards[0] == Card.Ace && this.IsSplit)
                {
                    //no actions can be taken on split aces
                    return possibleActions;
                }

                //double any 2 cards + DAS allowed
                possibleActions.CanDouble = true;

                if (this.Cards[0] == this.Cards[1] && !this.IsSplit)
                {
                    //split any identical pair, if not already split
                    possibleActions.CanSplit = true;
                }
            }

            //if the hand has more than 2 cards, only hit and stand are possible
            possibleActions.CanHit = true;

            return possibleActions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models.Enums;
using BlackJackSimulator.Models.Errors;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace BlackJackSimulator.Models
{
    public class Hand
    {
        private List<Card> _cardsMutable;
        public ReadOnlyCollection<Card> Cards => _cardsMutable.AsReadOnly();
        public double Bet { get; private set; } //the bet placed on the hand
        public HandState State { get; private set; } //tells if the hand is in play
        public bool IsSplit { get; private set; } //tells if the hand comes from a split

        private Hand(List<Card> cardsMutable, double bet, HandState state, bool isSplit)
        {
            _cardsMutable = cardsMutable;
            Bet = bet;
            State = state;
            IsSplit = isSplit;
        }

        public Hand()
        {
            _cardsMutable = new List<Card>();
            Bet = 1;
            State = HandState.InPlay;
            IsSplit = false;
        }

        public Either<HandError, Unit> Double(Shoe shoe)
        {
            if (!GetPossibleActions().CanDouble)
                return new HandError("Cannot double this hand!", Option<Exception>.None);

            Bet *= 2;

            return shoe.DrawCard()
                .Map(card => addCardToHand(card, _cardsMutable))
                .MapLeft(error => new HandError(error.Message, error.InnerException)); //!!
        }

        public Either<HandError, Unit> Hit(Shoe shoe)
        {
            if (!GetPossibleActions().CanHit && _cardsMutable.Count != 1)
                return new HandError("Cannot hit this hand!", Option<Exception>.None);

            return shoe.DrawCard()
                .Map(card => addCardToHand(card, _cardsMutable))
                .MapLeft(error => new HandError(error.Message, error.InnerException)); //!!
        }


        private Func<Card, IList<Card>, Unit> addCardToHand = (card, cardList) =>
        {
            cardList.Add(card);
            return unit;
        };

        /// <summary>
        /// Splits a hand into two hands, WITH 1 CARD EACH
        /// </summary>
        /// <returns></returns>
        public Either<HandError, Hand[]> Split() //validate
        {
            if (!GetPossibleActions().CanSplit) 
                return new HandError("Cannot split this hand!", Option<Exception>.None);

            var tempC = this._cardsMutable[1];
            this._cardsMutable.RemoveAt(1);
            this.IsSplit = true;

            var hands = new Hand[2];
            hands[0] = this;
            hands[1] = new Hand([tempC], this.Bet, HandState.InPlay, true);
            return hands;
        }

        public void Lose() => State = HandState.Lost;
        public void Win() => State = HandState.Won;
        public void RemoveFromGame() => State = HandState.NotPlaying;
        public HandValue GetValue() => HandValue.GetValue(this);
        public override string ToString() => GetValue().ToString();

        public void Reset()
        {
            _cardsMutable.Clear();
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

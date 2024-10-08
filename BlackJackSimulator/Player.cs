using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;
using BlackJackSimulator.Models.Abstracts;
using BlackJackSimulator.Models.Enums;
using LanguageExt;
using static BlackJackSimulator.Game;
using Action = BlackJackSimulator.Models.Enums.Action;

namespace BlackJackSimulator
{
    internal class Player : Agent
    {

        protected override Hand[] HandsMutable
        {
            get => _handsMutable;
            set => _handsMutable = value;
        }
        private Hand[] _handsMutable;

        public double Balance;

        private readonly Strategy _strategy;
        public Player(Strategy strategy)
        {
            _handsMutable = new Hand[2];
            _handsMutable[0] = new Hand();
            _handsMutable[1] = new Hand();
            _strategy = strategy;
        }

        public override void ClearHand()
        {
            base.ClearHand();
            HandsMutable[1].RemoveFromGame(); //split hand not in play yet
        }

        public override void PlayHands(Shoe shoe, Card dealerUpCard)
        {
            var possibleActions = HandsMutable[0].GetPossibleActions();
            Action? actionToTake = null;
            if (possibleActions.CanHit || possibleActions.CanDouble || possibleActions.CanSplit)
            {
                actionToTake = _strategy.GetAction(HandsMutable[0].GetValue().ToString(), dealerUpCard.GetValue().ToString())
                    .Match(
                        Some: x => x,
                        None: () => throw new Exception("No action found for hand")
                    );
            }

            if (actionToTake == Action.Split && possibleActions.CanSplit)
            {
                //split the hand into two
                HandsMutable = HandsMutable[0].Split().Match(
                    Left: x => throw new Exception("Error splitting hand"), //!!
                    Right: x => x
                );

                HandsMutable[0].Hit(shoe);
                //play hand1
                PlayOneHand(HandsMutable[0], shoe, dealerUpCard);


                HandsMutable[1].Hit(shoe);
                //play hand2
                PlayOneHand(HandsMutable[1], shoe, dealerUpCard);
                return;
            }
            PlayOneHand(HandsMutable[0], shoe, dealerUpCard);
        }

        internal void PlayOneHand(Hand hand, Shoe shoe, Card dealerUpcard)
        {
            PossibleActions possibleActions;
            Action? actionToTake = null;
            do
            {
                possibleActions = hand.GetPossibleActions();
                if (possibleActions.CanHit || possibleActions.CanDouble || possibleActions.CanSplit)
                {
                    actionToTake = _strategy.GetAction(hand.GetValue().ToString(), dealerUpcard.GetValue().ToString()).Match(
                        Some: x => x,
                        None: () => throw new Exception("No action found for hand")
                        );
                }

                if (actionToTake == Action.Hit && possibleActions.CanHit)
                {
                    hand.Hit(shoe);
                    continue;
                }

                if (actionToTake == Action.DoubleHit)
                {
                    if (possibleActions.CanDouble)
                    {
                        hand.Double(shoe);
                        return;
                    }

                    hand.Hit(shoe);
                    continue;
                }

                if (actionToTake == Action.DoubleStand)
                {
                    if (possibleActions.CanDouble)
                    {
                        hand.Double(shoe);
                    }
                    return;
                }

                if (actionToTake == Action.Split)
                {
                    throw new Exception("Should never get here");
                }

            } while (hand.GetValue().Value < 21 && actionToTake != null && actionToTake != Action.Stand);

            


        }
    }
}

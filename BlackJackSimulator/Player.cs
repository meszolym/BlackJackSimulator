using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;
using LanguageExt;
using static BlackJackSimulator.Game;
using Action = BlackJackSimulator.Models.Action;

namespace BlackJackSimulator
{
    internal class Player : Agent
    {

        internal override Hand[] Hands { get; set; }
        internal double Balance;

        Strategy Strategy;
        public Player(Strategy strategy)
        {
            Hands = new Hand[2];
            Hands[0] = new Hand();
            Hands[1] = new Hand();
            Strategy = strategy;
        }

        internal override void ClearHand()
        {
            base.ClearHand();
            Hands[1].InPlay = false; //split hand not in play yet
        }

        internal override void PlayHands(Game game)
        {
            var possibleActions = Game.GetPossibleActions(Hands[0]);
            Action? actionToTake = null;
            if (possibleActions.canHit || possibleActions.canDouble || possibleActions.canSplit)
            {
                actionToTake = Strategy.GetAction(Hands[0].GetValue().ToString(), game.DealerUpCard.GetValue().ToString())
                    .Match(
                        Some: x => x,
                        None: null
                    );
            }

            if (actionToTake == Action.Split && possibleActions.canSplit)
            {
                //split the hand into two
                var Card = Hands[0].Cards[1];
                Hands[0].Cards.RemoveAt(1);
                Hands[1].Cards.Add(Card);
                Hands[1].InPlay = true;

                Hands[0].IsSplit = true;
                Hands[1].IsSplit = true;

                game.Hit(Hands[0]);
                //play hand1
                PlayOneHand(Hands[0], game);


                game.Hit(Hands[1]);
                //play hand2
                PlayOneHand(Hands[1], game);
                return;
            }
            PlayOneHand(Hands[0], game);
        }

        internal void PlayOneHand(Hand hand, Game game)
        {
            PossibleActions possibleActions;
            Action? actionToTake = null;
            do
            {
                possibleActions = Game.GetPossibleActions(hand);
                if (possibleActions.canHit || possibleActions.canDouble || possibleActions.canSplit)
                {
                    actionToTake = Strategy.GetAction(hand.GetValue().ToString(), game.DealerUpCard.GetValue().ToString()).Match(
                        Some: x => x,
                        None: null
                        );
                }

                if (actionToTake == Action.Hit && possibleActions.canHit)
                {
                    game.Hit(hand);
                    continue;
                }

                if (actionToTake == Action.DoubleHit)
                {
                    game.Hit(hand);
                    if (possibleActions.canDouble)
                    {
                        hand.Bet *= 2;
                        return;
                    }
                    continue;
                }

                if (actionToTake == Action.DoubleStand)
                {
                    if (possibleActions.canDouble)
                    {
                        game.Hit(hand);
                        hand.Bet *= 2;
                    }
                    return;
                }

            } while (hand.GetValue().Value < 21 && actionToTake != null && actionToTake != Action.Stand);

            


        }
    }
}

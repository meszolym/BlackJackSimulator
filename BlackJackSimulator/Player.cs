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
        internal double Balance
        {
            get;
            set;
        }

        internal bool InPlay { 
            get
            {
                return Hands.Any(h => h.InPlay);
            }
        }

        Strategy strategy;

        public Player(Strategy strategy)
        {
            Hands = new Hand[2];
            Hands[0] = new Hand();
            Hands[1] = new Hand();
            this.strategy = strategy;
        }

        internal override void ClearHand()
        {
            base.ClearHand();
            Hands[1].InPlay = false;
        }

        internal override void PlayHands(Game game)
        {
            var possibleActions = Game.GetPossibleActions(Hands[0]);
            var actionToTake = strategy.GetAction(Hands[0], game.DealerUpCard);

            if (actionToTake == Action.Split && possibleActions[Action.Split])
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
            var possibleActions = Game.GetPossibleActions(hand);
            var actionToTake = strategy.GetAction(hand, game.DealerUpCard);

            /*
                Hit,
                Stand,
                DoubleHit,
                DoubleStand,
                Split
             */

            //while player can do anything besides standing, and it is not time to stand
            while (possibleActions.Any(x => x.Value == true && x.Key != Action.Stand) 
                && actionToTake != Action.Stand)
            {
                if (actionToTake == Action.Hit && possibleActions[Action.Hit])
                {
                    game.Hit(hand);
                    possibleActions = Game.GetPossibleActions(hand);
                    actionToTake = strategy.GetAction(hand, game.DealerUpCard);
                    continue;
                }
                
                if (actionToTake == Action.DoubleHit)
                {
                    //if can double, do, otherwise hit;
                    if (possibleActions[Action.DoubleHit])
                    {
                        hand.Bet *= 2;
                    }
                    game.Hit(hand);
                    break;
                }

                if (actionToTake == Action.DoubleStand)
                {
                    //if can double, do, otherwise stand
                    if (possibleActions[Action.DoubleStand])
                    {
                        hand.Bet *= 2;
                        game.Hit(hand);
                    }
                    break;
                }

                throw new Exception("Should never get here");

            }


        }
    }
}

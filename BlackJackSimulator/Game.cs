using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BlackJackSimulator.Models;

namespace BlackJackSimulator
{
    internal class Game
    {
        public const int DefaultNumberOfPlayers = 1;
        public const int DefaultNumberOfDecks = 6;

        Dealer dealer;
        internal Player[] players;
        Shoe shoe;

        public Game(int numberOfPlayers, int numberOfDecks, Strategy strategy, Random random)
        {
            dealer = new Dealer();
            players = new Player[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i] = new Player(strategy);
            }
            shoe = new Shoe(numberOfDecks, random);
        }

        internal Card DealerUpCard
        {
            get
            {
                if (dealer.Hand.Cards.Count == 0)
                {
                    throw new AccessViolationException("Dealer has no upcard at this time!");
                }
                return dealer.Hand.Cards[0];
            }
        }
        public void PlayRound()
        {
            clearTable();
            dealHands();
            //peek at dealer's hole card for blackjack if the upcard is an ace
            if (DealerUpCard == Card.Ace)
            {
                if (CheckAndHandleDealerBlackJack())
                {
                    return;
                }
            }

            //play each player's hand
            foreach (var p in players)
            {
                p.PlayHands(this);
                foreach (var hand in p.Hands.Where(h => h.InPlay))
                {
                    if (hand.GetValue().Value > 21)
                    {
                        //player busts
                        p.Balance -= hand.Bet;
                        hand.InPlay = false;
                    }
                }
            }

            //check for dealer blackjack
            if (CheckAndHandleDealerBlackJack())
            {
                return;
            }

            //play dealer's hand
            dealer.PlayHands(this);

            //check for dealer bust
            if (dealer.Hand.GetValue().Value > 21)
            {
                //dealer busts, all players still in game win
                foreach (var player in players)
                {
                    foreach (var hand in player.Hands.Where(h => h.InPlay))
                    {
                        player.Balance += hand.Bet;
                        hand.InPlay = false;
                    }
                }
                return;
            }

            //compare dealer's hand to each player's hand
            foreach (var player in players)
            {
                foreach (var hand in player.Hands.Where(h => h.InPlay))
                {
                    var playerValue = hand.GetValue();
                    var dealerValue = dealer.Hand.GetValue();
                    if (playerValue > dealerValue)
                    {
                        player.Balance += hand.Bet;
                        if (playerValue.IsBlackJack)
                        {
                            //player has blackjack, pay 3:2
                            player.Balance += hand.Bet * 0.5;
                        }
                    }
                    else if (playerValue < dealerValue)
                    {
                        player.Balance -= hand.Bet;
                    }
                    //otherwise it's a push, no change to balance
                }
            }

        }

        private bool CheckAndHandleDealerBlackJack()
        {
            if (dealer.Hand.GetValue().IsBlackJack)
            {
                //dealer has blackjack, all players lose unless they also have blackjack
                foreach (var player in players)
                {
                    foreach (var hand in player.Hands.Where(h => h.InPlay))
                    {
                        if (!hand.GetValue().IsBlackJack)
                        {
                            player.Balance -= hand.Bet;
                        }
                        hand.InPlay = false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deals the hands (2 cards) to the players and dealer
        /// </summary>
        private void dealHands()
        {
            if (shoe.ShouldShuffle)
            {
                shoe.Reset();
            }
            for (int i = 0; i < 2; i++)
            {
                foreach (var player in players)
                {
                    Hit(player.Hands[0]);
                }
                Hit(dealer.Hand);
            }
        }


        /// <summary>
        /// Clears the table of all cards and bets
        /// </summary>
        void clearTable()
        {
            dealer.ClearHand();
            foreach (var player in players)
            {
                player.ClearHand();
            }
        }

        internal struct PossibleActions
        {
            internal bool canHit;
            internal bool canDouble;
            internal bool canSplit;
            //stand is always an option.
        }

        /// <summary>
        /// Gets the possible actions on a hand
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        internal static PossibleActions GetPossibleActions(Hand hand)
        {
            var possibleActions = new PossibleActions();

            if (hand.GetValue().Value >= 21)
            {
                //busted hands & 21 can only stand
                return possibleActions;
            }

            if (hand.Cards.Count == 2)
            {
                //check for split aces
                if (hand.Cards[0] == Card.Ace && hand.IsSplit)
                {
                    //no actions can be taken on split aces
                    return possibleActions;
                }

                //double any 2 cards + DAS allowed
                possibleActions.canDouble = true; 

                if (hand.Cards[0] == hand.Cards[1] && !hand.IsSplit)
                {
                    //split any identical pair, if not already split
                    possibleActions.canSplit = true;
                }
            }

            //if the hand has more than 2 cards, only hit and stand are possible
            possibleActions.canHit = true;
            return possibleActions;

        }

        /// <summary>
        /// Hits the hand with a card from the shoe
        /// </summary>
        /// <param name="hand"></param>
        public void Hit(Hand hand) 
        {
            hand.Cards.Add(shoe.DrawCard());
        }


    }
}

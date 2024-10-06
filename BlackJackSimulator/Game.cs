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
                p.PlayHands(shoe,DealerUpCard);
                foreach (var hand in p.Hands.Where(h => h.State == HandState.InPlay))
                {
                    if (hand.GetValue().Value > 21)
                    {
                        //player busts
                        p.Balance -= hand.Bet;
                        hand.Lose();
                    }
                }
            }

            //check for dealer blackjack
            if (CheckAndHandleDealerBlackJack())
            {
                return;
            }

            //play dealer's hand
            dealer.PlayHands(shoe,DealerUpCard);

            //check for dealer bust
            if (dealer.Hand.GetValue().Value > 21)
            {
                //dealer busts, all players still in game win
                foreach (var player in players)
                {
                    foreach (var hand in player.Hands.Where(h => h.State == HandState.InPlay))
                    {
                        player.Balance += hand.Bet;
                        hand.Win();
                    }
                }
                return;
            }

            //compare dealer's hand to each player's hand
            foreach (var player in players)
            {
                foreach (var hand in player.Hands.Where(h => h.State == HandState.InPlay))
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
                        hand.Win();
                    }
                    else if (playerValue < dealerValue)
                    {
                        player.Balance -= hand.Bet;
                        hand.Lose();
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
                    foreach (var hand in player.Hands.Where(h => h.State == HandState.InPlay))
                    {
                        if (!hand.GetValue().IsBlackJack)
                        {
                            player.Balance -= hand.Bet;
                            hand.Lose();
                            continue;
                        }
                        hand.Win();
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
                    player.Hands.ElementAt(i).Hit(shoe);
                }
                dealer.Hand.Hit(shoe);
            }
        }
    }
}

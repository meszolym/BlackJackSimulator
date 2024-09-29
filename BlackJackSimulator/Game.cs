using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlackJackSimulator
{
    internal class Game
    {
        public const int DefaultNumberOfPlayers = 1;
        public const int DefaultNumberOfDecks = 6;

        internal struct PossibleActions
        {
            internal bool CanHit;
            internal bool CanStand;
            internal bool CanDouble;
            internal bool CanSplit;
        }

        Dealer dealer;
        Player[] players;
        Shoe shoe;

        

        public Game(int numberOfPlayers, int numberOfDecks)
        {
            dealer = new Dealer();
            players = new Player[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i] = new Player();
            }
            shoe = new Shoe(numberOfDecks);
        }

        public void PlayRound()
        {
            clearTable();
            dealHands();
            //peek at dealer's hole card for blackjack if the upcard is an ace
            if (dealer.Hand.Cards[0] == Card.Ace)
            {
                if (CheckAndHandleDealerBlackJack())
                {
                    return;
                }
            }

            //play each player's hand
            foreach (var p in players)
            {
                p.PlayHands(dealer.Hand.Cards[0]);
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
            dealer.PlayHands();

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
                    Hit(player.Hands[i]);
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

        /// <summary>
        /// Gets the possible actions on a hand
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static PossibleActions GetPossibleActions(Hand hand)
        {
            var p = new PossibleActions();
            p.CanStand = true; //always an option

            //check for blackjack or 21
            if (hand.GetValue().Value == 21)
            {
                //no actions can be taken on blackjack
                p.CanHit = false;
                p.CanDouble = false;
                p.CanSplit = false;
                return p;
            }

            if (hand.Cards.Count == 2)
            {
                //check for split aces
                if (hand.Cards[0] == Card.Ace && hand.IsSplit)
                {
                    //no actions can be taken on split aces
                    p.CanHit = false;
                    p.CanDouble = false;
                    p.CanSplit = false;
                    return p;
                }

                //double/hit any 2 cards
                p.CanDouble = true;
                p.CanHit = true;

                if (hand.Cards[0] == hand.Cards[1] && !hand.IsSplit)
                {
                    //split any identical pair, if not already split
                    p.CanSplit = true; 
                }
                return p;
            }

            //if the hand has more than 2 cards, only hit and stand are possible
            p.CanHit = true;
            return p;

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

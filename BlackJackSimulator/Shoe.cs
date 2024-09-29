using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator
{
    public enum Card
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }
    internal static class CardExtension
    {
        public static int GetValue(this Card card)
        {
            return HandValue.GetValue(card);
        }
    }


    internal class Shoe
    {
        Dictionary<Card, int> Cards = new();

        int NumberOfDecks; //number of decks in the shoe
        int ShuffleCard; //when the number of cards left in the shoe is less than or equal to this number, shuffle is imminent

        Random random = new Random();

        public Shoe(int numberOfDecks)
        {
            NumberOfDecks = numberOfDecks;

            foreach (Card card in Enum.GetValues(typeof(Card)))
            {
                Cards[card] = 4 * NumberOfDecks; //4 of each card in each deck
            }
            ShuffleCard = Cards.Values.Sum() - (40 + random.Next(-10, 10));
            //ShuffleCard is set to the number of cards in the shoe minus 40 plus a random number between -10 and 10
        }

        public Card DrawCard()
        {
            int totalCards = Cards.Values.Sum();
            //total number of cards in the shoe
            int num = random.Next(1,totalCards + 1);
            //each card is just as likely to be drawn as any other card

            //this way the probabilities are weighted by the number of each card in the shoe
            foreach (Card card in Cards.Keys)
            {
                //if the random number is less than or equal to the number of cards of this type, draw this card
                if (num <= Cards[card])
                {
                    Cards[card]--;
                    return card;
                }
                else
                {
                    //otherwise, subtract the number of cards of this type from the random number
                    num -= Cards[card];
                }
            }
            throw new Exception("Should never get here");

        }

        public bool ShouldShuffle
        {
            get
            {
                return Cards.Values.Sum() <= ShuffleCard;
            }
        }

        public void Reset()
        {
            //resets the shoe to the original state
            foreach (Card card in Enum.GetValues(typeof(Card)))
            {
                Cards[card] = 4 * NumberOfDecks;
            }
            ShuffleCard = Cards.Values.Sum() - (40 + random.Next(-10, 10));
        }
    }
}
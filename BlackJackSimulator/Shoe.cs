using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models;

namespace BlackJackSimulator
{
    public class Shoe
    {
        Dictionary<Card, int> Cards = new();

        int NumberOfDecks; //number of decks in the shoe
        int ShuffleCard; //when the number of cards left in the shoe is less than or equal to this number, shuffle is imminent

        int cardsInPlay;

        Random random;

        public Shoe(int numberOfDecks, Random random)
        {
            NumberOfDecks = numberOfDecks;
            this.random = random;

            Reset();
        }

        public Card DrawCard()
        {
            //total number of cards in the shoe
            int num = random.Next(1,cardsInPlay + 1);
            //each card is just as likely to be drawn as any other card

            //this way the probabilities are weighted by the number of each card in the shoe
            foreach (Card card in Cards.Keys)
            {
                //find the right card
                if (num > Cards[card])
                {
                    num -= Cards[card];
                    continue;
                }

                //if the random number is less than or equal to the number of cards of this type, draw this card
                Cards[card]--;
                cardsInPlay--;
                return card;
            }

            throw new Exception("Should never get here");

        }

        public bool ShouldShuffle
        {
            get
            {
                return cardsInPlay <= ShuffleCard;
            }
        }

        public void Reset()
        {
            //resets the shoe to the original state
            cardsInPlay = NumberOfDecks * 52;
            foreach (Card card in Enum.GetValues(typeof(Card)))
            {
                Cards[card] = 4 * NumberOfDecks;
            }
            ShuffleCard = cardsInPlay - (40 + random.Next(-10, 10));
        }
    }
}
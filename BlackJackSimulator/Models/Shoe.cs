using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models.Enums;
using LanguageExt;

namespace BlackJackSimulator.Models
{
    public class Shoe
    {
        private const int DeckSize = 52;

        private readonly Dictionary<Card, int> _cards = new();

        private readonly int _numberOfDecks; //number of decks in the shoe

        private int _shuffleCard; //when the number of cards left in the shoe is less than or equal to this number, shuffle is necessery before next deal

        private int _cardsInPlay; //number of cards in the shoe

        private readonly int _shuffleCardPlacementBase;

        private readonly int _shuffleCardPlacementVariance;

        private readonly Random _random;

        public Shoe(int numberOfDecks, Random random, int shuffleCardPlacementBase, int shuffleCardPlacementVariance)
        {
            _numberOfDecks = numberOfDecks;
            _random = random;
            _shuffleCardPlacementBase = shuffleCardPlacementBase;
            _shuffleCardPlacementVariance = shuffleCardPlacementVariance;

            Reset();
        }

        public Option<Card> DrawCard()
        {
            //total number of cards in the shoe
            var num = _random.Next(1, _cardsInPlay + 1);
            //each card is just as likely to be drawn as any other card

            //this way the probabilities are weighted by the number of each card in the shoe
            foreach (var card in _cards.Keys)
            {
                //find the right card
                if (num > _cards[card])
                {
                    num -= _cards[card];
                    continue;
                }

                //if the random number is less than or equal to the number of cards of this type, draw this card
                _cards[card]--;
                _cardsInPlay--;
                return card;
            }
            return Option<Card>.None;
        }

        public bool ShouldShuffle => _cardsInPlay <= _shuffleCard;

        public void Reset()
        {
            //resets the shoe to the original state
            _cardsInPlay = _numberOfDecks * DeckSize;

            ((Card[])Enum.GetValues(typeof(Card)))
                .Iter(card => _cards[card] = 4 * _numberOfDecks);

            _shuffleCard = _cardsInPlay - (_shuffleCardPlacementBase + _random.Next(-1 * _shuffleCardPlacementVariance, _shuffleCardPlacementVariance));
        }
    }
}
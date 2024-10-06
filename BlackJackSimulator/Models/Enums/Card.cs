using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator.Models.Enums
{
    public enum Card
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }


    public static class CardExtensions
    {
        public static int GetValue(this Card card) => card switch
        {
            Card.Ace => 11,
            Card.Two => 2,
            Card.Three => 3,
            Card.Four => 4,
            Card.Five => 5,
            Card.Six => 6,
            Card.Seven => 7,
            Card.Eight => 8,
            Card.Nine => 9,
            Card.Ten => 10,
            Card.Jack => 10,
            Card.Queen => 10,
            Card.King => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(card), card, null)
        };
    }
}

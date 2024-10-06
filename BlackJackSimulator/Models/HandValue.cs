using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BlackJackSimulator.Models.Enums;

namespace BlackJackSimulator.Models
{
    public class HandValue
    {
        public int Value { get; private set; }
        public bool IsSoft { get; private set; }
        public bool IsBlackJack { get; private set; }
        public bool IsPair { get; private set; }

        /// <summary>
        /// Gets the value of a hand
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static HandValue GetValue(Hand hand)
        {
            HandValue h = new();

            hand.Cards.Iter(card =>
            {
                h.Value += card.GetValue();
                if (card == Card.Ace) h.Value -= 10; //count aces as 1 for now
            });

            //if the hand has an ace and can be counted as 11 without busting, it's soft and the value is increased by 10
            //remember that the value of ace was counted as 1 before
            if (hand.Cards.Count(x => x == Card.Ace) > 0 && h.Value <= 11)
            {
                h.Value += 10;
                h.IsSoft = true;
            }

            if (hand.Cards.Count == 2 && h.Value == 21)
            {
                h.IsBlackJack = true;
                h.IsSoft = false;
            }

            if (hand.Cards.Count == 2 && hand.Cards[0] == hand.Cards[1] && !hand.IsSplit)
            {
                h.IsPair = true;
            }

            return h;
        }

        public override string ToString() => this switch
        {
            { IsBlackJack: true } => "BJ",
            { IsSoft: true, IsPair: true } => "P11", //Pair of Aces
            { IsSoft: true } => $"S{Value}",
            { IsPair: true } => $"P{Value / 2}",
            _ => Value.ToString()
        };

        public static bool operator <(HandValue left, HandValue right)
            => left.Value < right.Value || !left.IsBlackJack && right.IsBlackJack;


        public static bool operator >(HandValue left, HandValue right)
            => left.Value > right.Value || left.IsBlackJack && !right.IsBlackJack;

        public static bool operator ==(HandValue left, HandValue right)
            => !(left < right || left > right);

        public static bool operator !=(HandValue left, HandValue right)
            => left < right || left > right;

        protected bool Equals(HandValue other)
            => Value == other.Value
               && IsSoft == other.IsSoft
               && IsBlackJack == other.IsBlackJack
               && IsPair == other.IsPair;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HandValue)obj);
        }

        public override int GetHashCode()
            => ToString().GetHashCode();

    }
}

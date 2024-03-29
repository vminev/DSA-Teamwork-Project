﻿namespace TexasHoldem.AI.TuringExtended.Helpers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Logic;
    using Logic.Cards;
    using Logic.Helpers;

    public class BestHandExtension : IComparable<BestHandExtension>
    {
        internal BestHandExtension(HandRankType rankType, ICollection<CardType> cards)
        {
            //if (cards.Count != 3 || cards.Count != 4)
            //{
            //    throw new ArgumentException("Cards collection should contains exactly 5 elements", nameof(cards));
            //}

            this.Cards = cards;
            this.RankType = rankType;
        }

        // When comparing or ranking cards, the suit doesn't matter
        public ICollection<CardType> Cards
        {
            get;
        }

        public HandRankType RankType
        {
            get;
        }

        public int CompareTo(BestHandExtension other)
        {
            if (this.RankType > other.RankType)
            {
                return 1;
            }

            if (this.RankType < other.RankType)
            {
                return -1;
            }

            switch (this.RankType)
            {
                case HandRankType.HighCard:
                    return CompareTwoHandsWithHighCard(this.Cards, other.Cards);
                case HandRankType.Pair:
                    return CompareTwoHandsWithPair(this.Cards, other.Cards);
                case HandRankType.TwoPairs:
                    return CompareTwoHandsWithTwoPairs(this.Cards, other.Cards);
                case HandRankType.ThreeOfAKind:
                    return CompareTwoHandsWithThreeOfAKind(this.Cards, other.Cards);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int CompareTwoHandsWithHighCard(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstSorted = firstHand.OrderByDescending(x => x).ToList();
            var secondSorted = secondHand.OrderByDescending(x => x).ToList();
            var cardsToCompare = Math.Min(firstHand.Count, secondHand.Count);
            for (var i = 0; i < cardsToCompare; i++)
            {
                if (firstSorted[i] > secondSorted[i])
                {
                    return 1;
                }

                if (firstSorted[i] < secondSorted[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        private static int CompareTwoHandsWithPair(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstPairType = firstHand.GroupBy(x => x).First(x => x.Count() >= 2);
            var secondPairType = secondHand.GroupBy(x => x).First(x => x.Count() >= 2);

            if (firstPairType.Key > secondPairType.Key)
            {
                return 1;
            }

            if (firstPairType.Key < secondPairType.Key)
            {
                return -1;
            }

            // Equal pair => compare high card
            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }

        private static int CompareTwoHandsWithTwoPairs(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstPairType = firstHand.GroupBy(x => x).Where(x => x.Count() == 2).OrderByDescending(x => x.Key).ToList();
            var secondPairType = secondHand.GroupBy(x => x).Where(x => x.Count() == 2).OrderByDescending(x => x.Key).ToList();

            for (int i = 0; i < firstPairType.Count; i++)
            {
                if (firstPairType[i].Key > secondPairType[i].Key)
                {
                    return 1;
                }

                if (secondPairType[i].Key > firstPairType[i].Key)
                {
                    return -1;
                }
            }

            // Equal pairs => compare high card
            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }

        private static int CompareTwoHandsWithThreeOfAKind(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstThreeOfAKindType = firstHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
            var secondThreeOfAKindType = secondHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
            if (firstThreeOfAKindType.Key > secondThreeOfAKindType.Key)
            {
                return 1;
            }

            if (secondThreeOfAKindType.Key > firstThreeOfAKindType.Key)
            {
                return -1;
            }

            // Equal triples => compare high card
            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }

        private static int CompareTwoHandsWithFourOfAKind(
         ICollection<CardType> firstHand,
         ICollection<CardType> secondHand)
        {
            var firstFourOfAKingType = firstHand.GroupBy(x => x).First(x => x.Count() == 4);
            var secondFourOfAKindType = secondHand.GroupBy(x => x).First(x => x.Count() == 4);

            if (firstFourOfAKingType.Key > secondFourOfAKindType.Key)
            {
                return 1;
            }

            if (firstFourOfAKingType.Key < secondFourOfAKindType.Key)
            {
                return -1;
            }

            // Equal pair => compare high card
            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }
    }
}

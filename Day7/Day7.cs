using System.Diagnostics;

namespace Day7
{
    internal class Day7
    {
        public static List<Hand> Hands = new();

        static void Main()
        {
            bool useSampleData = false;

            if (useSampleData) LoadSampleData();
            else LoadData();

            Console.WriteLine("Part 1");
            var timer = Stopwatch.StartNew();
            Part1();
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}ms");

            if (useSampleData) LoadSampleData(true);
            else LoadData(true);

            Console.WriteLine("\r\nPart 2");
            timer.Restart();
            //248,960,113 - too high
            Part2();
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}ms");

            Console.ReadLine();
        }

        public static void Part1()
        {
            var answer = Hands.OrderBy(x => x).Zip(Enumerable.Range(1, Hands.Count), (hand, rank) => hand.Bid * rank).Sum();
            Console.WriteLine("{0:n0}", answer);
        }

        public static void Part2()
        {
            var orderedHands = Hands.OrderBy(x => x).ToList();
            var hasJoker = orderedHands.Where(x => x.Cards.Contains('J')).ToList();
            var answer = orderedHands.Zip(Enumerable.Range(1, Hands.Count), (hand, rank) => hand.Bid * rank).Sum();
            Console.WriteLine("{0:n0}", answer);
        }

        public static void LoadSampleData(bool part2 = false)
        {
            var input = "32T3K 765|T55J5 684|KK677 28|KTJJT 220|QQQJA 483".Split("|", StringSplitOptions.TrimEntries);

            //Alternate sample data results should be 1343 and 1369 for each part
            //var input = "AAAAA 2|22222 3|AAAAK 5|22223 7|AAAKK 11|22233 13|AAAKQ 17|22234 19|AAKKQ 23|22334 29|AAKQJ 31|22345 37|AKQJT 41|23456 43".Split("|", StringSplitOptions.TrimEntries);
            if (part2)
            {
                //Reassign point values for Jack's to become Jokers(wildcards)
                Hand.CardValues['J'] = 1;
            }

            Hands = input.Select(x => x.Split(" ", StringSplitOptions.TrimEntries))
                .Select(x => new Hand(x[0], int.Parse(x[1]))).ToList();
        }

        public static void LoadData(bool part2 = false)
        {
            var input = File.ReadAllLines("Input.txt");
            if (part2)
            {
                //Reassign point values for Jack's to become Jokers(wildcards)
                Hand.CardValues['J'] = 1;
            }

            Hands = input.Select(x => x.Split(" ", StringSplitOptions.TrimEntries))
                .Select(x => new Hand(x[0], int.Parse(x[1]))).ToList();
        }
    }

    // [DebuggerDisplay("Cards: {Cards}, Bid: {Bid}, Type: {Type}, UpgradedCards: {UpgradedCards}")]
    [DebuggerDisplay("{Cards} | {UpgradedCards} | {Type}")]
    public class Hand : IComparable<Hand>
    {
        public static Dictionary<char, int> CardValues = new()
        {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'J', 11 },
            { 'T', 10 },
            { '9', 9 },
            { '8', 8 },
            { '7', 7 },
            { '6', 6 },
            { '5', 5 },
            { '4', 4 },
            { '3', 3 },
            { '2', 2 }
        };

        private readonly bool _usingReplacements = CardValues['J'] == 1;

        public Hand(string cards, int bid)
        {
            Cards = cards;
            Bid = bid;
            if (_usingReplacements)
            {
                UpgradedCards = BuildReplacementCard();
            }
            Type = GetType();
        }

        public string Cards { get; }
        private string UpgradedCards { get; }

        public long Bid { get; }

        public Type Type { get; }

        public int CompareTo(Hand other)
        {
            var valueToCompare = Cards;

            if(Type > other.Type) return 1;
            if (Type < other.Type) return -1;
            if (Type == other.Type)
            {
                for (int i = 0; i < valueToCompare.Length; i++)
                {
                    var thisCard = CardValues[valueToCompare[i]];
                    var otherCard = CardValues[other.Cards[i]];

                    var value = thisCard.CompareTo(otherCard);
                    if (value == 0) continue;
                    return value;
                }
            }
            return 0;
        }

        private Type GetType()
        {
            var cards = _usingReplacements ? UpgradedCards : Cards;
            var cardCounts = cards.GroupBy(x => x).ToDictionary(k => k, v => v.Count());

            if (cardCounts.Any(x => x.Value == 5))
                return Type.Five;
            if (cardCounts.Any(x => x.Value == 4))
                return Type.Four;
            if (cardCounts.Any(x => x.Value == 3) && cardCounts.Any(x => x.Value == 2))
                return Type.Full;
            if (cardCounts.Any(x => x.Value == 3))
                return Type.Three;
            if (cardCounts.Select(x => x.Value).OrderBy(x => x).SequenceEqual(new []{1,2,2}))
                return Type.TwoPair;
            if (cardCounts.Any(x => x.Value == 2))
                return Type.Pair;
            return Type.High;
        }

        private string BuildReplacementCard()
        {
            if (Cards.Any(x => x == 'J'))
            {
                var jokerCount = Cards.Count(x => x == 'J');

                if (jokerCount == 5) return "AAAAA";

                
                var cardCounts = Cards.Where(x => x != 'J').GroupBy(x => x).ToDictionary(k => k.Key, v => v.Count());

                var highGroupCount = cardCounts.MaxBy(x => x.Value).Value;

                var numberOfGroups = cardCounts.Where(x => x.Value == highGroupCount);
                if (numberOfGroups.Count() == 1)
                {
                    return Cards.Replace('J', numberOfGroups.First().Key);
                }
                else
                {
                    var firstGroup = numberOfGroups.First().Key;
                    var lastGroup = numberOfGroups.Last().Key;
                    var replacement = CardValues[firstGroup] > CardValues[lastGroup] ? firstGroup : lastGroup;
                    return Cards.Replace('J', replacement);
                }
            }

            return Cards;
        }
    }

    public enum Type
    {
        Five = 6,
        Four = 5,
        Full = 4,
        Three = 3,
        TwoPair = 2,
        Pair = 1,
        High = 0
    }
}

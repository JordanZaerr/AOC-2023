using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day4
{
    internal class Day4
    {
        static void Main()
        {
            var timer = Stopwatch.StartNew();
            Part1();
            timer.Stop();
            Console.WriteLine($"Part 1: {timer.ElapsedMilliseconds}ms");
            timer.Restart();
            Part2();
            Console.WriteLine($"Part 2: {timer.ElapsedMilliseconds}ms");
        }

        public static void Part1()
        {
            // var input = new[]
            // {
            //     "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
            //     "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
            //     "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
            //     "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
            //     "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
            //     "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
            // };
            var input = File.ReadAllLines("Input.txt");

            var cards = input.Select(ParseCard).ToList();
            var values = cards.Select(x => x.GetCardPoints()).ToList();
            var answer = values.Sum();
            Console.WriteLine(answer);
        }

        public static void Part2()
        {
            // var input = new[]
            // {
            //     "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
            //     "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
            //     "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
            //     "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
            //     "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
            //     "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
            // };
            var input = File.ReadAllLines("Input.txt");

            var cards = input.Select(ParseCard).ToList();


            var cardLookup = cards.ToDictionary(k => k.Id);
            var cardCounts = Enumerable.Range(1, cards.Count).ToDictionary(k => k, v => 1L);
            var winningValues = cards.ToDictionary(k => k.Id, v => v.GetWinningCount());

            cards.ForEach(x => ProcessCard(x, cardLookup, cardCounts, winningValues));

            var answer = cardCounts.Values.Sum();
            Console.WriteLine(answer);
            //5472514 - too low
        }

        public static void ProcessCard(Card card, Dictionary<int, Card> cards, Dictionary<int, long> cardCounts, Dictionary<int, int> winningCards)
        {
            var winnerCount = winningCards[card.Id];

            if (winnerCount == 0) return;

            var winningNumbers = Enumerable.Range(card.Id + 1, winnerCount).Where(x => x <= cards.Count);
            
            //Update card totals
            foreach (var number in winningNumbers)
            {
                cardCounts[number]++;
                ProcessCard(cards[number], cards, cardCounts, winningCards);
            }
        }



        private static Card ParseCard(string input)
        {
            var cardRegex = @"Card\s+(?<Card>\d+):";
            var winningRegex = @":([\d\s]+)";
            var numbersRegex = @"\|([\d\s]+)";

            var cardId = Regex.Match(input, cardRegex).Groups["Card"].Value;
            var winning = Regex.Match(input, winningRegex).Groups[1].Value.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var numbers = Regex.Match(input, numbersRegex).Groups[1].Value.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var card = new Card
            {
                Id = int.Parse(cardId),
                Numbers = numbers.Select(int.Parse).ToList(),
                WinningNumbers = winning.Select(int.Parse).ToList()
            };

            return card;
        }

    }

    [DebuggerDisplay("Id: {Id}, Winning: {WinningNumbers.Count}, Numbers: {Numbers.Count}")]
    public class Card
    {
        public int Id { get; set; }
        public List<int> WinningNumbers { get; set; }
        public List<int> Numbers { get; set; }

        public int GetCardPoints()
        {
            var matchingNumbers = Numbers.Count(x => WinningNumbers.Contains(x));
            return matchingNumbers <= 1 ? matchingNumbers : (int)Math.Pow(2, matchingNumbers - 1);
        }

        public int GetWinningCount()
        {
            return Numbers.Count(x => WinningNumbers.Contains(x));
        }
    }
}

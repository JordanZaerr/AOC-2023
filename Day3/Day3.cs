using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day3
{
    internal class Day3
    {
        static void Main()
        {
            //Part1();
            Part2();
        }

        public static void Part1()
        {
            // var input = new[]
            // {
            //     "467..114..",
            //     "...*......",
            //     "..35..633.",
            //     "......#...",
            //     "617*......",
            //     ".....+.58.",
            //     "..592.....",
            //     "......755.",
            //     "...$.*....",
            //     ".664.598.."
            // };
            var input = File.ReadAllLines("Input.txt");

            var numberRegex = @"\d+";
            var symbolRegex = @"[^\.\d]";

            var numbers = new List<Position>();
            var symbols = new List<Position>();

            var line = 0;
            numbers.AddRange(input.SelectMany(x => GetPositions(x, numberRegex, line++)));
            line = 0;
            symbols.AddRange(input.SelectMany(x => GetPositions(x, symbolRegex, line++)));

            var answer = numbers.Where(x => HasSymbolNearby(x, symbols)).Sum(x => int.Parse(x.Value));
            Console.WriteLine(answer);
        }

        public static void Part2()
        {
            // var input = new[]
            // {
            //     "467..114..",
            //     "...*......",
            //     "..35..633.",
            //     "......#...",
            //     "617*......",
            //     ".....+.58.",
            //     "..592.....",
            //     "......755.",
            //     "...$.*....",
            //     ".664.598.."
            // };
            var input = File.ReadAllLines("Input.txt");

            var numberRegex = @"\d+";
            var symbolRegex = @"[^\.\d]";

            var numbers = new List<Position>();
            var symbols = new List<Position>();

            var line = 0;
            numbers.AddRange(input.SelectMany(x => GetPositions(x, numberRegex, line++)));
            line = 0;
            symbols.AddRange(input.SelectMany(x => GetPositions(x, symbolRegex, line++)));

            var answer = symbols.Where(x => x.Value == "*").Select(x => FindGearRatios(x, numbers)).Sum();
            Console.WriteLine(answer);

        }

        public static bool HasSymbolNearby(Position number, List<Position> symbols)
        {
            var symbolsInNearbyLines = symbols.Where(x => x.Line == number.Line || x.Line == number.Line - 1 || x.Line == number.Line + 1);
            var hasAdjacentSymbols = symbolsInNearbyLines.Any(s => Between(s.Start, number.Start - 1, number.End + 1));
            return hasAdjacentSymbols;
        }

        public static int FindGearRatios(Position symbol, List<Position> numbers)
        {
            var numbersInNearbyLines = numbers.Where(x => x.Line == symbol.Line || x.Line == symbol.Line - 1 || x.Line == symbol.Line + 1);
            var adjacentNumbers = numbersInNearbyLines.Where(n => Between(symbol.Start, n.Start - 1, n.End + 1)).ToList();
            if (adjacentNumbers.Count == 2)
            {
                return int.Parse(adjacentNumbers[0].Value) * int.Parse(adjacentNumbers[1].Value);
            }
            return 0;
        }


        public static bool Between(int num, int lower, int upper)
        {
            return lower <= num && num <= upper;
        }

        public static IEnumerable<Position> GetPositions(string input, string regex, int line)
        {
            var symbolMatches = Regex.Matches(input, regex);
            return symbolMatches.SelectMany(x => x.Captures.Select(c => new Position
            {
                Line = line,
                Start = c.Index+1,
                End = c.Index + c.Length,
                Value = c.Value
            }));
        }

        [DebuggerDisplay("Line: {Line}, Start: {Start}, End: {End}, Value: {Value}")]
        public class Position
        {
            public int Line { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
            public string Value { get; set; }
        }
    }
}

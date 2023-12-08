using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day2
{
    internal class Day2
    {
        static void Main()
        {
            //Part1();
            Part2();
        }

        private static void Part1()
        {
            // var input = new[]
            // {
            //     "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
            //     "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
            //     "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
            //     "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
            //     "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
            // };
            var input = File.ReadAllLines("Input.txt");

            var bag = new Set
            {
                Red = 12,
                Green = 13,
                Blue = 14
            };
            var games = input.Select(ParseGame);
            var answer = games.Where(x => x.IsPossible(bag)).Sum(x => x.Id);
            Console.WriteLine(answer);
        }

        private static void Part2()
        {
            // var input = new[]
            // {
            //     "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
            //     "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
            //     "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
            //     "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
            //     "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
            // };

            var input = File.ReadAllLines("Input.txt");

            var games = input.Select(ParseGame);
            var answer = games.Sum(x => x.GetMinimumCubeCountPower());
            Console.WriteLine(answer);
        }

        private static Game ParseGame(string input)
        {
            var gameRegex = @"Game (?<Game>\d+)";
            var setRegex = @"Game \d+:(?<Set>(\s\d+ \w+,?)+;?)+";
            var numColorRegex = @"(?<Num>\d+) (?<Color>\w+)";

            var gameId = Regex.Match(input, gameRegex).Groups["Game"].Value;
            var match = Regex.Match(input, setRegex);

            var setStrings = match.Groups["Set"].Captures.Select(x => x.Value);
            var sets = setStrings.Select(x =>
            {
                var set = Regex.Matches(x, numColorRegex).Select(g => g.Groups);
                var numbers = set.Select(x => x["Num"].Value);
                var colors = set.Select(x => x["Color"].Value);

                return numbers.Zip(colors, (n, c) => (int.Parse(n), c));
            });

            var game = new Game
            {
                Id = int.Parse(gameId)
            };

            foreach (var set in sets)
            {
                var newSet = new Set();
                foreach (var (number, color) in set)
                {
                    if (color == "red")
                    {
                        newSet.Red = number;
                    }
                    else if (color == "green")
                    {
                        newSet.Green = number;
                    }
                    else if (color == "blue")
                    {
                        newSet.Blue = number;
                    }
                    else
                    {
                        Console.WriteLine("Failure");
                    }
                }

                game.Sets.Add(newSet);
            }

            return game;
        }
    }

    [DebuggerDisplay("Game: {Id}, Sets: {Sets.Count}")]
    public class Game
    {
        public int Id { get; set; }
        public List<Set> Sets { get; set; } = new();

        public bool IsPossible(Set set)
        {
            return Sets.All(x => x.Red <= set.Red)
                && Sets.All(x => x.Green <= set.Green)
                && Sets.All(x => x.Blue <= set.Blue);
        }

        public int GetMinimumCubeCountPower()
        {
            var maxRed = Sets.Max(x => x.Red);
            var maxGreen = Sets.Max(x => x.Green);
            var maxBlue = Sets.Max(x => x.Blue);

            return maxRed * maxGreen * maxBlue;
        }
    }

    [DebuggerDisplay("Red: {Red}, Green: {Green}, Blue: {Blue}")]
    public class Set
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
}

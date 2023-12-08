using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day8
{
    internal class Day8
    {
        public static string Instructions;
        public static Dictionary<string, (string Left, string Right)> Steps;

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

            if (useSampleData) LoadSampleData();
            else LoadData();

            Console.WriteLine("\r\nPart 2");
            timer.Restart();
            Part2();
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}ms");

            Console.ReadLine();
        }

        public static void Part1()
        {
            var answer = Navigate("AAA", Instructions, 'Z');
            Console.WriteLine("{0:n0}", answer);
        }

        public static void Part2()
        {
            var startingNodes = Steps.Keys.Where(x => x[2] == 'A').ToList();

            var answers = startingNodes.Select(x => Navigate(x, Instructions, 'Z'));
            var answer = answers.Aggregate(1L, (s, c) => MathFunctions.LowestCommonMultiple(s, c));
            Console.WriteLine("{0:n0}", answer);
        }

        private static int Navigate(string startId, string instructions, char endsWith, int count = 0)
        {
            var currentId = startId;
            foreach (var x in instructions)
            {
                currentId = x == 'L' ? Steps[currentId].Left : Steps[currentId].Right;
                count++;
                if (currentId.EndsWith(endsWith)) return count;
            }
            return Navigate(currentId, instructions, endsWith, count);
        }


        public static void LoadSampleData(bool part2 = false)
        {
            if (part2)
            {
                Instructions = "LR";
                var input = "11A = (11B, XXX)|11B = (XXX, 11Z)|11Z = (11B, XXX)|22A = (22B, XXX)|22B = (22C, 22C)|22C = (22Z, 22Z)|22Z = (22B, 22B)|XXX = (XXX, XXX)".Split("|", StringSplitOptions.TrimEntries);
                Steps = input.Select(x => Regex.Match(x, @"(?<Id>[A-Z0-9]+) = \((?<Left>[A-Z0-9]+), (?<Right>[A-Z0-9]+)\)"))
                    .ToDictionary(k => k.Groups["Id"].Value, v => (v.Groups["Left"].Value, v.Groups["Right"].Value));
            }
            else
            {
                Instructions = "LLR";
                var input = "AAA = (BBB, BBB)|BBB = (AAA, ZZZ)|ZZZ = (ZZZ, ZZZ)".Split("|", StringSplitOptions.TrimEntries);
                Steps = input.Select(x => Regex.Match(x, @"(?<Id>[A-Z]+) = \((?<Left>[A-Z]+), (?<Right>[A-Z]+)\)"))
                    .ToDictionary(k => k.Groups["Id"].Value, v => (v.Groups["Left"].Value, v.Groups["Right"].Value));
            }
        }

        public static void LoadData()
        {
            var input = File.ReadAllLines("Input.txt");
            Instructions = "LRLLLRRLRRLRRLRRLLRRLRRLLRRRLLRRLRRLRRLRRLRLRLLLLLRRLRRLLRLRRRLLRRLRLLLLLLLRRLRLRRRLRRLRRRLRRLLLRRLLRRRLLRRRLRRLRLRRRLRRRLRLRLLRRRLRRRLRRLLRRRLRLRRLLRLLRRLLRRLRRRLRRLRLRRLLRRRLRRRLRRRLRLRLRLRRRLLRRRLRLRRLLRRLRRLRRLRLLRRLLRRRLRRRLRRLRRLRLLRRLRLRRLRRRLRRRLRRLRLRRRLRRRLRLLLRRLRLLRRRR";
            Steps = input.Select(x => Regex.Match(x, @"(?<Id>[A-Z]+) = \((?<Left>[A-Z]+), (?<Right>[A-Z]+)\)"))
                .ToDictionary(k => k.Groups["Id"].Value, v => (v.Groups["Left"].Value, v.Groups["Right"].Value));
        }
    }

    //I have a ProjectEuler(projecteuler.net) codebase that I copy/pasted these from
    public static class MathFunctions
    {
        //http://en.wikipedia.org/wiki/Euclidean_algorithm
        public static long GreatestCommonFactor(long a, long b)
        {
            while (b != 0)
            {
                var t = b;
                b = a % t;
                a = t;
            }
            return a;
        }

        //http://en.wikipedia.org/wiki/Least_common_multiple#Reduction_by_the_greatest_common_divisor
        public static long LowestCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }
    }
}

using System.Diagnostics;

namespace Day9
{
    internal class Day9
    {
        public static string[] Input = [];
        public static List<Range> Ranges;

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

            Console.WriteLine("\r\nPart 2");
            timer.Restart();
            Part2();
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}ms");

            Console.ReadLine();
        }

        public static void Part1()
        {
            var predictions = Ranges.Select(x => x.PredictNextReading(Direction.Forward));
            var answer = predictions.Sum();
            Console.WriteLine("{0:n0}", answer);
        }

        public static void Part2()
        {
            var predictions = Ranges.Select(x => x.PredictNextReading(Direction.Backward));
            var answer = predictions.Sum();
            Console.WriteLine("{0:n0}", answer);
        }

        public static void LoadSampleData()
        {
            Input = new[]
            {
                // "-3 -8 -13 -18", //-23
                "0 3 6 9 12 15", //18
                "1 3 6 10 15 21", //28
                "10 13 16 21 30 45" //68
            };
            Ranges = Input.Select(x => new Range { Values = x.Split(" ", StringSplitOptions.TrimEntries).Select(int.Parse).ToList() }).ToList();
        }

        public static void LoadData()
        {
            Input = File.ReadAllLines("Input.txt");
            Ranges = Input.Select(x => new Range { Values = x.Split(" ", StringSplitOptions.TrimEntries).Select(int.Parse).ToList() }).ToList();
        }
    }

    [DebuggerDisplay("{string.Join(\",\", Values)}")]
    public class Range
    {
        public List<int> Values { get; set; } = [];

        public Range GetNextRange(Direction direction)
        {
            var range = new Range();
            foreach (var slide in Values.Slide(2))
            {
                if(direction == Direction.Backward) slide.Reverse();
                range.Values.Add(slide[1] - slide[0]);
            }
            return range;
        }

        public int PredictNextReading(Direction direction)
        { 
            var predictionRanges = new List<Range> { this };
            var currentRange = this;
            while (currentRange.Values.Any(x => x != 0))
            {
                currentRange = currentRange.GetNextRange(direction);
                predictionRanges.Add(currentRange);
            }

            var nextValue = predictionRanges.Select(x => direction == Direction.Forward ? x.Values.Last() : x.Values.First()).Sum();

            return nextValue;
        }
    }

    public enum Direction
    { 
        Forward,
        Backward
    }

    //More of my ProjectEuler code...small tweaks
    public static class SlidingExtensions
    {
        //Only grabs full windows.
        //1,2,3,4 window size 2 => [1,2], [2,3], [3,4]
        public static IEnumerable<List<T>> Slide<T>(this IEnumerable<T> src, int window)
        {
            int current = 0;
            var source = src.ToList();
            while (current + window <= source.Count)
            {
                yield return source.Skip(current).Take(window).ToList();
                current++;
            }
        }
    }
}

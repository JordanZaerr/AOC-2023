using System.Diagnostics;

namespace Day6
{
    internal class Day6
    {
        public static List<Race> Races;

        static void Main()
        {
            bool useSampleData = false;

            if (useSampleData) LoadSampleData(); else LoadData();

            var timer = Stopwatch.StartNew();
            Part1();
            timer.Stop();
            Console.WriteLine($"Part 1: {timer.ElapsedMilliseconds}ms");

            if (useSampleData) LoadSampleData(true); else LoadData(true);

            timer.Restart();
            Part2();
            Console.WriteLine($"Part 2: {timer.ElapsedMilliseconds}ms");

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public static void Part1()
        {
            var answer = Races.Select(GetWaysToWin).Product();
            Console.WriteLine(answer);
        }

        public static void Part2()
        {
            var answer = Races.Select(GetWaysToWin).First();
            Console.WriteLine(answer);
        }

        public static int GetWaysToWin(Race race)
        {
            var waysToWin = 0;
            var maxDistanceTraveled = 0L;
            for (long holdTime = 1; holdTime < race.Duration; holdTime++)
            {
                var timeRemaining = race.Duration - holdTime;
                var distanceTraveled = timeRemaining * holdTime;
                if (distanceTraveled > race.RecordDistance)
                    waysToWin++;

                if(distanceTraveled > maxDistanceTraveled)
                    maxDistanceTraveled = distanceTraveled;

                if (distanceTraveled < maxDistanceTraveled && distanceTraveled < race.RecordDistance) break;
            }

            return waysToWin;
        }

        public static void LoadSampleData(bool part2 = false)
        {
            //Time: 7  15   30
            //Distance: 9  40  200
            int[] time;
            int[] distance;

            if (part2)
            {
                time = new[] { 71530 };
                distance = new[] { 940200 };
            }
            else
            {
                time = new[] { 7, 15, 30 };
                distance = new[] { 9, 40, 200 };
            }

            Races = time.Zip(distance, (t, d) => new Race { Duration = t, RecordDistance = d }).ToList();
        }

        public static void LoadData(bool part2 = false)
        {
            //"Time:      46     80     78     66"
            //"Distance: 214   1177   1402   1024";
            int[] time;
            long[] distance;

            if (part2)
            {
                time = new[] { 46807866 };
                distance = new[] { 214117714021024 };
            }
            else
            {
                time = new[] { 46, 80, 78, 66 };
                distance = new[] { 214L, 1177, 1402, 1024 };
            }

            Races = time.Zip(distance, (t, d) => new Race { Duration = t, RecordDistance = d }).ToList();
        }
    }

    public class Race
    {
        public int Duration { get; set; }
        public long RecordDistance { get; set; }
    }

    public static class Ext
    {
        public static long Product(this IEnumerable<int> src)
        {
            return src.Aggregate(1L, (total, current) => total * current);
        }
    }
}

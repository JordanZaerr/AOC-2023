using System.Diagnostics;

namespace Day11
{
    internal class Day11
    {
        public static string[] Input = [];
        public static Universe Universe;

        static void Main()
        {
            bool useSampleData = false;

            if (useSampleData) LoadSampleData();
            else LoadData();

            Console.WriteLine();
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
            Universe.ActualExpand();
            var galaxies = Universe.AssignGalaxies();
            var combinations = galaxies.GetCombinations(x => x.Number).ToList();
            var answer = combinations.Sum(x => x.First().GetDistance(x.Last()));
            Console.WriteLine("{0:n0}", answer);
        }

        public static void Part2()
        {
            Universe.Expand(1000000);
            var galaxies = Universe.AssignGalaxies();
            var combinations = galaxies.GetCombinations(x => x.Number).ToList();
            var answer = combinations.Sum(x => x.First().GetDistance(x.Last()));
            Console.WriteLine("{0:n0}", answer);
        }

        public static void LoadSampleData()
        {
            Input =
            [
                "...#......",
                ".......#..",
                "#.........",
                "..........",
                "......#...",
                ".#........",
                ".........#",
                "..........",
                ".......#..",
                "#...#....."
            ];
            Universe = new Universe(Input);
        }

        public static void LoadData()
        {
            Input = File.ReadAllLines("Input.txt");
            Universe = new Universe(Input);
        }
    }

    public class Universe
    {
        private readonly List<List<string>> _grid;

        public Universe(string[] input)
        {
            _grid = input.Select(x => new List<string>(x.ToCharArray().Select(c => c.ToString()))).ToList();
        }

        //This physically expands the size of the grid
        //which isn't going to work for part two...so new strategy
        public void ActualExpand()
        {
            //Expand rows
            for (int i = 0; i < _grid.Count; i++)
            {
                var row = _grid[i];
                if (row.All(x => x == "."))
                {
                    _grid.Insert(i++, Enumerable.Repeat(".", row.Count).ToList());
                }
            }

            var columnCount = _grid.First().Count;
            //Expand columns
            for (int i = 0; i < columnCount; i++)
            {
                var column = _grid.Select(x => x[i]);
                if (column.All(x => x == "."))
                {
                    _grid.ForEach(x => x.Insert(i + 1, "."));
                    i++;
                    columnCount++;
                }
            }
        }

        //Don't touch the actual grid, just store the row/col expansions.
        //This could actually allow for variable rate expansions (1st increase by 10, 2nd by 100, 3rd by 1000, etc.)
        //Also doesn't consume tons of ram :)
        private readonly Dictionary<int, int> _rowExpansions = new();
        private readonly Dictionary<int, int> _columnExpansions = new();
        public void Expand(int size)
        {
            //Expand rows
            for (int i = 0; i < _grid.Count; i++)
            {
                var row = _grid[i];
                if (row.All(x => x == "."))
                {
                    _rowExpansions.Add(i, size > 1 ?  size-1 : 1);
                }
            }

            var columnCount = _grid.First().Count;
            //Expand columns
            for (int i = 0; i < columnCount; i++)
            {
                var column = _grid.Select(x => x[i]);
                if (column.All(x => x == "."))
                {
                    _columnExpansions.Add(i, size > 1 ? size-1 : 1);
                }
            }
        }

        public List<Galaxy> AssignGalaxies()
        {
            var galaxies = new List<Galaxy>();
            var count = 0;
            for (var r = 0; r < _grid.Count; r++)
            {
                for (var c = 0; c < _grid[r].Count; c++)
                {
                    if (_grid[r][c] == "#")
                    {
                        _grid[r][c] = (++count).ToString();
                        var rowExpansion = _rowExpansions.Where(x => x.Key < r).Sum(x => x.Value);
                        var columnExpansion = _columnExpansions.Where(x => x.Key < c).Sum(x => x.Value);
                        galaxies.Add(new Galaxy { Number = count, X = c + columnExpansion, Y = r + rowExpansion});
                    }
                }
            }
            return galaxies;
        }
    }

    [DebuggerDisplay("{Number}| ({X},{Y})")]
    public class Galaxy
    {
        public int Number { get; set; }
        public long X { get; set; }
        public long Y { get; set; }
    }

    public static class Ext
    {
        public static Galaxy Get(this List<Galaxy> list, int number)
        { 
            return list.First(x => x.Number == number);
        }

        public static long GetDistance(this Galaxy g1, Galaxy g2)
        {
            var x = Math.Abs(g1.X - g2.X);
            var y = Math.Abs(g1.Y - g2.Y);
            return x + y;
        }

        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this List<T> list, Func<T, int> valueFunc)
        {
            //Sample data output
            //12,13,14,15,16,17,18,19
            //23,24,25,26,27,28,29
            //34,35,36,37,38,39
            //45,46,47,48,49
            //56,57,58,59
            //67,68,69
            //78,79
            //89

            var result = new List<List<T>>();
            var orderedList = list.OrderBy(valueFunc).ToList();
            foreach (var item in orderedList)
            {
                foreach (var subItem in orderedList.SkipWhile(x => valueFunc(x) <= valueFunc(item)))
                {
                    result.Add([item, subItem]);
                }
            }
            return result;
        }
    }
}

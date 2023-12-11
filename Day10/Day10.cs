using System.Diagnostics;
using System.Text;

namespace Day10
{
    internal class Day10
    {
        public static string[] Input = [];
        public static List<List<Tile>> Tiles = new();

        static void Main()
        {
            //Want a better display of the pipe layout using unicode...
            //https://www.w3.org/TR/xml-entity-names/025.html
            Console.OutputEncoding = Encoding.UTF8;

            bool useSampleData = true;

            if (useSampleData) LoadSampleData(8);
            else LoadData();

            BuildGrid();

            Console.WriteLine();
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
            var answer = TraversePipe(out _, out _);
            PrintGrid(true);
            Console.WriteLine("{0:n0}", answer);
        }

        public static void Part2()
        {
            //Sets the IsPathMember on tiles
            TraversePipe(out var path, out var start);

            //Add in the starting point into the path
            path.Insert(0,start);

            //Figure out which nonPath members are inside/outside the path
            var nonPathMembers = Tiles.SelectMany(x => x).Where(x => !x.IsPathMember).ToList();
            foreach (var tile in nonPathMembers)
            {
                tile.IsInside = IsTileInsidePath(tile, path);
            }

            PrintGrid(true, true);
            var answer = nonPathMembers.Count(x => x.IsInside);
            Console.WriteLine("{0:n0}", answer);
        }

        public static int TraversePipe(out List<Tile> path, out Tile startPosition)
        {
            path = [];
            startPosition = Tiles.SelectMany(x => x).First(x => x.IsStartingTile);
            var current = startPosition;
            Tile previous = null;

            bool CanMove(Func<Tile, bool> canMove, Func<Tile, Tile> moveTile) 
                => canMove(current) && moveTile(current) != previous;

            void Move(Func<Tile, Tile> moveTile)
            {
                previous = current;
                current = moveTile(current);
            }

            do
            {
                if (CanMove(x => x.CanMoveUp, x => x.Above))
                    Move(x => x.Above);
                else if (CanMove(x => x.CanMoveRight, x => x.Right))
                    Move(x => x.Right);
                else if (CanMove(x => x.CanMoveDown, x => x.Below))
                    Move(x => x.Below);
                else if (CanMove(x => x.CanMoveLeft, x => x.Left))
                    Move(x => x.Left);

                if (current.IsStartingTile) break;

                path.Add(current);
                current.IsPathMember = true;
            }
            while (current != startPosition);

            var middleIndex = (int)Math.Ceiling(path.Count / 2d);

            return middleIndex;
        }

        public static bool IsTileInsidePath(Tile tile, List<Tile> path)
        {
            //Was looking at flood fill algorithms for a bit (https://en.wikipedia.org/wiki/Flood_fill)
            //Which took me to point in polygon...(https://en.wikipedia.org/wiki/Point_in_polygon)
            // https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
            var inside = false;
            for (int i = 0, upperBounds = path.Count - 1; i < path.Count; upperBounds = i++)
            {
                var xBounds = path[upperBounds].PositionX - path[i].PositionX;
                var yBounds = path[upperBounds].PositionY - path[i].PositionY;
                var xPos = path[i].PositionX;
                var yPos = tile.PositionY - path[i].PositionY;

                if (path[i].PositionY > tile.PositionY != path[upperBounds].PositionY > tile.PositionY
                    && tile.PositionX < xBounds * yPos / yBounds + xPos)
                {
                    inside = !inside;
                }
            }

            return inside;
        }


        public static void LoadSampleData(int sampleNum)
        {
            //1:Simple Loop
            var sample1 = new[]
            {
                ".....",
                ".S-7.",
                ".|.|.",
                ".L-J.",
                "....."
            };
            //2:Simple Loop with junk pipes
            var sample2 = new[]
            {
                "-L|F7",
                "7S-7|",
                "L|7||",
                "-L-J|",
                "L|-JF"
            };

            //3:Complex loop
            var sample3 = new[]
            {
                "..F7.",
                ".FJ|.",
                "SJ.L7",
                "|F--J",
                "LJ..."
            };

            //4:Complex loop with junk pipes
            var sample4 = new[]
            {
                "7-F7-",
                ".FJ|7",
                "SJLL7",
                "|F--J",
                "LJ.LJ"
            };

            //5: Enclosed Loop with opening
            var sample5 = new[] 
            {
                "...........",
                ".S-------7.",
                ".|F-----7|.",
                ".||.....||.",
                ".||.....||.",
                ".|L-7.F-J|.",
                ".|..|.|..|.",
                ".L--J.L--J.",
                "..........."
            };

            //6:Enclosed Loop
            var sample6 = new[]
            {
                "..........",
                ".S------7.",
                ".|F----7|.",
                ".||....||.",
                ".||....||.",
                ".|L-7F-J|.",
                ".|..||..|.",
                ".L--JL--J.",
                ".........."
            };

            //7:Large Enclosed loop
            var sample7 = new[]
            {
                ".F----7F7F7F7F-7....",
                ".|F--7||||||||FJ....",
                ".||.FJ||||||||L7....",
                "FJL7L7LJLJ||LJ.L-7..",
                "L--J.L7...LJS7F-7L7.",
                "....F-J..F7FJ|L7L7L7",
                "....L7.F7||L7|.L7L7|",
                ".....|FJLJ|FJ|F7|.LJ",
                "....FJL-7.||.||||...",
                "....L---J.LJ.LJLJ..."
            };

            //8: Large Enclosed loop with junk pipes
            var sample8 = new[] 
            {
                "FF7FSF7F7F7F7F7F---7",
                "L|LJ||||||||||||F--J",
                "FL-7LJLJ||||||LJL-77",
                "F--JF--7||LJLJ7F7FJ-",
                "L---JF-JLJ.||-FJLJJ7",
                "|F|F-JF---7F7-L7L|7|",
                "|FFJF7L7F-JF7|JL---7",
                "7-L-JL7||F7|L7F-7F7|",
                "L.L7LFJ|||||FJL7||LJ",
                "L7JLJL-JLJLJL--JLJ.L"
            };


            Input = sampleNum switch
            {
                1 => sample1,
                2 => sample2,
                3 => sample3,
                4 => sample4,
                5 => sample5,
                6 => sample6,
                7 => sample7,
                _ => sample8
            };

            for (int row = 0; row < Input.Length; row++)
            {
                var rowInput = Input[row];
                var rowTiles = rowInput.Select((t, col) => new Tile(t, col, row)).ToList();
                Tiles.Add(rowTiles);
            }
        }

        public static void LoadData()
        {
            Input = File.ReadAllLines("Input.txt");
            for (var row = 0; row < Input.Length; row++)
            {
                var rowInput = Input[row];
                var rowTiles = rowInput.Select((t, col) => new Tile(t, col, row)).ToList();
                Tiles.Add(rowTiles);
            }
        }

        public static void BuildGrid()
        {
            foreach (var window in Tiles.SelectMany(row => row.Slide(2)))
            {
                window[0].Right = window[1];
                window[1].Left = window[0];
            }

            var columnCount = Tiles.First().Count;
            foreach (var columnIndex in Enumerable.Range(0, columnCount))
            { 
                var column = Tiles.Select(x => x[columnIndex]).Slide(2);
                foreach (var window in column)
                {
                    window[0].Below = window[1];
                    window[1].Above = window[0];
                }
            }
        }

        private static void PrintGrid(bool withHeaders = false, bool colorBoundaries = false)
        {
            //Print Column Headers (Horizontal numbers)
            var rowCountLength = (Tiles.Count-1).ToString().Length;
            if (withHeaders)
            {
                var columnCountLength = (Tiles.First().Count - 1).ToString().Length;
                var values = Enumerable.Range(0, Tiles.First().Count).Select(x => x.ToString().PadLeft(columnCountLength)).ToList();
                foreach (var digit in Enumerable.Range(0, columnCountLength))
                {
                    Console.Write("".PadRight(rowCountLength, ' '));
                    values.Select(x => x[digit]).Foreach(Console.Write);
                    Console.WriteLine();
                }
            }

            Tiles.Foreach((x, i) =>
            {
                if(withHeaders)
                    Console.Write(i.ToString().PadLeft(rowCountLength, ' '));
                
                x.Foreach(c =>
                {
                    if (c.IsPathMember)
                    {
                        using (new CColor(ConsoleColor.Green))
                        {
                            Console.Write(c);
                        }
                    }
                    else if (colorBoundaries)
                    {
                        using (new CColor(c.IsInside ? ConsoleColor.Blue : ConsoleColor.Red))
                        {
                            Console.Write(c);
                        }
                    }
                    else
                        Console.Write(c);
                });
                Console.WriteLine();
            });
            Console.WriteLine();
        }
    }

    [DebuggerDisplay("{Type.ToString()} | {PositionX} | {PositionY}")]
    public class Tile
    {
        private readonly char _character;
        public bool IsStartingTile { get; }
        public bool IsPathMember { get; set; }

        public  bool IsInside { get; set; }

        private readonly Dictionary<char, char> _tileTypes = new()
        {
            { '|', '\u2503'},
            { '-', '\u2501'},
            { 'L', '\u2517'},
            { 'J', '\u251B'},
            { '7', '\u2513'},
            { 'F', '\u250F'},
            { '.', 'X' },
            { 'S', 'S' }
        };

        private readonly Dictionary<char, Movement> _movementMap = new()
        {
            { '|', new Movement { CanMoveUp = true, CanMoveDown = true } },
            { '-', new Movement { CanMoveLeft = true, CanMoveRight = true } },
            { 'L', new Movement { CanMoveUp = true, CanMoveRight = true } },
            { 'J', new Movement { CanMoveUp = true, CanMoveLeft = true } },
            { '7', new Movement { CanMoveDown = true, CanMoveLeft = true } },
            { 'F', new Movement {CanMoveDown = true, CanMoveRight = true } },
            { '.', new Movement() },
            //This isn't technically correct but start goes wherever its adjoining tiles go.
            { 'S', new Movement { CanMoveUp = true, CanMoveDown = true, CanMoveLeft = true, CanMoveRight = true } }
        };


        public Tile(char character, int x, int y)
        {
            _character = character;
            Type = _tileTypes[character];
            PositionX = x;
            PositionY = y;
            IsStartingTile = character == 'S';
            IsPathMember = IsStartingTile;
        }

        //This is for display purposes
        public char Type { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public Tile Above { get; set; }
        public Tile Below { get; set; }
        public Tile Left { get; set; }
        public Tile Right { get; set; }

        public bool CanMoveUp => Above != null && _movementMap[_character].CanMoveUp && _movementMap[Above._character].CanMoveDown;

        public bool CanMoveDown => Below != null && _movementMap[_character].CanMoveDown && _movementMap[Below._character].CanMoveUp;

        public bool CanMoveLeft => Left != null && _movementMap[_character].CanMoveLeft && _movementMap[Left._character].CanMoveRight;

        public bool CanMoveRight => Right != null && _movementMap[_character].CanMoveRight && _movementMap[Right._character].CanMoveLeft;

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    public class Movement
    { 
        public bool CanMoveUp { get; set; }
        public bool CanMoveDown { get; set; }
        public bool CanMoveLeft { get; set; }
        public bool CanMoveRight { get; set; }
    }

    public static class EXT
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

        public static void Foreach<T>(this IEnumerable<T> src, Action<T> action)
        {
            foreach (var item in src)
            {
                action(item);
            }
        }
        public static void Foreach<T>(this IEnumerable<T> src, Action<T, int> action)
        {
            var count = 0;
            foreach (var item in src)
            {
                action(item, count);
                count++;
            }
        }
    }

    public class CColor : IDisposable
    {
        private readonly ConsoleColor _originalColor;

        public CColor(ConsoleColor color)
        {
            _originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ForegroundColor = _originalColor;
        }
    }
}

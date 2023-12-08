using System.Text.RegularExpressions;

namespace Day1
{
    internal class Day1
    {
        static void Main()
        {
            //Part1();
            Part22();
        }

        public static void Part1()
        {
            // var input = new []
            // {
            //     "1abc2",
            //     "pqr3stu8vwx",
            //     "a1b2c3d4e5f",
            //     "treb7uchet"
            // };
            var input = File.ReadAllLines("Input.txt");
            var output = input.Select(x => x.Where(char.IsNumber).ToList())
                .Select(x => int.Parse($"{x.First()}{x.Last()}")).Sum();
            Console.WriteLine(output);
        }

        public static void Part22()
        {
            var list = new List<string>(File.ReadAllLines(@"input.txt"));
            var numbers = new Dictionary<string, int>() {
                {"one" ,   1}
                ,{"two" ,  2}
                ,{"three" , 3}
                ,{"four" , 4}
                ,{"five" , 5}
                ,{"six" , 6}
                ,{"seven" , 7}
                ,{"eight" , 8}
                , {"nine" , 9 }
            };
            int total = 0;
            string digit1 = string.Empty;
            string digit2 = string.Empty;
            foreach (var item in list)
            {
                //forward
                digit1 = calculateTotal(item, numbers);
                digit2 = calculateTotal(new string(item.Reverse().ToArray()), numbers.ToDictionary(k => new string(k.Key.Reverse().ToArray()), k => k.Value));
                total += int.Parse(digit1 + digit2);
            }

            Console.WriteLine(total);

            string calculateTotal(string item, Dictionary<string, int> numbers)
            {
                int index = 0;
                int digit = 0;
                foreach (var c in item)
                {
                    var sub = item.AsSpan(index++);
                    foreach (var n in numbers)
                    {
                        if (sub.StartsWith(n.Key))
                        {
                            digit = n.Value;
                            goto end;
                        }
                    }

                    if ((int)c >= 48 && (int)c <= 57)
                    {
                        digit = ((int)c) - 48;
                        break;
                    }
                }
                end:
                return digit.ToString();
            }
        }

        //Figure out why this doesn't work...
        public static void Part2()
        {
            var lookup = new Dictionary<string, string>{
                {"one","1"},
                {"two","2"},
                {"three","3"},
                {"four","4"},
                {"five","5"},
                {"six","6"},
                {"seven","7"},
                {"eight","8"},
                {"nine","9"}
            };
            // var input = new []
            // {
            //     "fourthree5threenineq",
            //     "two1nine",
            //     "eightwothree",
            //     "abcone2threexyz",
            //     "xtwone3four",
            //     "4nineeightseven2",
            //     "zoneight234",
            //     "7pqrstsixteen"
            // };
            var input = File.ReadAllLines("Input.txt");

            // File.WriteAllLines("output.txt", input.Select(x => ReplaceNumbers(x, lookup)));
            var output = input.Select(x => ReplaceNumbers(x, lookup)).Select(x => x.Where(char.IsNumber).ToList())
                .Select(x => int.Parse($"{x.First()}{x.Last()}")).Sum();

            Console.WriteLine(output);
        }

        private static string ReplaceNumbers(string input, Dictionary<string, string> replacements)
        {
            var matches = replacements.Select(kvp =>
                (Match: kvp, Index: input.IndexOf(kvp.Key, StringComparison.InvariantCultureIgnoreCase)))
                .Where(x => x.Index >= 0).OrderBy(x => x.Index).ToList();

            var offset = 0;
            var value = input;

            foreach (var match in matches)
            {
                var stillMatch = value.IndexOf(match.Match.Key) == match.Index - offset;
                if (stillMatch)
                {
                    var count = Regex.Matches(value, $"{match.Match.Key}");
                    value = value.Replace(match.Match.Key, match.Match.Value);
                    offset += (match.Match.Key.Length - 1) * count.Count;
                }
                else
                {
                    var trimStart = match.Match.Key.Substring(1);
                    var stillMatchStart = value.IndexOf(trimStart) == match.Index - offset;
                    if (stillMatchStart)
                    {
                        var count = Regex.Matches(value, $"{match.Match.Key}");
                        value = value.Replace(trimStart, match.Match.Value);
                        offset += match.Match.Key.Length * count.Count;
                    }
                    else
                        Console.WriteLine(value);
                }
            }
            //var value = input;
            //var first = matches.First();
            // if (matches.Count == 1)
            // {
            //     value = input.Insert(first.Index + first.Match.Key.Length, first.Match.Value);
            // }
            //
            // if (matches.Count > 1)
            // {
            //     var last = matches.Last();
            //     value = input.Insert(first.Index + first.Match.Key.Length, first.Match.Value)
            //         .Insert(last.Index + last.Match.Key.Length, last.Match.Value);
            // }

            return value;
        }
    }
}

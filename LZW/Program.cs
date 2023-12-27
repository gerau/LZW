

using LZW.BitStream;
using System.Text.RegularExpressions;
using System.Text;

namespace LZW
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var r = new Regex(@"(?i)LZW\s+(?<cmd>\w)(?-i)\s+(?<input>[\w:\\.]+)(?<output>\s[\w:\\.]+)?");
            var rhelp = new Regex(@"(?i)LZW\s+help\s*(?-i)");
            while (true)
            {
                Console.WriteLine("Enter command(type 'LZW help' for more information):");
                var s = Console.ReadLine();
                if (s == "")
                {
                    Console.WriteLine("Error: empty input");
                    continue;
                }
                if (r.IsMatch(s))
                {
                    var match = r.Match(s);
                    if (match.Groups[1].ToString().Equals("e", StringComparison.OrdinalIgnoreCase))
                    {
                        if (match.Groups["input"].ToString().Trim() == "")
                        {
                            Console.WriteLine("Error: empty file given to input!");
                            continue;
                        }
                        if (match.Groups["output"].ToString().Trim() == "")
                        {
                            var output = match.Groups["input"].ToString().Split('.')[0] + "_commpressed.bin";
                            LZW.Encode(match.Groups["input"].ToString(), output);
                            continue;
                        }
                        LZW.Encode(match.Groups["input"].ToString(), match.Groups["output"].ToString().Trim());

                    }
                    else if (match.Groups[1].ToString().Equals("d", StringComparison.OrdinalIgnoreCase))
                    {
                        if (match.Groups["input"].ToString().Trim() == "")
                        {
                            Console.WriteLine("Error: empty file given to input!");
                            continue;
                        }
                        if (match.Groups["output"].ToString().Trim() == "")
                        {
                            var output = match.Groups["input"].ToString().Split('.')[0] + "_decommpressed.bin";
                            LZW.Decode(match.Groups["input"].ToString(), output);
                            continue;
                        }
                        LZW.Decode(match.Groups["input"].ToString(), match.Groups["output"].ToString());

                    }
                    else
                    {
                        Console.WriteLine($"Error - unknown command '{match.Groups[1]}'");
                    }
                    continue;
                }
                if (rhelp.IsMatch(s))
                {
                    Console.WriteLine("Usage: LZW <cmd> <input> <output>");
                    Console.WriteLine("Where: <cmd> - 'd' or 'e' - decode or encode file");
                    Console.WriteLine("<input> - input file, which we need decode or encode");
                    Console.WriteLine("<output> - output file, where we save result of decode or encode. Can be empty, so result saves on file with same name");
                    Console.WriteLine("type 'exit' to close the console");
                    continue;
                }
                if (s.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                Console.WriteLine("Unknown command - commands starts from 'base64'");
            }
        }
    }
}
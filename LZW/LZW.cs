using LZW.BitStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    internal class LZW
    {
        
        
        public static byte[] divideToBytes(int number)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)number;
            bytes[1] = (byte)(number >> 8);
            return bytes;
        }
        public static int concanateBytes(byte[] bytes)
        {
            int number = 0;
            number += bytes[0];
            number += bytes[1] << 8;
            return number;
        }


        //public static void Encode(string inputFile, string outputFile)
        //{
        //    var dictionary = new Dictionary<string, int>();
        //    for (byte i = 0; i < byte.MaxValue; i++)
        //    {
        //        dictionary.Add(((char)i).ToString(), i);
        //    }
        //    int dictSize = 256;

        //    var lines = File.ReadAllText(inputFile);

        //    var writer = new BitStreamWriter(outputFile);

        //    var currentBitsForWrite = 8;

        //    bool dictStop = false;
        //    string w = "";
        //    foreach(char c in lines)
        //    {
        //        var wc = w + c;
        //        if (dictionary.ContainsKey(wc))
        //        {
        //            w = wc;
        //            continue;
        //        }
        //        if (!dictStop)
        //        {
        //            writer.WriteBitSequence(divideToBytes(dictionary[w]), currentBitsForWrite);
        //            dictionary[wc] = dictSize;
        //            dictSize += 1;
        //            if (1 << currentBitsForWrite <= dictSize)
        //            {
        //                currentBitsForWrite++;
        //            }
        //            w = c.ToString();
        //        }
        //        else
        //        {
        //            writer.WriteBitSequence(divideToBytes(dictionary[w]), currentBitsForWrite);
        //            w = c.ToString();
        //        }
        //        dictStop = currentBitsForWrite > 16;
        //    }
        //    writer.Close();
        //}
        //public static void Decode(string inputFile, string outputFile)
        //{
        //    var dictionary = new Dictionary< int, string>();
        //    for (byte i = 0; i < byte.MaxValue; i++)
        //    {
        //        dictionary.Add(i, ((char)i).ToString());
        //    }
        //    int dictSize = 256;




        //    var reader = new BitStreamReader(inputFile);
        //    var writer = new BinaryWriter(new FileStream(outputFile, FileMode.OpenOrCreate));

        //    if(reader.reader == null)
        //    {
        //        return;
        //    }

        //    int oldI = reader.ReadBitSequence(8)[0];
        //    string oldS = dictionary[oldI];
        //    writer.Write(oldS);
        //    string str = oldS;
        //    var currentBitsForWrite = 8;

        //    var length = reader.reader.BaseStream.Length;
        //    try
        //    {
        //        while (true)
        //        {
        //            int I = concanateBytes(reader.ReadBitSequence(currentBitsForWrite));
        //            if (dictionary.ContainsKey(I))
        //            {
        //                str = dictionary[I];
        //                writer.Write(str);
        //                dictionary.Add(dictionary.Count, oldS + str.ElementAt(0));
        //                oldI = I;
        //                oldS = str;
        //            }
        //            else
        //            {
        //                str = oldS + oldS.ElementAt(0);
        //                writer.Write(str);
        //                dictionary.Add(dictionary.Count, str);
        //            }

        //            if (dictionary.Count >= 1 << currentBitsForWrite)
        //            {
        //                currentBitsForWrite++;
        //            }
        //            if (currentBitsForWrite > 16)
        //                break;
        //        }
        //    }
        //    catch
        //    {
        //        reader.Close();
        //        writer.Close();
        //    }

        //}


        public static void Encode(string inputFile, string outputFile)
        {
            var dictionary = new Dictionary<string, int>();
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                dictionary.Add(((char)i).ToString(), i);
            }
            int index = -1;
            string str = "";
            var writer = new BitStreamWriter(outputFile);
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: this file {inputFile} doesnt exist!");
                return;
            }
            var size = 256;
            var reader = new BinaryReader(new FileStream(inputFile, FileMode.Open));
            var currentBitLength = 8;
            try
            {
                while (true)
                {
                    var c = reader.ReadByte();
                    str = str + (char)c;
                    if (dictionary.ContainsKey(str))
                        index = dictionary[str];
                    else
                    {
                        var bytes = divideToBytes(index);
                        writer.WriteBitSequence(bytes, currentBitLength);
                        dictionary.Add(str, dictionary.Count);
                        str = "";
                        str += (char)c;
                        index = dictionary[str];
                    };
                    if (dictionary.Count >= size)
                    {
                        size *= 2;
                        currentBitLength++;
                    }
                    if (currentBitLength >= 16)
                        break;
                }
            }
            catch
            {

            }
            finally
            {
                var bytes = divideToBytes(index);
                writer.WriteBitSequence(bytes, currentBitLength);
                writer.Close();
                reader.Close();
                foreach( var kvp in dictionary)
                {
                    Console.WriteLine($"{kvp.Key} - {kvp.Value}");
                }
            }
        }
        public static void Decode(string inputFile, string outputFile)
        {
            var dictionary = new Dictionary<int, string>();
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                dictionary.Add(i, ((char)i).ToString());
            }

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: this file {inputFile} doesnt exist!");
                return;
            }
            var reader = new BitStreamReader(inputFile);
            var writer = new BinaryWriter(new FileStream(outputFile, FileMode.OpenOrCreate));
            var size = 256;

            int index = -1;
            string str = "";
            var currentBitLength = 8;

            int OldI = reader.ReadBitSequence(8)[0];
            string OldS = dictionary[OldI];
            writer.Write(OldS);
            size *= 2;
            currentBitLength++;

            try
            {
                while (true)
                {
                    int I = concanateBytes(reader.ReadBitSequence(currentBitLength));
                    if (dictionary.ContainsKey(I))
                    {
                        str = dictionary[I];
                        writer.Write(str);
                        dictionary.Add(dictionary.Count(), OldS + str.ElementAt(0));
                        OldI = I;
                        OldS = str;
                    }
                    else
                    {
                        str = OldS + OldS.ElementAt(0);
                        writer.Write(str);
                        dictionary.Add(dictionary.Count(), str);
                    }

                    if (dictionary.Count >= size)
                    {
                        size *= 2;
                        currentBitLength++;
                    }
                    if (currentBitLength > 16)
                        break;
                }

            }
            catch
            {

            }
            finally
            {
                writer.Close();
                reader.Close();
            }
        }
    }
}

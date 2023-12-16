using LZW.BitStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    internal class LZW
    {
        public List<(byte, int)> dictionary;
        public LZW()
        {
            dictionary = new List<(byte, int)>();
            for (byte i = 0; i <= byte.MaxValue; i++)
            {
                dictionary.Add((i, -1));
            }
        }


        public void Encode(string inputFile, string outputFile)
        {
            int index = -1;
            string str = "";
            var writer = new BitStreamWriter(outputFile);
            if (!File.Exists(inputFile))
            {
                throw new Exception($"file {inputFile} doesn't exist!");
            }
            var reader = new BinaryReader(new FileStream(inputFile, FileMode.Open));
            var currentBitLength = 8;
            try
            {
                var c = reader.ReadByte();

            }
            catch { }
        }
    }
}

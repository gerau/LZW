

using LZW.BitStream;

namespace LZW
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stream = new BitStream.BitStreamWriter("data.txt");

            byte[] test1 = { 0xe1, 0x01 };
            byte[] test2 = { 0xee, 0x00 };

            stream.WriteBitSequence(test1, 9);
            stream.WriteBitSequence(test2, 9);


            stream.CloseStream();

            var read = new BitStreamReader("data.txt");

            
            try
            {
                var temp1 = read.ReadBitSequence(11);
                var temp2 = read.ReadBitSequence(7);
                foreach (byte b in temp1)
                {
                    Console.WriteLine(b.ToString("X"));
                }
                foreach (byte b in temp2)
                {
                    Console.WriteLine(b.ToString("X"));
                }
            }
            catch
            {
            }

        }
    }
}
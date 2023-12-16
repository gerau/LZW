using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW.BitStream
{
    internal class BitStreamReader
    {
        public string path;
        public BinaryReader reader;
        public int currentState;
        public byte lastByte;


        public BitStreamReader(string path)
        {
            this.path = path;
            currentState = 0;
            lastByte = 0;
            reader = new BinaryReader(new FileStream(path, FileMode.Open));
        }

        public bool isFileExist()
        {
            return File.Exists(path);
        }

        public byte ReadOneByte(int shift)
        {
            if (currentState == 0)
            {
                lastByte = reader.ReadByte();
                byte temp = (byte)(lastByte & ((0x01 << shift) - 0x01));
                currentState = shift;
                return temp;
            }
            var newCurrentState = currentState + shift;
            if (newCurrentState >= 8)
            {
                byte temp = reader.ReadByte();
                byte res = (byte)((lastByte >> currentState) + (temp << 8 - currentState));
                newCurrentState %= 8;
                currentState = newCurrentState;
                lastByte = temp;
                return res;
            }
            else
            {
                return (byte)(lastByte >> newCurrentState);
            }
        }

        public byte[] ReadBitSequence(int length)
        {
            var count = length / 8;
            var shift = length % 8;

            byte[] res;

            if(count == 0)
            {
                res = new byte[1];
                res[0] = ReadOneByte(shift);
                return res;
            }

            if (currentState == 0)
            {
                res = new byte[count + 1];
                for (int i = 0; i < count; i++)
                {
                    res[i] = reader.ReadByte();
                }

                int mask = (0x01 << shift) - 0x01;
                lastByte = reader.ReadByte();
                res[count] = (byte)(lastByte & (byte)mask);
                currentState = shift;
            }
            else 
            {
                res = new byte[count + 2];
                byte tempPrev = reader.ReadByte();
                res[0] = (byte)((lastByte >> currentState) + (byte)(tempPrev << (8 - currentState)));
                byte tempNext = reader.ReadByte();
                for(int  i = 1; i < count; i++) 
                {
                    res[i] = (byte)((tempPrev >> currentState) + (tempNext << 8 - currentState));
                    tempPrev = tempNext;
                    tempNext = reader.ReadByte();
                }
                var newCurrentState = shift + currentState;
                if (newCurrentState >= 8)
                {
                    res[count] = (byte)((tempPrev >> currentState) + (tempNext << 8 - currentState));
                    newCurrentState %= 8;
                    lastByte = tempNext;
                    res[count + 1] = (byte)(tempNext & ((0x01 << newCurrentState) - 0x01));
                }
                else
                {
                    res[count] = (byte)((tempPrev >> currentState) + (tempNext << 8 - newCurrentState));
                    lastByte = tempNext;
                }
                currentState = newCurrentState;
            }
            return res;
        }
        public void closeReader()
        {
            reader.Close();
        }
    }
}

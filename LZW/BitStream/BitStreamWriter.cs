using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LZW.BitStream
{

    internal class BitStreamWriter
    {
        public string path;
        public BinaryWriter writer;
        public int currentState;
        public byte lastByte;
        
        public BitStreamWriter(string path)
        {
            lastByte = 0;
            currentState = 0;
            this.path = path;
            writer = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate));
        }

        public void WriteOneByte(byte b, int shift)
        {
            if(currentState == 0)
            {
                lastByte = (byte)(b & ((0x01 << shift) - 0x01));
                currentState = shift;
                return;
            }
            var newCurrentState = currentState + shift;
            if(newCurrentState >= 8)
            {
                writer.Write((byte)(lastByte + (b << currentState)));
                newCurrentState %= 8;
                lastByte = (byte)(b & ((0x01 << newCurrentState) - 0x01));
            }
            else
            {
                lastByte = (byte)(lastByte + ((b & ((0x01 << shift) - 0x01)) << currentState));
            }

            
        }
        public void WriteBitSequence(byte[] bitSequence, int sizeOfSequence) 
        {

            int shift = sizeOfSequence % 8;
            int count = sizeOfSequence / 8;

            if(count == 0)
            {
                WriteOneByte(bitSequence[0], shift);
                return;
            }

            if (currentState == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    writer.Write(bitSequence[i]);
                }

                int mask = (0x01 << shift) - 0x01;
                lastByte = (byte)(bitSequence[count] & (byte)mask);   
                currentState = shift;
            }
            else
            { 
                writer.Write((byte)(lastByte + (byte)(bitSequence[0] << currentState)));
                for(int i = 1; i < count; i++)
                {
                    writer.Write((byte)(bitSequence[i - 1] >> (8 - currentState) + bitSequence[i] << currentState));
                }
                var newCurrentState = shift + currentState;
                if(newCurrentState >= 8)
                {
                    writer.Write((byte)((bitSequence[count - 1] >> (8 - currentState)) + (bitSequence[count] << currentState)));
                    newCurrentState %= 8;
                    lastByte = (byte)(bitSequence[count] & ((0x01 << newCurrentState) - 0x01));
                }
                else
                {
                    lastByte = (byte)((bitSequence[count - 1] >> (8 - currentState)) + ((bitSequence[count]& ((0x01 << shift) - 0x01)) << currentState));
                }
                currentState = newCurrentState;
            }
        }
        public void CloseStream()
        {
            if(currentState != 0)
            {
                writer.Write((byte)lastByte);
            }
            writer.Close();
        }
    }
}

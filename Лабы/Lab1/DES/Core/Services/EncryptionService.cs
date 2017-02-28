using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Misc;

namespace Core.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptWithDES(string text, string key, EncodingType type)
        {
            var bitText = text.ToBitArray();
            var bitKey = key.ToBitArray();

            // var bitText = "0000000100100011010001010110011110001001101010111100110111101111";
            // var bitKey = "0001001100110100010101110111100110011011101111001101111111110001";

            var key56Bits = Get56BitKey(bitKey);

            var bitTextBlocks = Enumerable.Range(0, bitText.Length / 64)
                .Select(i => bitText.Substring(i * 64, 64)).ToList();

            var encodedBitTextBlocks = bitTextBlocks.Select(block => EncryptWithDES64Bit(block, key56Bits, type));

            var encodedText = string.Join("", encodedBitTextBlocks).FromBitsToString();

            return encodedText;
        }

        private string EncryptWithDES64Bit(string text64Bits, string key56Bit, EncodingType type)
        {
            var initiallyInvertedText = InitialInvert(text64Bits);

            var leftPart = initiallyInvertedText.Substring(0, 32);
            var rightPart = initiallyInvertedText.Substring(32, 32);

            for (int i = 0; i < 16; i++)
            {
                var leftPartCopy = leftPart;
                leftPart = rightPart;

                int roundNumber = i + 1;

                if (type == EncodingType.Decoding)
                {
                    roundNumber = 16 - i;
                }

                var key48Bit = Get48BitKey(key56Bit, roundNumber);

                rightPart = XOR(leftPartCopy, EncodeFunction(rightPart, key48Bit));
            }

            var finallyInvertedText = FinalInvert(rightPart + leftPart);

            return finallyInvertedText;
        }

        private string InitialInvert(string text64Bits)
        {
            char[] initiallyInvertedText = new char[64];

            for(int i = 0; i < text64Bits.Length; i++)
            {
                initiallyInvertedText[i] = text64Bits[Constants.InitialInversionTable[i] - 1];
            }

            return string.Join("", initiallyInvertedText);
        }

        private string FinalInvert(string text64Bits)
        {
            char[] finallyInvertedText = new char[64];

            for (int i = 0; i < text64Bits.Length; i++)
            {
                finallyInvertedText[i] = text64Bits[Constants.FinalInversionTable[i] - 1];
            }

            return string.Join("", finallyInvertedText);
        }

        private string EncodeFunction(string text32Bits, string key48Bits)
        {
            var pBoxEncodedText = ExtentionPBoxEncoding(text32Bits);

            var result = XOR(pBoxEncodedText, key48Bits);

            var sBoxEncodedText = SBoxEncoding(result);

            return StraightPBoxEncoding(sBoxEncodedText);
        }

        private string ExtentionPBoxEncoding(string text32Bits)
        {
            char[] pBoxEncodedText = new char[48];

            for (int i = 0; i < pBoxEncodedText.Length; i++)
            {
                pBoxEncodedText[i] = text32Bits[Constants.ExtentionPBox[i] - 1];
            }

            return string.Join("", pBoxEncodedText);
        }

        private string XOR(string text48Bits, string key48Bits)
        {
            char[] res = new char[text48Bits.Length];

            for (int i = 0; i < text48Bits.Length; i++)
            {
                res[i] = (Convert.ToByte(text48Bits[i].ToString()) ^ Convert.ToByte(key48Bits[i].ToString())).ToString()[0];
            }

            return string.Join("", res);
        }

        private string Get48BitKey(string key56Bit, int round)
        {
            // Instead of creating every key with shifting the previous key
            // we just shift the initial key on the sum of all shifts.
            var shift = Enumerable.Range(1, round).Sum(x => Constants.ShiftTable[x]);

            var leftKeyPart = key56Bit.Substring(0, 28);
            var rightKeyPart = key56Bit.Substring(28, 28);

            var roundLeftPart = string.Join("", Enumerable.Range(0, 28).Select(x => leftKeyPart[(x + shift) % 28]));
            var roundRightPart = string.Join("", Enumerable.Range(0, 28).Select(x => rightKeyPart[(x + shift) % 28]));

            var encoded56BitKey = roundLeftPart + roundRightPart;

            var key48Bit = string.Join("", Enumerable.Range(0, 48).Select(x => encoded56BitKey[Constants.KeyEncodingPBoxTable[x] - 1]));

            return key48Bit;
        }

        private string SBoxEncoding(string text48Bits)
        {
            var blocks6Bits = Enumerable.Range(0, 8).Select(x => text48Bits.Substring(x * 6, 6)).ToList();
            var text32Bits = new StringBuilder("");

            for (int i = 0; i < 8; i++)
            {
                var m = Convert.ToInt32(blocks6Bits[i][0].ToString() + blocks6Bits[i][5].ToString(), 2);
                var l = Convert.ToInt32(blocks6Bits[i].Substring(1, 4), 2);

                var encoded4BitBlock = Convert.ToString(Constants.SBoxTable[i][m][l], 2).PadLeft(4, '0');

                text32Bits.Append(encoded4BitBlock);
            }

            return text32Bits.ToString();
        }

        private string StraightPBoxEncoding(string text32Bits)
        {
            char[] pBoxEncoded = new char[32];

            for (int i = 0; i < 32; i++)
            {
                pBoxEncoded[i] = text32Bits[Constants.StraightPBox[i] - 1];
            }

            return string.Join("", pBoxEncoded);
        }

        private string Get56BitKey(string bitKey)
        {
            //string key56Bit = bitKey.Substring(0, 56);
            //var key64BitKey = new StringBuilder("");

            //for (int i = 0; i < 8; i++)
            //{
            //    var keyBlock = key56Bit.Substring(i * 7, 7);
            //    char newBit = '0';

            //    if(keyBlock.Count(x => x == '1') % 2 == 0)
            //    {
            //        newBit = '1';
            //    }

            //    keyBlock += newBit;

            //    key64BitKey.Append(keyBlock);
            //}

            string key64BitKey = bitKey.Substring(0, 64);

            return string.Join("", Enumerable.Range(0, 56).Select(x => key64BitKey[Constants.KeyEncodingTable[x] - 1]));
        }
    }
}

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
        public string EncryptWithDES(string text, string key)
        {
            var bitText = text.ToBitArray();
            var bitKey = key.ToBitArray();

            // TODO: Get 56 bits from a key;
            var key56Bits = bitKey;

            var bitTextBlocks = Enumerable.Range(0, bitText.Length / 64)
                .Select(i => bitText.Substring(i * 64, 64)).ToList();

            bitTextBlocks.ForEach(block => block = EncryptWithDES64Bit(block, key56Bits));

            var encodedText = string.Join("", bitTextBlocks).FromBitsToString();

            return encodedText;
        }

        private string EncryptWithDES64Bit(string text64Bits, string key56Bit)
        {
            var initiallyInvertedText = InitialInvert(text64Bits);

            var leftPart = initiallyInvertedText.Substring(0, 32);
            var rightPart = initiallyInvertedText.Substring(32, 32);

            for (int i = 0; i < 16; i++)
            {
                var leftPartCopy = leftPart;
                leftPart = rightPart;

                var key48Bit = Get48BitKey(key56Bit);

                rightPart = EncodeFunction(leftPartCopy, key48Bit);
            }

            var finallyInvertedText = FinalInvert("");

            return finallyInvertedText;
        }

        private string InitialInvert(string text64Bits)
        {
            char[] initiallyInvertedText = new char[64];

            for(int i = 0; i < text64Bits.Length; i++)
            {
                initiallyInvertedText[Constants.InversionTable.IndexOf(i)] = text64Bits[i];
            }

            return string.Join("", initiallyInvertedText);
        }

        private string FinalInvert(string text64Bits)
        {
            char[] finallyInvertedText = new char[64];

            for (int i = 0; i < text64Bits.Length; i++)
            {
                finallyInvertedText[Constants.InversionTable[i]] = text64Bits[i];
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
                pBoxEncodedText[i] = text32Bits[Constants.ExtentionPBox[i]];
            }

            return string.Join("", pBoxEncodedText);
        }

        private string XOR(string text48Bits, string key48Bits)
        {
            char[] res = new char[48];

            for (int i = 0; i < 48; i++)
            {
                res[i] = (Convert.ToByte(text48Bits[i].ToString()) ^ Convert.ToByte(key48Bits[i].ToString())).ToString()[0];
            }

            return string.Join("", res);
        }

        private string Get48BitKey(string key56Bit)
        {
            // TODO: Add 48 bit key generating.
            return "";
        }

        private string SBoxEncoding(string text48Bits)
        {
            // TODO: Add S-Box encoding.
            return "";
        }

        private string StraightPBoxEncoding(string text48Bits)
        {
            // TODO: Add іstraight P-Box encoding.
            return "";
        }
    }
}

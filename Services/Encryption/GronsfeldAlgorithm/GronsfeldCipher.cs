using Caesar_decoder_encoder.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caesar_decoder_encoder.Services.Encryption.GronsfeldAlgorithm
{
    public class GronsfeldCipher : IGronsfeldCipher
    {
        private static double _updateFrequency = 0.02;
        public static  int RussianAlphabetPower => 32;

        public  static int EnglishAlphabetPower => 26;
        private static int Mod(int value, int div)
        {
            var result = value % div;
            if (result < 0)
            {
                result += div;
                result %= div;
            }
            return result;
        }
        public async Task<string> DecodeAsync(string content, string key, 
            Language language, IProgress<double> progress, CancellationToken token = default)
        {
            return await TransformContent(content, key, language, progress, Decode, token);
        }
        private async Task<string> TransformContent(string content, string key,
            Language language, IProgress<double> progress, Func<char, short, char , int, char> transformChar,
                CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                var start = language == Language.Russian ? 'а' : 'a';
                var alphabetLength = language == Language.Russian ? RussianAlphabetPower : EnglishAlphabetPower;
                var result = new StringBuilder();
                var lastPercent = 0d;
                var lettersAmount = 0;
                for (int i = 0; i < content.Length; i++)
                {
                    var charLetter = content[i];
                    if (!(Alphabet.IsRussianLetter(charLetter) || Alphabet.IsEnglishLetter(charLetter)))
                    {
                        result.Append(charLetter);
                        continue;
                    }
                    var currentPercent = i / content.Length;
                    if (currentPercent - lastPercent >= _updateFrequency)
                    {
                        progress.Report(currentPercent);
                        lastPercent = currentPercent;
                    }
                    var isUp = char.IsUpper(charLetter);
                    charLetter = char.ToLower(charLetter);
                    var currentOffset = short.Parse(new ReadOnlySpan<char>(key[lettersAmount % key.Length]));
                    lettersAmount++;
                    var encodedChar = transformChar(charLetter, currentOffset, start, alphabetLength);
                    if (isUp)
                        encodedChar = char.ToUpper(encodedChar);
                    result.Append(encodedChar);
                }
                return result.ToString();
            }, token);
        }
        private char Encode(char charLetter, short key, char start, int alphabetLength)
        {
            return (char)((Mod(((charLetter - start) + key), alphabetLength)) + start);
        }
        private char Decode(char charLetter, short key, char start, int alphabetLength)
        {
            return (char)((Mod(((charLetter - start) - key), alphabetLength)) + start);
        }
        public async Task<string> EncodeAsync(string content, string key,
            Language language, IProgress<double> progress, CancellationToken token = default)
        {
            {
                return await TransformContent(content, key, language, progress, Encode, token);
            }
        }
    }
}

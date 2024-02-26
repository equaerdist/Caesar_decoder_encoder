using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caesar_decoder_encoder.Services.Encryption.VigenereAlgorithm
{
    class VigenereAlgorithm : IVigenereCipher
    {
        public static int RussianAlphabetPower => 32;

        public static int EnglishAlphabetPower => 26;
        private static char[,] EnglishAlphabet = InitEnglishAlphabet();
        private static char[,] RussianAlphabet = InitRussianAlphabet();

       

        public VigenereAlgorithm() 
        {
        }
        private static char[,] InitRussianAlphabet() => InitAlphabet(Language.Russian);
        private static char[,] InitEnglishAlphabet() => InitAlphabet(Language.English);
        private static char[,] InitAlphabet(Language language)
        {
                var length = language == Language.English ? EnglishAlphabetPower : RussianAlphabetPower;
                var start = language == Language.English ? 'a' : 'а';
                var alphabet = new char[length, length];
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        var currentChar = (char)(start + ((i + j + 1) % length));
                        alphabet[i, j] = currentChar;
                    }
                }
                return alphabet;
        }
        private char DecodeChar(int row, char[,] alphabet, char charLetter, char start)
        {
            for (int j = 0; j < alphabet.GetLength(0); j++)
            {
                if (alphabet[row, j] == charLetter)
                    return (char)(start + j);
            }
            throw new ArgumentNullException();
        }
        private char EncodeChar(int row, char[,] alphabet, char charLetter, char start)
        {
            var column = charLetter - start;
            return alphabet[row, column];
        }
        public async Task<string> DecodeAsync(string content, string key, Language language,
            IProgress<double> progress, CancellationToken token = default) =>
            await TransformContent(content, key, language, progress, DecodeChar, token);
       
        private async Task<string> TransformContent(string content, string key, Language language,
            IProgress<double> progress, Func<int, char[,], char, char, char> transform, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                var result = new StringBuilder();
                var alphabet = language == Language.Russian ? RussianAlphabet : EnglishAlphabet;
                var start = language == Language.Russian ? 'а' : 'a';
                var onlyLettersCount = 0;
                for (int i = 0; i < content.Length; i++)
                {
                    var charLetter = content[i];
                    if (!(Alphabet.IsRussianLetter(charLetter) || Alphabet.IsEnglishLetter(charLetter)))
                    {
                        result.Append(charLetter);
                        continue;
                    }
                    var isUp = char.IsUpper(charLetter);
                    if (isUp)
                        charLetter = char.ToLower(charLetter);
                    var row = char.ToLower(key[onlyLettersCount % key.Length]) - start;
                    var encodedChar = transform(row, alphabet, charLetter, start);
                    if (isUp)
                        encodedChar = char.ToUpper(encodedChar);
                    result.Append(encodedChar);
                    onlyLettersCount++;
                }
                return result.ToString();
            });
        }

        public async Task<string> EncodeAsync(string content, string key, Language language, 
            IProgress<double> progress, CancellationToken token = default) => 
            await TransformContent(content, key, language, progress, EncodeChar, token);
    }
}

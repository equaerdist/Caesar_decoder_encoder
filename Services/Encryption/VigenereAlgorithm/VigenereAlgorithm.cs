using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services.Encryption.Parameters;
using System;
using System.Collections;
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
    class VigenereAlgorithm : VigenereCipher
    {
        private static char[,] EnglishAlphabet = InitEnglishAlphabet();
        private static char[,] RussianAlphabet = InitRussianAlphabet();

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

        protected override char TranformChar(VigenereParameters @params)
        {
            var key = @params.Key;
            var onlyLettersCount = @params.LettersAmount;
            var start = @params.Start;
            var charLetter = @params.CharLetter;
            var alphabet = @params.Alphabet;
            var row = char.ToLower(key[onlyLettersCount % key.Length]) - start;
            Func<int, char[,], char, char, char> transform = @params.Mode == 0 ? EncodeChar : DecodeChar;
            return transform(row, alphabet, charLetter, start);
        }

        protected override void InitializeParameters(VigenereParameters @params)
        {
        }
    }
}

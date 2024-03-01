using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services.Encryption.Parameters;
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
    public class GronsfeldAlgorithm : GronsfeldCipher
    {
        private char Encode(char charLetter, short key, char start, int alphabetLength)
        {
            return (char)((Mod(((charLetter - start) + key), alphabetLength)) + start);
        }
        private char Decode(char charLetter, short key, char start, int alphabetLength)
        {
            return (char)((Mod(((charLetter - start) - key), alphabetLength)) + start);
        }
        protected override GronsfeldParameters InitializeParameters(short mode, Language language, string key)
        {
            GronsfeldParameters @params = new();
            @params.Mode = mode;
            @params.Key = key;
            @params.AlphabetLength = language == Language.Russian ? RussianAlphabetPower : EnglishAlphabetPower;
            @params.Start = language == Language.Russian ? 'а' : 'a';
            return @params;
        }

        protected override char TranformChar(GronsfeldParameters @params)
        {
            var key = @params.Key;
            var currentOffset = short.Parse(new ReadOnlySpan<char>(key[@params.LettersAmount % key.Length]));
            Func<char, short, char, int, char> transformChar = @params.Mode == 0 ? Encode : Decode;
            var encodedChar = transformChar(@params.CharLetter, currentOffset, @params.Start, @params.AlphabetLength);
            return encodedChar;
        }
    }
}

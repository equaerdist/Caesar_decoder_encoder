using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Caesar_decoder_encoder.Services.CaesarAlgorithm
{
    public class CaesarCipher : ICaesarCipher
    {
        #region частота русских букв
        private readonly Dictionary<char, double> _rusFrequency =
            new Dictionary<char, double>()
            {
                {'а', 8.01},
                {'б', 1.59},
                {'в', 4.54},
                {'г', 1.70},
                {'д', 2.98},
                {'е', 8.45},
                {'ё', 0.04},
                {'ж', 0.94},
                {'з', 1.65},
                {'и', 7.35},
                {'й', 1.21},
                {'к', 3.49},
                {'л', 4.40},
                {'м', 3.21},
                {'н', 6.70},
                {'о', 10.97},
                {'п', 2.81},
                {'р', 4.73},
                {'с', 5.47},
                {'т', 6.26},
                {'у', 2.62},
                {'ф', 0.26},
                {'х', 0.97},
                {'ц', 0.48},
                {'ч', 1.44},
                {'ш', 0.73},
                {'щ', 0.36},
                {'ъ', 0.04},
                {'ы', 1.90},
                {'ь', 1.74},
                {'э', 0.32},
                {'ю', 0.64},
                {'я', 2.01}
            }.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
        #endregion
        #region частота английсских букв
        private readonly Dictionary<char, double> _englishFrequency = new()
        {
            {'e', 12.7},
            {'t', 9.06},
            {'a', 8.17},
            {'o', 7.51},
            {'i', 6.97},
            {'n', 6.75},
            {'s', 6.33},
            {'h', 6.09},
            {'r', 5.99},
            {'d', 4.25},
            {'l', 4.03},
            {'c', 2.78},
            {'u', 2.76},
            {'m', 2.41},
            {'w', 2.36},
            {'f', 2.23},
            {'g', 2.02},
            {'y', 1.97},
            {'p', 1.93},
            {'b', 1.49},
            {'v', 0.98},
            {'k', 0.77},
            {'x', 0.15},
            {'j', 0.15},
            {'q', 0.1},
            {'z', 0.05}
        };
        #endregion
        public int RussianAlphabetPower => 32;

        public int EnglishAlphabetPower => 26;

        public async Task<string> DecodeAsync(string content, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                var contentFrequency = new Dictionary<char, double>();
                var lengthWithLetters = 0;
                for(int i=0;i < content.Length; i++)
                {
                    var letter = content[i];
                    if (!(Alphabet.IsRussianLetter(letter) || Alphabet.IsEnglishLetter(letter)))
                        continue;
                    lengthWithLetters++;
                    var charLetter = char.ToLower(letter);
                    if (!contentFrequency.ContainsKey(charLetter))
                        contentFrequency.Add(charLetter, 0);
                    else
                        contentFrequency[charLetter]++;
                    if(i % 4 == 0)
                        progress.Report(i / content.Length / 2);
                }
                contentFrequency = contentFrequency
                    .Select(p => (p.Key, p.Value / lengthWithLetters * 100))
                    .OrderByDescending(p => p.Item2)
                    .ToDictionary(t => t.Key, t => t.Item2);
                var alphabetDict = language == Language.Russian ? _rusFrequency : _englishFrequency;
                var decodeDictionary = contentFrequency.Zip(alphabetDict).ToDictionary(t => t.First.Key, t => t.Second.Key);
                var result = new StringBuilder();
                for(int i=0; i < content.Length; i++)
                {
                    var letter = content[i];

                    if (!(Alphabet.IsRussianLetter(letter) || Alphabet.IsEnglishLetter(letter)))
                    {
                        result.Append(letter);
                        continue;
                    }
                    var isUp = char.IsUpper(letter);
                    var charLetter = char.ToLower(letter);
                    var decodedChar = decodeDictionary[charLetter];
                    if (isUp)
                        decodedChar = char.ToUpper(decodedChar);
                    result.Append(decodedChar);
                    if(i % 4 == 0)
                        progress.Report(i / content.Length);
                }
                return result.ToString();
            });
            
        }
        private static BigInteger Mod(BigInteger value, int div) 
        {
            var result = value % div;
            if (result < 0)
            {
                result += div;
                result %= div;
            }
            return result;
        }
        public async Task<string> EncodeAsync(string content, BigInteger key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                StringBuilder encryptedText = new StringBuilder();

                for(int i=0; i < content.Length;i++)
                {
                    var ch = content[i];
                    var upper = char.IsUpper(ch);
                    ch = char.ToLower(ch);
                    if (char.IsLetter(ch))
                    {
                        var russian = 'а';
                        var english = 'a';
                        int alphabetStart = language == Language.Russian ? (int)russian : (int)english;
                        int alphabetSize = language == Language.Russian ? RussianAlphabetPower : EnglishAlphabetPower;
                        var aread = (ch - alphabetStart + key);
                        char encryptedChar = (char)((Mod(aread, alphabetSize) + alphabetStart));
                        if (upper)
                            encryptedChar = char.ToUpper(encryptedChar);
                        encryptedText.Append(encryptedChar);
                    }
                    else
                    {
                        encryptedText.Append(ch);
                    }
                    progress.Report(i / content.Length);
                }

                return encryptedText.ToString();
            }, token);
        }
    }
}

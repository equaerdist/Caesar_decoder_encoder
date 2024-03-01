﻿using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.FrequencyAnalyze
{
    public class FrequencyAnalyzer : IFrequencyAnalyzator
    {
        private readonly Dictionary<string, FrequencyAnalyzeResult> _cache = new();
        private static readonly double _updateFrequency = 0.01;
        #region частота русских букв
        public static readonly ImmutableDictionary<char, double> RusFrequency =
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
            }.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, p => p.Value).ToImmutableDictionary();
        #endregion
        #region частота английсских букв
        public static readonly ImmutableDictionary<char, double> EnglishFrequency = new Dictionary<char, double>()
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
        }.ToImmutableDictionary();
        #endregion
        public async Task<string> GetDecodeAsync(string content, Language language,
            IProgress<(string, double)> progress, CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var analyzeResult = await GetFrequencyStats(content, progress, token);
                var contentFrequency = analyzeResult.Frequency.ToDictionary(v => v.Key, v => v.Value);
                var lengthWithLetters = analyzeResult.LettersCount;
                contentFrequency = contentFrequency
                    .Select(p => (p.Key, p.Value / lengthWithLetters * 100))
                    .OrderByDescending(p => p.Item2)
                    .ToDictionary(t => t.Key, t => t.Item2);
                var alphabetDict = language == Language.Russian ? RusFrequency : EnglishFrequency;
                var decodeDictionary = contentFrequency.Zip(alphabetDict).ToDictionary(t => t.First.Key, t => t.Second.Key);
                var result = new StringBuilder();
                for (int i = 0; i < content.Length; i++)
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
                    var currentPercent = (double)i / content.Length / 2;
                    if (currentPercent > _updateFrequency)
                        progress.Report(("Расшифровка символов", currentPercent));
                }
                progress.Report(("Расшифровка символов", 1));
                return result.ToString();
            }, token);

        }

        public async Task<FrequencyAnalyzeResult> GetFrequencyStats(string content, IProgress<(string, double)> progress,
            CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                var hash = GetHashString(content);
                if(_cache.ContainsKey(hash))
                    return _cache[hash];
                var lastPercent = 0d;
                var contentFrequency = new Dictionary<char, double>();
                var lengthWithLetters = 0;
                for (int i = 0; i < content.Length; i++)
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
                    var currentPercent = (double)i / content.Length / 2;
                    if (currentPercent - lastPercent > _updateFrequency)
                    {
                        progress.Report(("Анализ частотности", currentPercent));
                        lastPercent = currentPercent;
                    }
                }
                lastPercent = 0d;
                progress.Report(("Анализ частотности", 1));
                var notHandledDict = contentFrequency.ToImmutableDictionary(v => v.Key, v => (int)v.Value);
                var result = new FrequencyAnalyzeResult(notHandledDict, lengthWithLetters);
                _cache[hash] = result;
                return result;
            }, token);
        }
        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = MD5.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
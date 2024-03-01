using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services.Encryption.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption
{
    public abstract class CharEncryptor<TParams> : AlphabetBasedEncryptor where TParams : IParameters
    {
        protected double _updateFrequency = 0.02;
        public override async Task<string> DecodeAsync(string content, string key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            var @params = InitializeParameters(1, language, key);
            return await TransformContent(content, @params, progress, token);
        }

        public override async Task<string> EncodeAsync(string content, string key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            var @params = InitializeParameters(0, language, key);
            return await TransformContent(content, @params, progress, token);
        }
        protected abstract TParams InitializeParameters(short mode, Language language, string key);
        protected async Task<string> TransformContent(string content,TParams @params, 
            IProgress<double> progress, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                var onlyLettersCount = 0;
                var result = new StringBuilder();
                var lastPercent = 0d;
                for (int i = 0; i < content.Length; i++)
                {
                    var charLetter = content[i];
                    if (!(Alphabet.IsRussianLetter(charLetter) || Alphabet.IsEnglishLetter(charLetter)))
                    {
                        result.Append(charLetter);
                        continue;
                    }
                    double current = i / content.Length;
                    if (current - lastPercent >= _updateFrequency)
                    {
                        progress.Report(current);
                        current = lastPercent;
                    }
                    var isUp = char.IsUpper(charLetter);
                    if (isUp)
                        charLetter = char.ToLower(charLetter);
                    @params.CharLetter = charLetter;
                    @params.LettersAmount = onlyLettersCount;
                    var encodedChar = TranformChar(@params);
                    if (isUp)
                        encodedChar = char.ToUpper(encodedChar);
                    result.Append(encodedChar);
                    onlyLettersCount++;
                }
                return result.ToString();
            }, token);
        }
        protected abstract char TranformChar(TParams @params);
    }
}

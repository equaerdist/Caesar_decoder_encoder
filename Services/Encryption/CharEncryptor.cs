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
    public abstract class CharEncryptor<TParams> : AlphabetBasedEncryptor where TParams : IParameters, new()
    {
        protected double _updateFrequency = 0.02;
        public override async Task<string> DecodeAsync(string content, string key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            var @params = CommonParametersInitialization(1, language, key);
            return await TransformContent(content, @params, progress, token);
        }

        public override async Task<string> EncodeAsync(string content, string key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            var @params = CommonParametersInitialization(0, language, key);
            return await TransformContent(content, @params, progress, token);
        }
        protected virtual TParams CommonParametersInitialization(short mode, Language language, string key)
        {
            var @params = new TParams();
            @params.Key = key;
            @params.Start = language == Language.Russian ? 'а' : 'a';
            @params.Mode = mode;
            @params.LettersAmount = language == Language.Russian ? 32 : 26;
            @params.SkipNotAlphabet = true;
            InitializeParameters(@params); 
            return @params;
        }
        protected abstract void InitializeParameters(TParams @params);
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
                    if (!(Alphabet.IsRussianLetter(charLetter) || Alphabet.IsEnglishLetter(charLetter)) && @params.SkipNotAlphabet)
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

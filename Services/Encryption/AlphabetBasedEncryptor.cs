using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption
{
    public abstract class AlphabetBasedEncryptor : IWordEncryptor
    {
        public static int Mod(int value, int div)
        {
            var result = value % div;
            if (result < 0)
            {
                result += div;
                result %= div;
            }
            return result;
        }
        public static int RussianAlphabetPower => 32;

        public static int EnglishAlphabetPower => 26;
        public abstract Task<string> DecodeAsync(string content, string key, Language language,
            IProgress<double> progress, CancellationToken token = default);


        public abstract Task<string> EncodeAsync(string content, string key, Language language,
            IProgress<double> progress, CancellationToken token = default);
    }
}

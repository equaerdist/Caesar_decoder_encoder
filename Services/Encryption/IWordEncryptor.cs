using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption
{
    public interface IWordEncryptor
    {
        static int RussianAlphabetPower { get; }
        static int EnglishAlphabetPower { get; }
        Task<string> EncodeAsync(string content, string key, Language language,
            IProgress<double> progress, CancellationToken token = default);
        Task<string> DecodeAsync(string content, string key, Language language,
            IProgress<double> progress, CancellationToken token = default);
    }
}

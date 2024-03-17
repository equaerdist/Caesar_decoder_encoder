using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.FrequencyAnalyze
{
    public interface IFrequencyAnalyzator
    {
        public static readonly ImmutableDictionary<char, double> RusFrequency = null!;
        public static readonly ImmutableDictionary<char, double> EnglishFrequency = null!;
        public Task<FrequencyAnalyzeResult> GetFrequencyStats(string content,
           IProgress<(string, double)> progress, CancellationToken token = default);
        public Task<string> GetDecodeAsync(string content, Language language,
           IProgress<(string, double)> progress, CancellationToken token = default);
    }
}

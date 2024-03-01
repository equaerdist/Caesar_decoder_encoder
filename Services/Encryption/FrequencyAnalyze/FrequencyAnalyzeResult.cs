using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.FrequencyAnalyze
{
    public class FrequencyAnalyzeResult
    {
        public readonly int LettersCount;

        public readonly ImmutableDictionary<char, int> Frequency;
        public FrequencyAnalyzeResult(ImmutableDictionary<char, int> dict, int lettersCount) 
        {
            Frequency = dict;
            LettersCount = lettersCount;
        }
    }
}

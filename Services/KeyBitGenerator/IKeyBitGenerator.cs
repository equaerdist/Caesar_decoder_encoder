using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.KeyBitGenerator
{
    public interface IKeyBitGenerator
    {
        Task<string> GenerateAsync(string content, 
            IProgress<double> progress, 
            CancellationToken token = default);
    }
}

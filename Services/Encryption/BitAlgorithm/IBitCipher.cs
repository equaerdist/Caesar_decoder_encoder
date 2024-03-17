using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.BitAlgorithm
{
    public interface IBitCipher
    {
        Task<string> EncodeAsync(string content, 
            IProgress<double> progress, 
            CancellationToken token = default);
        Task<string> DecodeAsync(string content,
            IProgress<double> progress, 
            CancellationToken token = default);
    }
}

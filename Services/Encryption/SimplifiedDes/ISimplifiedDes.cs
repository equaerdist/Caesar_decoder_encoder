using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.SimplifiedDes
{
    public interface ISimplifiedDes
    {
        Task<string> EncodeAsync(string content, string key, Language language,
           IProgress<double> progress, IProgress<string> status, CancellationToken token = default);
        Task<string> DecodeAsync(string content, string key, Language language,
            IProgress<double> progress, IProgress<string> status, CancellationToken token = default);
    }
}

using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.GronsfeldAlgorithm
{
    internal class GronsfeldCipher : IGronsfeldCipher
    {
        public Task<string> DecodeAsync(string content, string key, 
            Language language, IProgress<double> progress, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> EncodeAsync(string content, string key, 
            Language language, IProgress<double> progress, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}

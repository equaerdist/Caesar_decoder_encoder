using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.CaesarAlgorithm
{
    public interface ICaesarCipher
    {
        Task<string> EncodeAsync(string content, BigInteger key, Language language);
        Task<string> DecodeAsync(string content, out BigInteger key, Language language);
    }
}

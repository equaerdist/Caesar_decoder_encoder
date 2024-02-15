using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.CaesarAlgorithm
{
    public interface ICaesarEncoder
    {
        string Encode(string content);
    }
}

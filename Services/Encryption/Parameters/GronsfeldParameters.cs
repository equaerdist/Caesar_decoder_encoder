using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.Parameters
{
    public class GronsfeldParameters : IParameters
    {
        public char CharLetter { get; set; }
        public int LettersAmount { get; set; }
        public string Key { get; set; } = null!;
        public char Start { get; set; }
        public short Mode { get; set; }
        public int AlphabetLength { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.Parameters
{
    public class VigenereParameters : IParameters
    {
        public char CharLetter { get ; set; }
        public int LettersAmount { get; set; }
        public string Key { get; set; } = null!;
        public char Start { get; set; }
        public short Mode { get; set; }
        public char[,] Alphabet { get; set; } = null!;
    }
}

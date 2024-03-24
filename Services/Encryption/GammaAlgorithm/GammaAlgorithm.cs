using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.GammaAlgorithm
{
    public class GammaAlgorithm : GammaCipher
    {

        protected override void InitializeParameters(GammaParameters @params)
        {
            @params.SkipNotAlphabet = false;
        }

        protected override char TranformChar(GammaParameters @params)
        {
            var charLetter = byte.Parse(@params.CharLetter.ToString());
            var keyForSingle = byte.Parse(@params.Key[@params.LettersAmount % @params.Key.Length]
                .ToString());
            return ((charLetter + keyForSingle) % 2).ToString().First();
        }
    }
}

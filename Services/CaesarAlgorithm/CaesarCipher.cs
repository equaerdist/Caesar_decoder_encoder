using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Caesar_decoder_encoder.Services.CaesarAlgorithm
{
    public class CaesarCipher : ICaesarCipher
    {
        public int RussianAlphabetPower => 32;

        public int EnglishAlphabetPower => 26;

        public Task<string> DecodeAsync(string content, out BigInteger key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        private static BigInteger Mod(BigInteger value, int div) 
        {
            var result = value % div;
            if (result < 0)
            {
                result += div;
                result %= div;
            }
            return result;
        }
        public async Task<string> EncodeAsync(string content, BigInteger key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                StringBuilder encryptedText = new StringBuilder();

                for(int i=0; i < content.Length;i++)
                {
                    var ch = content[i];
                    var upper = char.IsUpper(ch);
                    ch = char.ToLower(ch);
                    if (char.IsLetter(ch))
                    {
                        var russian = 'а';
                        var english = 'a';
                        int alphabetStart = language == Language.Russian ? (int)russian : (int)english;
                        int alphabetSize = language == Language.Russian ? RussianAlphabetPower : EnglishAlphabetPower;
                        var aread = (ch - alphabetStart + key);
                        char encryptedChar = (char)((Mod(aread, alphabetSize) + alphabetStart));
                        if (upper)
                            encryptedChar = char.ToUpper(encryptedChar);
                        encryptedText.Append(encryptedChar);
                    }
                    else
                    {
                        encryptedText.Append(ch);
                    }
                    progress.Report(i / content.Length);
                }

                return encryptedText.ToString();
            }, token);
        }
    }
}

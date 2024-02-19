using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
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
        public Task<string> DecodeAsync(string content, out BigInteger key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<string> EncodeAsync(string content, BigInteger key, Language language, 
            IProgress<double> progress, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                StringBuilder encryptedText = new StringBuilder();

                for(int i=0; i < content.Length;i++)
                {
                    progress.Report(i / content.Length);
                    var ch = content[i];
                    if (char.IsLetter(ch))
                    {
                        int alphabetStart = language == Language.Russian ? (int)'а' : (int)'a';
                        int alphabetSize = language == Language.Russian ? 32 : 26;

                        char encryptedChar = (char)(((ch - alphabetStart + key) % alphabetSize) + alphabetStart);
                        encryptedText.Append(encryptedChar);
                    }
                    else
                    {
                        encryptedText.Append(ch);
                    }
                }

                return encryptedText.ToString();
            }, token);
        }
    }
}

using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Encryption.BitAlgorithm
{
    public class BitAlgorithm : IBitCipher
    {
        private int _amountOfBits = 16;
        private double _updateFrequency = 0.05;
        private byte _base = 2;
        public async Task<string> DecodeAsync(string content,
            IProgress<double> progress, 
            CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                double lastPercent = 0;
                var result = new StringBuilder();
                for (int i = 0; i < content.Length; i+= _amountOfBits)
                {
                    var currentPercent = i / content.Length;
                    if (currentPercent - lastPercent > _updateFrequency)
                    {
                        progress.Report(currentPercent);
                        lastPercent = currentPercent;
                    }
                    var currentLetter = content.Substring(i, _amountOfBits);
                    char restoredSymbol = Convert.ToChar(Convert.ToUInt16(currentLetter, _base));
                    result.Append(restoredSymbol);
                }
               
                return result.ToString();
            }, token);
           
        }

        public async Task<string> EncodeAsync(string content,
            IProgress<double> progress,
            CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                double lastPercent = 0;
                var result = new StringBuilder();
                for (int i = 0; i < content.Length; i++)
                {
                    var currentPercent = i / content.Length;
                    if (currentPercent - lastPercent > _updateFrequency)
                    {
                        progress.Report(currentPercent);
                        lastPercent = currentPercent;
                    }
                    var charLetter = content[i];
                    string binaryRepresentation = Convert.ToString(charLetter, _base).PadLeft(_amountOfBits, '0');
                    result.Append(binaryRepresentation);
                }
                return result.ToString();
            }, token);
        }
    }
}

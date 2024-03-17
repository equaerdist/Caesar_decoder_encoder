using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.KeyBitGenerator
{
    public class KeyBitGenerator : IKeyBitGenerator
    {
        private readonly double _updateFrequency = 0.02;
        public async Task<string> GenerateAsync(string content, IProgress<double> progress, CancellationToken token = default)
        {
            return await Task.Run(() => {
                var result = new StringBuilder();
                int length = content.Length;
                int halfLength = (length + 1) / 2;
                int zeros = 0;
                int ones = 0;
                var lastPercent = 0d;
                for (int i = 0; i < length; i++)
                {
                    var currentPercent = (double)i / length;
                    if (currentPercent - lastPercent >= _updateFrequency)
                    {
                        progress.Report(currentPercent);
                        lastPercent = currentPercent;
                    }
                    if (zeros < halfLength && ones < halfLength)
                    {
                        if (new Random().Next(2) == 0)
                        {
                            result.Append('0');
                            zeros++;
                        }
                        else
                        {
                            result.Append("1");
                            ones++;
                        }
                    }
                    else if (zeros < halfLength)
                    {
                        result.Append('0');
                        zeros++;
                    }
                    else
                    {
                        result.Append("1");
                        ones++;
                    }
                }
                progress.Report(1);
                return result.ToString();
            }, token);
            
        }
    }
}

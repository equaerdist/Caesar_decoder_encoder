using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services.Encryption.BitAlgorithm;
using Caesar_decoder_encoder.Services.KeyBitGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caesar_decoder_encoder.Services.Encryption.SimplifiedDes
{
    public class SimplifiedDes : ISimplifiedDes
    {
        private readonly TimeSpan _updateTime = TimeSpan.FromMilliseconds(200);
        private readonly IKeyBitGenerator _keygen;
        private readonly IBitCipher _bitConverter;
        private int _amountOfBits = 16;
        private byte _base = 2;

        public SimplifiedDes(IKeyBitGenerator keygen, IBitCipher bitConverter) 
        {
            _keygen = keygen;
            _bitConverter = bitConverter;
        }
        public async Task<string> DecodeAsync(string content,
            string key, 
            Language language, 
            IProgress<double> progress, 
            IProgress<string> status, 
            CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                key = await PrepareKey(key, language, progress, status, token);
                var amountOfBlocks = content.Length / key.Length;
                var blocks = new List<string>(amountOfBlocks);
                status.Report("Разбиение на блоки...");
                for (int i = 0; i < content.Length; i += key.Length)
                    blocks.Add(content.Substring(i, key.Length));
                var result = new List<string>();
                blocks.Reverse();
                for (int i = 0; i < blocks.Count; i++)
                {
                    var block = blocks[i];
                    var temporary = string.Empty;
                    if (i == blocks.Count - 1)
                    {
                        temporary = XOR(block, key, progress);
                        status.Report($"Расшифровываем последний блок {block}\nРезультат {temporary}");
                    }
                    else
                    {
                        temporary = XOR(block, blocks[i + 1], progress);
                        status.Report($"Расшифровываем блок {block}\nРезультат {temporary}");
                    }
                    result.Add(temporary);
                }
                status.Report("Переводим биты в текст...");
                result.Reverse();
                var decoded =  await _bitConverter.DecodeAsync(string.Join(string.Empty,result), progress, token);
                status.Report($"Расшифровка прошла успешно. Искомое слово - {decoded}");
                return decoded;
            }, token);
        }
        private bool AlreadyInBits(string key)
        {
            string pattern = "^[01]+$";
            return (Regex.IsMatch(key, pattern));
        }
        private string XOR(string first, string second, IProgress<double> progress)
        {
            if (first.Length != second.Length)
                throw new ArgumentException();
            var result = new StringBuilder();
            var lastTime = DateTime.Now;
            for(int i=0; i <first.Length; i++) 
            {
                if(DateTime.Now - lastTime > _updateTime)
                {
                    lastTime = DateTime.Now;
                    progress.Report(i / first.Length);
                }
                var currentBitFirst = byte.Parse(first[i].ToString());
                var currentBitSecond = byte.Parse(second[i].ToString());
                result.Append((currentBitFirst + currentBitSecond) % 2);
            }
            return result.ToString();

        }
        private bool ValidateContent(string content, Language language)
        {
            Func<char, bool> validator = language.Equals(Language.Russian) ?
                  Alphabet.IsEnglishLetter : Alphabet.IsRussianLetter;
            return (!content.Any(validator));
        }
        private string ExpandContent(string content, string key)
        {
            var difference = content.Length % key.Length;
            if (difference > 0) difference = key.Length - difference;
            return content.PadRight(content.Length + difference, '0');
        }
        private async Task<string> PrepareKey(string key, Language language,
            IProgress<double> progress, IProgress<string> status, CancellationToken token)
        {
            if (!AlreadyInBits(key))
            {
                if (!ValidateContent(key, language))
                    throw new ArgumentException("Ключ был неверной раскладки");
                status.Report("Перевод ключа в биты...");
                key = await _bitConverter.EncodeAsync(key, progress, token);
            }
            return key;
        }
        public async Task<string> EncodeAsync(string content,
            string key, Language language,
            IProgress<double> progress,
            IProgress<string> status,
            CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                key = await PrepareKey(key, language, progress, status, token);
                status.Report("Генерация итерационного ключа...");
                var ivKey = await _keygen.GenerateAsync(key, progress, token);
                status.Report($"Ключ {ivKey}.");
                status.Report("Сложение ключей...");
                var resultKey = XOR(key, ivKey, progress);
                if (!AlreadyInBits(content))
                {
                    if (!ValidateContent(content, language))
                        throw new ArgumentException("Текст был неверной раскладки");
                    status.Report("Перевод текста в биты...");
                    content = await _bitConverter.EncodeAsync(content, progress, token);
                }
                if (content.Length % resultKey.Length != 0)
                    content = ExpandContent(content, key);
                var amountOfBlocks = content.Length / resultKey.Length;
                var blocks = new List<string>();
                status.Report($"Шифровка каждого из {amountOfBlocks} блоков");
                for (int i = 0; i < content.Length; i += resultKey.Length)
                {
                    var currentBlock = content.Substring(i, resultKey.Length);
                    var temporary = string.Empty;
                    if (blocks.Count == 0)
                    {
                        temporary = XOR(currentBlock, resultKey, progress);
                        status.Report($"Первый блок зашифрованный с ключом {currentBlock}\nРезультат {temporary}");
                    }
                    else
                    {
                        temporary = XOR(currentBlock, blocks.Last(), progress);
                        status.Report($"Следующий блок {currentBlock}\nРезультат {temporary}");
                    }
                    blocks.Add(temporary);
                }
                var result = string.Join(string.Empty, blocks.ToArray());
                status.Report($"Итоговая шифровка {result}.\n Ключ для расшифровки {resultKey}");
                return result;
            }, token);
        }
    }
}

using Caesar_decoder_encoder.Infrastructure.Commands;
using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services;
using Caesar_decoder_encoder.Services.CaesarAlgorithm;
using Caesar_decoder_encoder.Services.Dialogs;
using Caesar_decoder_encoder.Services.Encryption.CaesarAlgorithm;
using Caesar_decoder_encoder.Services.Encryption.FrequencyAnalyze;
using Caesar_decoder_encoder.Services.Encryption.GronsfeldAlgorithm;
using Caesar_decoder_encoder.Services.Encryption.VigenereAlgorithm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Caesar_decoder_encoder.ViewModels
{
    public class EncoderViewModel : ValidationViewModel
    {
		private CancellationTokenSource? _tokenSource;
		private List<string> _previous = new();

		#region Шифр
		private Cipher ParseCipher() => SelectdCipher.Equals("Виженер") ? Cipher.Vigenere : Cipher.Caesar;
		private string _selectedCipher = "Цезарь";

		public string SelectdCipher
		{
			get { return _selectedCipher; }
			set => Set(ref _selectedCipher, value);
		}
        public ObservableCollection<string> Ciphers { get; set; }
        #endregion

        private double _progressValue;
		public double ProgressValue
		{
			get { return _progressValue; }
			set => Set(ref _progressValue, value);
		}
        #region Редактируема
        private bool _editable = true;

        public bool Editable
		{
			get { return _editable; }
			set => Set(ref _editable, value);
		}

        #endregion

        #region содержание
        private string _content = string.Empty;
		public string Content
		{
			get { return _content; }
			set  
			{
				
				RemoveErrors();
				Set(ref _content, value);
			}
		}
        #endregion
        #region Зашифровать
        public ICommand Encode { get; set; }
		private bool CanEncodeExecuted(object? p) => true;
        private async void OnEncodeExecuted(object? p)
        {
            try
            {
                if (!(p is string))
                    throw new ArgumentException();
                var multiply = (string)p;
                string errorMessage = string.Empty;
                if (!ValidateContent(out errorMessage)) return;
                string result = string.Empty;
                var language = ParseLanguage();
                var progress = new Progress<double>(v => ProgressValue = v);
                _tokenSource = new CancellationTokenSource();
                Editable = false;
                if (SelectdCipher == "Цезарь")
                {
                    var key = BigInteger.Parse(Key);
                    Func<string, BigInteger, Language,
                        IProgress<double>, CancellationToken, Task<string>> action =
                        multiply == "positive" ? _cipher.EncodeAsync : _cipher.DecodeAsync;
                    result = await action(Content, key, language, progress, _tokenSource.Token);
                }
                else if (SelectdCipher == "Виженер")
                {
                    Func<string, string, Language,
                       IProgress<double>, CancellationToken, Task<string>> action =
                       multiply == "positive" ? _vigCipher.EncodeAsync : _vigCipher.DecodeAsync;
                    result = await action(Content, Key, language, progress, _tokenSource.Token);
                }
                else if (SelectdCipher == "Гронсфельд")
                {
                    Func<string, string, Language,
                       IProgress<double>, CancellationToken, Task<string>> action =
                       multiply == "positive" ? _gronCipher.EncodeAsync : _gronCipher.DecodeAsync;
                    result = await action(Content, Key, language, progress, _tokenSource.Token);
                }
                else
                    throw new ArgumentException(nameof(SelectdCipher));
                _previous.Add(Content);
                Content = result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _dialogs.ShowError(ex.Message);
            }
            finally
            {
                Editable = true;
                _tokenSource?.Dispose();
                _tokenSource = null;
            }
        }
        private bool ValidateContent(out string errorMessage)
		{
            if (!CheckContent(out errorMessage))
            {
                _dialogs.ShowError(errorMessage);
				return false;
            }
            if (!CheckKey(out errorMessage))
            {
                _dialogs.ShowError(errorMessage);
				return false;
            }
			return true;
        }
        #endregion
        #region валидация
        private bool CheckContent(out string message)
		{
			message = string.Empty;
            Func<char, bool> validator = SelectedLanguage.Equals("Russian") ?
                    Alphabet.IsEnglishLetter : Alphabet.IsRussianLetter;
			if (!Content.Any(validator))
				return true;
			message = "Текст содержит символы другой раскладки";
			return false;
        }
		private bool CheckKey(out string message)
		{
			if (SelectdCipher == "Цезарь")
			{
                message = "Число должно быть целым";
                string pattern = @"^-?\d+$";
				return Regex.IsMatch(Key, pattern);
			}
			else if(SelectdCipher == "Виженер")
			{
				message = "Ключ содержит буквы другой раскладки";
				if (SelectedLanguage == "Russian" && Key.Any(c => !Alphabet.IsRussianLetter(c)))
					return false;
				else if (SelectedLanguage == "English" && Key.Any(c => !Alphabet.IsEnglishLetter(c)))
					return false;
				return true;
			}
			else if(SelectdCipher == "Гронсфельд")
			{
				message = "Число должно быть целым и неотрицательным";
				return Regex.IsMatch(Key, @"^\d+$");
			}
			else
				throw new ArgumentException(nameof(SelectdCipher));
        }
        #endregion
       
		#region ключ
		private string _key = string.Empty;

		public string Key
		{
			get => _key;
			set 
			{
				
                RemoveErrors();
				Set(ref _key, value); 
			}
		}
        #endregion
        #region выбранный язык
        private Language ParseLanguage() =>
            SelectedLanguage.Equals("Russian") ? Language.Russian : Language.English;
        private string _language = "Russian";

		public string SelectedLanguage
		{
			get => _language;
			set => Set(ref _language, value);
		}
		public ObservableCollection<string> Languages { get; set; }
        #endregion
        #region сервисы
        private readonly ICaesarCipher _cipher;
        private readonly IFrequencyAnalyzator _analyzator;
        private readonly GronsfeldCipher _gronCipher;
        private readonly VigenereCipher _vigCipher;
        private readonly IUserDialogs _dialogs;
        #endregion
        #region команда возвращения
        public ICommand ShowPrevious { get; }
		private void OnShowPreviousExecuted(object? p)
		{
			var elem = _previous.Last();
			_previous.RemoveAt(_previous.Count - 1);
			Content = elem;
		}
		private bool CanShowPreviousExecuted(object? p) => _previous.Count >= 1;
        #endregion
        #region команда открытия окна дешифровки
        public ICommand OpenDecoderCommand { get; }
		private void OnOpenDecoderExecuted(object? p)
		{
			int decodedKey = 0;
			var language = ParseLanguage();
			string errorMessage = string.Empty;
			if (!ValidateContent(out errorMessage)) return;
			if (_dialogs.ShowDecodeWindow(ref _content, language, ref decodedKey))
			{
				var message = $"Ваш текст расшифрован. Ключ {decodedKey}";
				Key = (decodedKey * -1).ToString();
				_dialogs.ShowInfo(message);
				OnPropertyChanged(nameof(Content));
			}
		}
		private bool CanOpenDecoderExecuted(object? p) => true;
        #endregion
        #region команда выгрузки из файла
        public ICommand LoadContentCommand { get; }
		private async void OnLoadContentExecuted(object? p)
		{
			string fileName = string.Empty;
			string filter = "Текстовые файлы (*.txt)|*.txt";

			if (!_dialogs.SelectFile(out fileName, filter))
				return;
			try
			{
				using var fs = File.OpenRead(fileName);
				using var streamReader = new StreamReader(fs);
				var fileContent = await streamReader.ReadToEndAsync();
				Content = fileContent;
			}
			catch(Exception ex)
			{
				_dialogs.ShowError($"Произошла ошибка: {ex.Message}");
			}
		}
		private bool CanLoadContentExecuted(object? p) => true;
        #endregion
        #region команда дешифрования путем частотного анализа
        public ICommand DecodeWithFrequemcyCommand { get; }
		private async void OnDecodeCommandExecuted(object? p)
		{
			string errorMessage = string.Empty;
			if (!ValidateContent(out errorMessage))
				return;
			var progress = new Progress<(string,double)>(p => ProgressValue = p.Item2);
			var result = await _analyzator.GetDecodeAsync(Content, ParseLanguage(), progress);
			_dialogs.ShowInfo("Частотный анализ выполнен. Возможный вариант представлен");
			Content = result;
		}
		private bool CanDecodeCommandExecuted(object? p) => true;
        #endregion
        public EncoderViewModel(ICaesarCipher cipher, IUserDialogs dialogs, 
			VigenereCipher vigCipher, 
			GronsfeldCipher gronCipher, 
			IFrequencyAnalyzator analyzator)
		{
			DecodeWithFrequemcyCommand = new RelayCommand(OnDecodeCommandExecuted, CanDecodeCommandExecuted);
			LoadContentCommand = new RelayCommand(OnLoadContentExecuted, CanLoadContentExecuted);
			OpenDecoderCommand = new RelayCommand(OnOpenDecoderExecuted, CanOpenDecoderExecuted);
			Encode = new RelayCommand(OnEncodeExecuted, CanEncodeExecuted);
			ShowPrevious = new RelayCommand(OnShowPreviousExecuted, CanShowPreviousExecuted);
			Ciphers = new() { SelectdCipher, "Виженер", "Гронсфельд" };
			Languages = new() { SelectedLanguage, "English" };
			_cipher = cipher;
			_analyzator = analyzator;
			_gronCipher = gronCipher;
			_vigCipher = vigCipher;
			_dialogs = dialogs;
		}
	}
}

using Caesar_decoder_encoder.Infrastructure.Commands;
using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services;
using Caesar_decoder_encoder.Services.CaesarAlgorithm;
using Caesar_decoder_encoder.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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
		private Language ParseLanguage() =>
			SelectedLanguage.Equals("Russian") ? Language.Russian : Language.English;
        private double _progressValue;
        #region Редактируема
        private bool _editable = true;

        public bool Editable
		{
			get { return _editable; }
			set => Set(ref _editable, value);
		}

		public double ProgressValue
		{
			get { return _progressValue; }
			set => Set(ref _progressValue, value);
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
			message = string.Empty;
            string pattern = @"^-?\d+$";
            if (!Regex.IsMatch(Key, pattern))
			{
				message = "Число должно быть целым";
				return false;
			}         
			return true;
        }
        #endregion
        private async void OnEncodeExecuted(object? p)
		{
			try
			{
				if (!(p is string))
					throw new ArgumentException();
				var multiply = (string)p;
				var multiplier = multiply == "positive" ? 1 : -1;
				string errorMessage = string.Empty;
				if (!ValidateContent(out errorMessage)) return;
                var key = BigInteger.Parse(Key) * multiplier;
				Editable = false;
                _tokenSource = new CancellationTokenSource();
                var language = ParseLanguage();
                var progress = new Progress<double>(v => ProgressValue = v);
                string result = await _cipher.EncodeAsync(Content, key, language, progress, _tokenSource.Token);
                _previous.Add(Content);
                Content = result;
            }
			catch(Exception ex)
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
        private string _language = "Russian";

		public string SelectedLanguage
		{
			get => _language;
			set => Set(ref _language, value);
		}
		#endregion
		public ObservableCollection<string> Languages { get; set; }
        private readonly ICaesarCipher _cipher;
        private readonly IUserDialogs _dialogs;
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
        public EncoderViewModel(ICaesarCipher cipher, IUserDialogs dialogs)
		{
			OpenDecoderCommand = new RelayCommand(OnOpenDecoderExecuted, CanOpenDecoderExecuted);
			Encode = new RelayCommand(OnEncodeExecuted, CanEncodeExecuted);
			ShowPrevious = new RelayCommand(OnShowPreviousExecuted, CanShowPreviousExecuted);
			Languages = new() { SelectedLanguage, "English" };
			_cipher = cipher;
			_dialogs = dialogs;
		}
	}
}

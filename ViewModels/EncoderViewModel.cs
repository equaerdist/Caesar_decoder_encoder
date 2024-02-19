using Caesar_decoder_encoder.Infrastructure.Commands;
using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services.CaesarAlgorithm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caesar_decoder_encoder.ViewModels
{
    public class EncoderViewModel : BaseViewModel
    {
		private CancellationTokenSource? _tokenSource;
		private List<string> _previous = new();
        #region проверка на соответствие
        static bool IsRussianLetter(char ch)
        {
            return (ch >= 'а' && ch <= 'я') || (ch >= 'А' && ch <= 'Я') || ch == 'ё' || ch == 'Ё';
        }

        static bool IsEnglishLetter(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }
        #endregion
        private double _progressValue;
		private bool _editable = true;
        #region Редактируема
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
				var validator = SelectedLanguage.Equals("Russian") ? IsRussianLetter : IsEnglishLetter;
				if (value.All(validator))
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
				Editable = false;
				_tokenSource = new CancellationTokenSource();
				var language = SelectedLanguage.Equals("Russian") ? Language.Russian : Language.English;
				var progress = new Progress<double>();
				progress.ProgressChanged += (s, v) => ProgressValue = v;
				string result = await _cipher.EncodeAsync(Content, Key, language, progress, _tokenSource.Token);
				_previous.Add(Content);
				Content = result;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
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
        private BigInteger _key = new BigInteger(5);

		public BigInteger Key
		{
			get => _key;
			set => Set(ref _key, value);
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
		public ObservableCollection<string> Languages;
        private readonly ICaesarCipher _cipher;
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
        public EncoderViewModel(ICaesarCipher cipher)
		{
			Encode = new RelayCommand(OnEncodeExecuted, CanEncodeExecuted);
			ShowPrevious = new RelayCommand(OnShowPreviousExecuted, CanShowPreviousExecuted);
			Languages = new() { SelectedLanguage };
			Languages.Add("English");
			_cipher = cipher;
		}
	}
}

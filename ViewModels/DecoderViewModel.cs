using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.Services.CaesarAlgorithm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.ViewModels
{
    public class DecoderViewModel : BaseViewModel
    {
        private readonly ICaesarCipher _cipher;
		private Dictionary<int, string> _decodedContent = new();
		#region ключи
		private int _selectedKey;

		public int SelectedKey
		{
			get { return _selectedKey; }
			set 
			{ 
				if (!Set(ref _selectedKey, value)) return;
				if(_decodedContent.ContainsKey(SelectedKey))
				{
					Content = _decodedContent[SelectedKey];
					return;
				}
				DecodeAsync();
			}
		}
		private async Task DecodeAsync()
		{
			var progress = new Progress<double>(p => { });
			var result = await _cipher.EncodeAsync(StartText, SelectedKey, Language, progress).ConfigureAwait(false);
			Content = result;
			_decodedContent[SelectedKey] = result;
			
		}
		private List<int> _keys = new() { 0 };
        public List<int> Keys 
		{ 
			get => _keys; 
			set 
			{
				Set(ref _keys, value);  
				SelectedKey = _keys.First(); 
			} 
		}
        #endregion
        #region текст
        private string _content = null!;

		public string Content
		{
			get { return _content; }
			set { Set(ref _content, value); }
		}
		public string StartText { get; set; } = null!;
		#endregion
		private Language _language;
		public Language Language
		{
			get => _language;
			set
			{
				_language = value;
				var alphabetPower = _language == Language.Russian ? _cipher.RussianAlphabetPower : _cipher.EnglishAlphabetPower;
				Keys = Enumerable.Range(1, alphabetPower).ToList();
				OnPropertyChanged(nameof(Keys));
			}
		}

        public DecoderViewModel(ICaesarCipher cipher)
		{
            _cipher = cipher;
		}


	}
}

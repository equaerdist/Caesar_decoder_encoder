using Caesar_decoder_encoder.Infrastructure.Commands;
using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caesar_decoder_encoder.ViewModels
{
    public class EncoderViewModel : BaseViewModel
    {
		private List<string> _previous = new();
        #region содержание
        private string _content = string.Empty;
		public string Content
		{
			get { return _content; }
			set => Set(ref _content, value);
		}
        #endregion
        #region Зашифровать
        public ICommand Encode { get; set; }
		private bool CanEncodeExecuted(object? p) => true;
		private void OnEncodeExecuted(object? p)
		{

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

		public string Language
		{
			get => _language;
			set => Set(ref _language, value);
		}
		#endregion
		public ObservableCollection<string> Languages;

        public EncoderViewModel()
		{
			Encode = new RelayCommand(OnEncodeExecuted, CanEncodeExecuted);
			Languages = new() { Language };
			Languages.Add("English");
		}
	}
}

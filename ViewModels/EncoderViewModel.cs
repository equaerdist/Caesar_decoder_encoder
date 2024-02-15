using Caesar_decoder_encoder.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caesar_decoder_encoder.ViewModels
{
    class EncoderViewModel : BaseViewModel
    {
		private string _content = string.Empty;

		public string Content
		{
			get { return _content; }
			set => Set(ref _content, value);
		}
		public ICommand Encode { get; set; }
		private bool CanEncodeExecuted(object? p) => true;
		private void OnEncodeExecuted(object? p)
		{

		}
		public EncoderViewModel()
		{
			Encode = new RelayCommand(OnEncodeExecuted, CanEncodeExecuted);
		}
	}
}

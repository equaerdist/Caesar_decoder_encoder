using Caesar_decoder_encoder.Models;
using Caesar_decoder_encoder.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace Caesar_decoder_encoder.Services.Dialogs
{
    public class UserDialogs : IUserDialogs
    {
        public bool SelectFile(out string fileName, string filters)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Directory.GetLogicalDrives().First();
            dialog.Filter = filters;
            fileName = string.Empty;
            if(dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
                return true;
            }
            return false;
        }

        public bool ShowDecodeWindow(ref string content,Language language, ref int decodedKey)
        {
            var window = new Decoder();
            var decoderViewModel = ViewModelLocator.DecoderViewModel;
            decoderViewModel.Content = content;
            decoderViewModel.StartText = content;
            decoderViewModel.Language = language;
            window.DataContext = decoderViewModel;
            var result = window.ShowDialog();
            if(result == true)
            {
                content = decoderViewModel.Content;
                decodedKey = decoderViewModel.SelectedKey;
                return true;
            }
            return false;
        }

        public void ShowError(string message) =>
            MessageBox.Show(message, "modal", MessageBoxButton.OK, MessageBoxImage.Error);

        public void ShowInfo(string message) => 
            MessageBox.Show(message, "modal", MessageBoxButton.OK, MessageBoxImage.Information);
      
    }
}

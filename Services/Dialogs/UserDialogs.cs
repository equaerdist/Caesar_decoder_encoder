using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caesar_decoder_encoder.Services.Dialogs
{
    public class UserDialogs : IUserDialogs
    {
        public void ShowError(string message) =>
            MessageBox.Show(message, "modal", MessageBoxButton.OK, MessageBoxImage.Error);
      
    }
}

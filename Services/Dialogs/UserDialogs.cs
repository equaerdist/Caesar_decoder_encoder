using Caesar_decoder_encoder.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Dialogs
{
    public class UserDialogs : IUserDialog
    {
        public void ShowAlphabetChoice(out string choice)
        {
            var dialog = new AlphabetChoice();
            var result = dialog.ShowDialog();
            var language = result == true ? "Russian" : "English";
            choice = language;
        }
    }
}

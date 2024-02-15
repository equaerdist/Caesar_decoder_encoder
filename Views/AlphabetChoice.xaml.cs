using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Caesar_decoder_encoder.Views
{
    /// <summary>
    /// Логика взаимодействия для AlphabetChoice.xaml
    /// </summary>
    public partial class AlphabetChoice : Window
    {
        public string Alphabet { get; set; } = string.Empty;
        public AlphabetChoice()
        {
            InitializeComponent();
        }
    }
}

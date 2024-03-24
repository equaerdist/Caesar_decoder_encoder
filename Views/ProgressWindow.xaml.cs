using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {


        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", 
                typeof(double), typeof(ProgressWindow), new PropertyMetadata(double.NaN, onProgressChanged));

        private static void onProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => 
            ((ProgressWindow)d).Progress_view.Value = (double)e.NewValue;

        private IProgress<double> _progress = null!;

        public IProgress<double> Progress
        {
            get => _progress ??= new Progress<double>(value => ProgressValue = value);
        }
        private IProgress<string> _status = null!;

        public IProgress<string> Status
        {
            get => _status ??= new Progress<string>(p => Content_box.Text += $"\n{p}\n");
        }
        private string _content = string.Empty;

        public string ContentBox
        {
            get { return _content; }
            set { _content = value; }
        }
        private CancellationTokenSource _source = new CancellationTokenSource();
        public CancellationToken Token => _source.Token;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _source.Cancel();
            _source.Dispose();
        }

        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

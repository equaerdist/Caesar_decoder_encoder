using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.ViewModels
{
    public class ViewModelLocator
    {
        public static EncoderViewModel MainViewModel => App.HostInstance.Services.GetRequiredService<EncoderViewModel>();
        public static DecoderViewModel DecoderViewModel => App.HostInstance.Services.GetRequiredService<DecoderViewModel>();
    }
}

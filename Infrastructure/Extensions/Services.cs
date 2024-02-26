using Caesar_decoder_encoder.Services.CaesarAlgorithm;
using Caesar_decoder_encoder.Services.Dialogs;
using Caesar_decoder_encoder.Services.Encryption.CaesarAlgorithm;
using Caesar_decoder_encoder.Services.Encryption.VigenereAlgorithm;
using Caesar_decoder_encoder.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Infrastructure.Extensions
{
    static class Services
    {
        public static IServiceCollection Configure(this IServiceCollection services)
        {
            return services.AddSingleton<ICaesarCipher, CaesarCipher>()
                .AddSingleton<EncoderViewModel>()
                .AddTransient<DecoderViewModel>()
                .AddSingleton<IVigenereCipher, VigenereAlgorithm>()
                .AddSingleton<IUserDialogs, UserDialogs>();
        }
    }
}

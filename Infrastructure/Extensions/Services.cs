using Caesar_decoder_encoder.Services.Dialogs;
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
            return services.AddSingleton<IUserDialog, UserDialogs>();
        }
    }
}

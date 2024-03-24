﻿using Caesar_decoder_encoder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Dialogs
{
    public interface IUserDialogs
    {
        void ShowError(string message);
        bool ShowDecodeWindow(ref string content, Language language, ref int decodedKey);
        void ShowInfo(string message);
        bool SelectFile(out string fileName, string filters);
        (IProgress<double>, IProgress<string>, CancellationToken, Action) ShowProgress(string windowName);

    }
}

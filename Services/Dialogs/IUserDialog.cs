﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Services.Dialogs
{
    public interface IUserDialog
    {
        void ShowAlphabetChoice(out string choice);
    }
}

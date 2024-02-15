using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.Infrastructure.Commands
{
    class RelayCommand : BaseCommand
    {
        private Action<object?> _execute;
        private Func<object?, bool> _canExecute;
        public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute) 
        {
            _execute = execute; 
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter) => _canExecute(parameter);


        public override void Execute(object? parameter) => _execute(parameter);
    }
}

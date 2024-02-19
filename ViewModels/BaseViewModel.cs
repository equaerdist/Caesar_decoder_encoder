using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Caesar_decoder_encoder.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var actions = PropertyChanged?.GetInvocationList();
            if (actions is null)
                return;
            foreach(var action in actions)
            {
                if(action?.Target is DispatcherObject dispatcherObject)
                    dispatcherObject.Dispatcher.Invoke(
                        action, 
                        this,
                        new PropertyChangedEventArgs(propertyName));
                else
                    action?.DynamicInvoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if(Equals(oldValue, newValue)) return false;
            oldValue = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

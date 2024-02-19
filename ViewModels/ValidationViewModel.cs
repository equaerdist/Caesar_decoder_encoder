using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Caesar_decoder_encoder.ViewModels
{
    public class ValidationViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        protected readonly Dictionary<string, List<string>> _errors = new();
        protected virtual void OnErorrsChanged([CallerMemberName] string? propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new(propertyName));
        }
        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName is not null && _errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return Enumerable.Empty<string>();
        }
        protected virtual void AddError(string errorMessage, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null)
                throw new ArgumentNullException();
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new();
            _errors[propertyName].Add(errorMessage);
            OnErorrsChanged(propertyName);
        }
        protected virtual void RemoveErrors([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null)
                throw new ArgumentNullException();
            if (_errors.ContainsKey(propertyName))
            {
                _errors[propertyName].Clear();
                OnErorrsChanged(propertyName);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KooliProjekt.WpfApplication
{
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _executeAsync;
        private readonly Func<T, bool> _canExecute;
        private readonly Action<Exception> _onError;
        private bool _isExecuting;

        public RelayCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute, Action<Exception> onError)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            _onError = onError ?? throw new ArgumentNullException(nameof(onError));
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && _canExecute(GetParameter(parameter));
        }

        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _executeAsync(GetParameter(parameter));
            }
            catch (Exception exception)
            {
                _onError(exception);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private static T GetParameter(object parameter)
        {
            return parameter is T value ? value : default;
        }
    }
}

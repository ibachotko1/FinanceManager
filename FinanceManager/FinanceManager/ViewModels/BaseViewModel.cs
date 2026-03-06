using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FinanceManager.ViewModels
{
    // Базовый VM: уведомления для привязок (WPF bindings)
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            // propName берётся автоматически, чтобы не писать строки руками
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
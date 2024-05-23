using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reactive.Bindings;
using System;
using System.Windows.Input;

namespace WpfApp1
{
    internal class UnlimitedSelectionItemViewModel : ObservableObject
    {
        public object? Value { get; }

        public ReactivePropertySlim<bool> IsSelected { get; set; } = new();

        public ICommand SelectedCommand { get; }

        public event EventHandler? Selected;

        public UnlimitedSelectionItemViewModel(object? value, ICommand filterCommand)
        {
            this.Value = value;
            this.SelectedCommand = filterCommand;
        }
    }
}

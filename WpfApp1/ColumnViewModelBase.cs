using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace WpfApp1
{
    internal abstract class ColumnViewModelBase : ObservableObject
    {
        public virtual ReactivePropertySlim<string?> DisplayMenber { get; } = new();
        public virtual object? CellTemplateResourceKey { get; } = null;

        public ReactivePropertySlim<bool> IsSorting { get; set; } = new();

        public ReactivePropertySlim<ListSortDirection?> SortDirection { get; set; } = new();

        public abstract ReactivePropertySlim<bool> IsFiltering { get; }

        public ReactivePropertySlim<bool> IsGrouping { get; set; } = new();

        public ICommand SortCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand GroupCommand { get; }
        public ICommand ResetFilterAndGroupCommand { get; }

        public event EventHandler<ListSortDirection?>? SortRequested;
        public event EventHandler? FilterRequested;
        public event EventHandler? GroupingRequested;

        public ColumnViewModelBase()
        {
            this.SortCommand = new RelayCommand<ListSortDirection?>(this.SortCommandExecute);
            this.FilterCommand = new RelayCommand(this.FilterCommandExecute);
            this.GroupCommand = new RelayCommand(this.GroupCommandExecute);
            this.ResetFilterAndGroupCommand = new RelayCommand(this.ResetFilterCommandExecute);
        }

        public SortDescription Sort(ListSortDirection direction)
        {
            this.IsSorting.Value = true;
            this.SortDirection.Value = direction;
            return this.SortOverride(direction);
        }

        protected abstract SortDescription SortOverride(ListSortDirection direction);

        public void ResetSort()
        {
            this.IsSorting.Value = false;
            this.SortDirection.Value = null;
        }

        public bool Filter(object itemVm)
        {
            return this.FilterOverride(itemVm);
        }
        protected abstract bool FilterOverride(object itemVm);

        public GroupDescription Group()
        {
            return this.GroupOverride();
        }

        public abstract GroupDescription GroupOverride();

        public void ResetGroup()
        {
            this.IsGrouping.Value = false;
        }

        private void SortCommandExecute(ListSortDirection? sortDirection)
        {
            this.SortRequested?.Invoke(this, sortDirection);
        }

        private void FilterCommandExecute()
        {
            this.FilterRequested?.Invoke(this, EventArgs.Empty);
        }

        private void GroupCommandExecute()
        {
            this.GroupingRequested?.Invoke(this, EventArgs.Empty); ;
        }

        private void ResetFilterCommandExecute()
        {
            var isThisFiltering = this.IsFiltering.Value;
            var isThisGoruping = this.IsGrouping.Value;
            this.ResetFilterAndGroupCommandExecuteOverride();
            if (isThisFiltering)
            {
                this.FilterRequested?.Invoke(this, EventArgs.Empty);
            }
            if (isThisGoruping)
            {
                this.GroupingRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void ResetFilterAndGroupCommandExecuteOverride() { }
    }
}

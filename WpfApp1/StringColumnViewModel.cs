using Reactive.Bindings;
using System;
using System.ComponentModel;

namespace WpfApp1
{
    internal class StringColumnViewModel : ColumnViewModelBase
    {
        private string propertyName;

        public override ReactivePropertySlim<string?> DisplayMenber => new(this.propertyName);
        public ReactivePropertySlim<string> HeaderText { get; }

        public ReactivePropertySlim<string> FilterText { get; set; }

        public override ReactivePropertySlim<bool> IsFiltering => new(!string.IsNullOrEmpty(this.FilterText.Value));

        public StringColumnViewModel(string propertyName, string headerText)
        {
            this.propertyName = propertyName;
            this.HeaderText = new(headerText);
            FilterText = new(string.Empty);
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription($"{this.propertyName}.{nameof(StringViewModel.Value)}.{nameof(StringViewModel.Value.Value)}", direction);
        }

        protected override bool FilterOverride(object itemVm)
        {
            return
                (itemVm.GetType().GetProperty(this.propertyName)!.GetValue(itemVm) as StringViewModel)?.Filter(this.FilterText.Value)
                ?? false;
        }

        public override GroupDescription GroupOverride()
        {
            throw new NotImplementedException();
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.FilterText.Value = string.Empty;
        }
    }
}

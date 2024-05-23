using Reactive.Bindings;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp1
{
    internal class BloodTypeColumnViewModel : ColumnViewModelBase
    {
        public override object CellTemplateResourceKey => "BloodTypeCell";

        public ReactivePropertySlim<bool> ShowAType { get; set; } = new();

        public ReactivePropertySlim<bool> ShowBType { get; set; } = new();

        public ReactivePropertySlim<bool> ShowABType { get; set; } = new();

        public ReactivePropertySlim<bool> ShowOType { get; set; } = new();

        public override ReactivePropertySlim<bool> IsFiltering => new(
            this.ShowAType.Value || this.ShowBType.Value ||
            this.ShowABType.Value || this.ShowOType.Value);

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(nameof(PersonViewModel.BloodType), direction);
        }

        protected override bool FilterOverride(object itemVm)
        {
            if (itemVm is not PersonViewModel pvm) { return false; }
            if (!this.IsFiltering.Value) { return true; }
            return (this.ShowAType.Value && pvm.BloodType == BloodType.A) ||
                (this.ShowBType.Value && pvm.BloodType == BloodType.B) ||
                (this.ShowABType.Value && pvm.BloodType == BloodType.AB) ||
                (this.ShowOType.Value && pvm.BloodType == BloodType.O); ;
        }

        public override GroupDescription GroupOverride()
        {
            return new BloodTypeGroupDescription();
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.ShowAType.Value = false;
            this.ShowBType.Value = false;
            this.ShowABType.Value = false;
            this.ShowOType.Value = false;
            this.IsGrouping.Value = false;
        }

        public class BloodTypeGroupDescription : GroupDescription
        {
            public BloodTypeGroupDescription()
            {
                this.CustomSort = new BloodTypeComparer();
            }

            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                if (item is not PersonViewModel pvm) { throw new ArgumentException(nameof(item)); }
                var title = pvm.BloodType switch
                {
                    BloodType.A => "A型",
                    BloodType.B => "B型",
                    BloodType.AB => "AB型",
                    BloodType.O => "O型",
                };
                return new GroupHeaderViewModel(pvm.BloodType, title);
            }

            private class BloodTypeComparer : IComparer
            {
                public int Compare(object? x, object? y)
                {
                    return (x, y) switch
                    {
                        (CollectionViewGroup gx, CollectionViewGroup gy) =>
                            ((BloodType)((GroupHeaderViewModel)gx.Name).Value).CompareTo((BloodType)((GroupHeaderViewModel)gy.Name).Value),
                        _ => throw new ArgumentException(),
                    };
                }
            }
        }
    }
}

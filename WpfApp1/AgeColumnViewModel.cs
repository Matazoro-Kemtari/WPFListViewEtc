using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp1
{
    internal sealed class AgeColumnViewModel : ColumnViewModelBase, IDisposable
    {
        public override object CellTemplateResourceKey => "AgeCell";
        private ObservableCollection<PersonViewModel> persons;

        public ReactivePropertySlim<bool> UnderTen { get; set; } = new();

        public ReactivePropertySlim<bool> TeenAgers { get; set; } = new();

        public ReactivePropertySlim<bool> Twenties { get; set; } = new();

        public ReactivePropertySlim<bool> Thirties { get; set; } = new();

        public ReactivePropertySlim<bool> Fourties { get; set; } = new();

        public ReactivePropertySlim<bool> Fifties { get; set; } = new();

        public ReactivePropertySlim<bool> Sixties { get; set; } = new();

        public ReactivePropertySlim<bool> OverSeventies { get; set; } = new();

        public ReactivePropertySlim<bool> UnderTenExist { get; set; } = new();

        public ReactivePropertySlim<bool> TeenAgersExist { get; set; } = new();

        public ReactivePropertySlim<bool> TwentiesExist { get; set; } = new();

        public ReactivePropertySlim<bool> ThirtiesExist { get; set; } = new();

        public ReactivePropertySlim<bool> FourtiesExist { get; set; } = new();

        public ReactivePropertySlim<bool> FiftiesExist { get; set; } = new();

        public ReactivePropertySlim<bool> SixtiesExist { get; set; } = new();

        public ReactivePropertySlim<bool> OverSeventiesExist { get; set; } = new();

        public override ReactivePropertySlim<bool> IsFiltering => new(
            this.UnderTen.Value || this.TeenAgers.Value ||
            this.Twenties.Value || this.Thirties.Value ||
            this.Fourties.Value || this.Fifties.Value ||
            this.Sixties.Value || this.OverSeventies.Value);

        public AgeColumnViewModel(ObservableCollection<PersonViewModel> persons)
        {
            this.ShowFilters(persons);
            persons.CollectionChanged += this.Persons_CollectionChanged;
            this.persons = persons;
        }

        public void Dispose()
        {
            this.persons.CollectionChanged -= this.Persons_CollectionChanged;
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(nameof(PersonViewModel.Age), direction);
        }

        protected override bool FilterOverride(object itemVm)
        {
            if (itemVm is not PersonViewModel pvm) { return false; }
            if (!this.IsFiltering.Value) { return true; }
            var category = pvm.Age.CategorizeAge();
            return
                (this.UnderTen.Value && category == AgeCategory.UnderTen) ||
                (this.TeenAgers.Value && category == AgeCategory.TeenAgers) ||
                (this.Twenties.Value && category == AgeCategory.Twenties) ||
                (this.Thirties.Value && category == AgeCategory.Thirties) ||
                (this.Fourties.Value && category == AgeCategory.Fourties) ||
                (this.Fifties.Value && category == AgeCategory.Fifties) ||
                (this.Sixties.Value && category == AgeCategory.Sixties) ||
                (this.OverSeventies.Value && category == AgeCategory.OverSeventies);
        }

        public override GroupDescription GroupOverride()
        {
            return new AgeGroupDescription();
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.UnderTen.Value = false;
            this.TeenAgers.Value = false;
            this.Twenties.Value = false;
            this.Thirties.Value = false;
            this.Fourties.Value = false;
            this.Fifties.Value = false;
            this.Sixties.Value = false;
            this.OverSeventies.Value = false;
            this.IsGrouping.Value = false;
        }

        private void Persons_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.ShowFilters((ObservableCollection<PersonViewModel>)sender);
                    break;
            }
        }

        void ShowFilters(ObservableCollection<PersonViewModel> persons)
        {
            foreach (var person in persons)
            {
                switch (person.Age.CategorizeAge())
                {
                    case AgeCategory.UnderTen:
                        this.UnderTenExist.Value = true;
                        break;
                    case AgeCategory.TeenAgers:
                        this.TeenAgersExist.Value = true;
                        break;
                    case AgeCategory.Twenties:
                        this.TwentiesExist.Value = true;
                        break;
                    case AgeCategory.Thirties:
                        this.ThirtiesExist.Value = true;
                        break;
                    case AgeCategory.Fourties:
                        this.FourtiesExist.Value = true;
                        break;
                    case AgeCategory.Fifties:
                        this.FiftiesExist.Value = true;
                        break;
                    case AgeCategory.Sixties:
                        this.SixtiesExist.Value = true;
                        break;
                    case AgeCategory.OverSeventies:
                        this.OverSeventiesExist.Value = true;
                        break;
                }
            }
        }

        public class AgeGroupDescription : GroupDescription
        {
            public AgeGroupDescription()
            {
                this.CustomSort = new AgeComparer();
            }

            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                if (item is not PersonViewModel pvm) { throw new ArgumentException(nameof(item)); }
                var category = pvm.Age.CategorizeAge();
                var title = category switch
                {
                    AgeCategory.UnderTen => "10才未満",
                    AgeCategory.TeenAgers => "10歳台",
                    AgeCategory.Twenties => "20歳台",
                    AgeCategory.Thirties => "30歳台",
                    AgeCategory.Fourties => "40歳台",
                    AgeCategory.Fifties => "50歳台",
                    AgeCategory.Sixties => "60歳台",
                    AgeCategory.OverSeventies => "70才以上",
                };
                return new GroupHeaderViewModel(category, title);
            }

            private class AgeComparer : IComparer
            {
                public int Compare(object? x, object? y)
                {
                    return (x, y) switch
                    {
                        (CollectionViewGroup gx, CollectionViewGroup gy) =>
                            ((AgeCategory)((GroupHeaderViewModel)gx.Name).Value).CompareTo((AgeCategory)((GroupHeaderViewModel)gy.Name).Value),
                        _ => throw new ArgumentException(),
                    };
                }
            }
        }
    }
}

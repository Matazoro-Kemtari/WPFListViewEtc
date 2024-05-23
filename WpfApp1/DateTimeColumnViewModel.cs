using CommunityToolkit.Mvvm.Input;
using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApp1
{
    internal class DateTimeColumnViewModel : ColumnViewModelBase, IDisposable
    {
        private string propertyName;
        private INotifyCollectionChanged? items;
        private ICollection<DateTime> filterRange;

        public override ReactivePropertySlim<string?> DisplayMenber => new(this.propertyName);

        public ReactivePropertySlim<bool> Range { get; set; } = new();

        public ReactivePropertySlim<bool> Today { get; set; } = new();
        public ReactivePropertySlim<bool> Yesterday { get; set; } = new();

        public ReactivePropertySlim<bool> ThisWeek { get; set; } = new();

        public ReactivePropertySlim<bool> LastWeek { get; set; } = new();

        public ReactivePropertySlim<bool> ThisMonth { get; set; } = new();

        public ReactivePropertySlim<bool> LastMonth { get; set; } = new();

        public ReactivePropertySlim<bool> MorePast { get; set; } = new();

        public ReactivePropertySlim<bool> TodayExist { get; set; } = new();

        public ReactivePropertySlim<bool> YesterdayExist { get; set; } = new();

        public ReactivePropertySlim<bool> ThisWeekExist { get; set; } = new();

        public ReactivePropertySlim<bool> LastWeekExist { get; set; } = new();

        public ReactivePropertySlim<bool> ThisMonthExist { get; set; } = new();

        public ReactivePropertySlim<bool> LastMonthExist { get; set; } = new();

        public ReactivePropertySlim<bool> MorePastExist { get; set; } = new();

        public override ReactivePropertySlim<bool> IsFiltering => new(
            this.Range.Value ||
            this.Today.Value || this.Yesterday.Value ||
            this.ThisWeek.Value || this.LastWeek.Value ||
            this.ThisMonth.Value || this.LastMonth.Value ||
            this.MorePast.Value);

        public ICommand SelectFilterRangeComand { get; }

        public DateTimeColumnViewModel(string propertyName, IEnumerable items)
        {
            this.propertyName = propertyName;
            this.filterRange = new List<DateTime> { DateTime.Today };
            this.SelectFilterRangeComand = new RelayCommand<SelectedDatesCollection>(this.SelectFilterRangeComandExecute);

            Range.Subscribe(v =>
            {
                if (v)
                {
                    this.Today.Value = false;
                    this.Yesterday.Value = false;
                    this.ThisWeek.Value = false;
                    this.LastWeek.Value = false;
                    this.ThisMonth.Value = false;
                    this.LastMonth.Value = false;
                    this.MorePast.Value = false;
                }
            });
            Today.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });
            Yesterday.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });
            ThisWeek.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });
            LastWeek.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });
            ThisMonth.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });
            LastMonth.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });
            MorePast.Subscribe(v =>
            {
                if (v)
                {
                    Range.Value = false;
                }
            });

            ShowFilters(items);
            if (items is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged += this.Items_CollectionChanged;
                this.items = ncc;
            }
        }

        public void Dispose()
        {
            if (this.items is not null)
            {
                this.items.CollectionChanged -= this.Items_CollectionChanged;
            }
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(this.propertyName, direction);
        }

        protected override bool FilterOverride(object itemVm)
        {
            if (!this.IsFiltering.Value) { return true; }

            var value = itemVm.GetType().GetProperty(this.propertyName)?.GetValue(itemVm);
            if (value is not DateTime dt) { throw new NotImplementedException(); }
            var category = dt.CategorizeDateTime();
            return
                (this.Range.Value && this.filterRange!.Min().Date <= dt && dt < this.filterRange.Max().Date.AddDays(1)) ||
                (!this.Range.Value && (
                    (this.Today.Value && category == DateTimeCategory.Today) ||
                    (this.Yesterday.Value && category == DateTimeCategory.Yesterday) ||
                    (this.ThisWeek.Value && category == DateTimeCategory.ThisWeek) ||
                    (this.LastWeek.Value && category == DateTimeCategory.LastWeek) ||
                    (this.ThisMonth.Value && category == DateTimeCategory.ThisMonth) ||
                    (this.LastMonth.Value && category == DateTimeCategory.LastMonth) ||
                    (this.MorePast.Value && category == DateTimeCategory.MorePast)
                    ));
        }

        private void SelectFilterRangeComandExecute(SelectedDatesCollection? filterRange)
        {
            if (filterRange is null) { return; }
            this.filterRange = filterRange;
            this.Range.Value = true;
            this.FilterCommand.Execute(null);
        }

        public override GroupDescription GroupOverride()
        {
            return new DateTimeGroupDescription(this.propertyName);
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.Range.Value = false;
            this.Today.Value = false;
            this.Yesterday.Value = false;
            this.ThisWeek.Value = false;
            this.LastWeek.Value = false;
            this.ThisMonth.Value = false;
            this.LastMonth.Value = false;
            this.MorePast.Value = false;
            this.IsGrouping.Value = false;
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    ShowFilters((IEnumerable)sender);
                    break;
            }
        }

        void ShowFilters(IEnumerable items)
        {
            foreach (var item in items.Cast<object>())
            {
                var value = item.GetType().GetProperty(this.propertyName)?.GetValue(item);
                if (value is not DateTime dt) { throw new NotImplementedException(); }
                var category = dt.CategorizeDateTime();
                switch (dt.CategorizeDateTime())
                {
                    case DateTimeCategory.Today:
                        this.TodayExist.Value = true;
                        break;
                    case DateTimeCategory.Yesterday:
                        this.YesterdayExist.Value = true;
                        break;
                    case DateTimeCategory.ThisWeek:
                        this.ThisWeekExist.Value = true;
                        break;
                    case DateTimeCategory.LastWeek:
                        this.LastWeekExist.Value = true;
                        break;
                    case DateTimeCategory.ThisMonth:
                        this.ThisMonthExist.Value = true;
                        break;
                    case DateTimeCategory.LastMonth:
                        this.LastMonthExist.Value = true;
                        break;
                    case DateTimeCategory.MorePast:
                        this.MorePastExist.Value = true;
                        break;
                }
            }
        }

        public class DateTimeGroupDescription : GroupDescription
        {
            private readonly string propertyName;

            public DateTimeGroupDescription(string propertyName)
            {
                this.CustomSort = new DateTimeComparer();
                this.propertyName = propertyName;
            }

            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                var value = item.GetType().GetProperty(this.propertyName)?.GetValue(item);
                if (value is not DateTime dt) { throw new NotImplementedException(); }
                var category = dt.CategorizeDateTime();
                var title = category switch
                {
                    DateTimeCategory.Today => "今日",
                    DateTimeCategory.Yesterday => "昨日",
                    DateTimeCategory.ThisWeek => "今週（今日・昨日を除く）",
                    DateTimeCategory.LastWeek => "先週",
                    DateTimeCategory.ThisMonth => "今月（今週・先週を除く）",
                    DateTimeCategory.LastMonth => "先月",
                    DateTimeCategory.MorePast => "かなり前",
                    _ => throw new NotImplementedException(),
                };
                return new GroupHeaderViewModel(category, title);
            }

            private class DateTimeComparer : IComparer
            {
                public int Compare(object? x, object? y)
                {
                    return (x, y) switch
                    {
                        (CollectionViewGroup gx, CollectionViewGroup gy) =>
                            ((DateTimeCategory)((GroupHeaderViewModel)gx.Name).Value).CompareTo((DateTimeCategory)((GroupHeaderViewModel)gy.Name).Value),
                        _ => throw new ArgumentException(),
                    };
                }
            }
        }
    }
}

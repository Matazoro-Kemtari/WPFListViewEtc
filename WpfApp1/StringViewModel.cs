using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings;

namespace WpfApp1
{
    internal class StringViewModel : ObservableObject
    {
        private readonly string originalString;

        public StringViewModel(string originalString)
        {
            this.originalString = originalString;
            PreviousFilteredText = new(string.Empty);
            FilteredText = new(string.Empty);
            FollowingFilteredText =new( originalString);
        }

        public ReactivePropertySlim<string> Value => new(this.originalString);

        public ReactivePropertySlim<string> PreviousFilteredText { get; set; }

        public ReactivePropertySlim<string> FilteredText { get; set; }

        public ReactivePropertySlim<string> FollowingFilteredText { get; set; }

        public bool Filter(string filterText)
        {
            var index = this.originalString.IndexOf(filterText);
            if (index == -1)
            {
                this.PreviousFilteredText.Value = this.originalString;
                this.FilteredText.Value = string.Empty;
                this.FollowingFilteredText.Value = string.Empty;
                return false;
            }
            else
            {
                this.PreviousFilteredText.Value = this.originalString[..index];
                this.FilteredText.Value = this.originalString.Substring(index, filterText.Length);
                this.FollowingFilteredText.Value = this.originalString[(index + filterText.Length)..];
                return true;
            }
        }
    }
}

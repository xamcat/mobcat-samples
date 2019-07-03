using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.MobCAT.MVVM;
using News.Helpers;

namespace News.ViewModels
{
    /// <summary>
    /// News by source view model.
    /// </summary>
    public class NewsBySourceViewModel : BaseNavigationViewModel
    {
        private int _selectedSourcePosition;

        public SourceNewsViewModel SelectedSource => Sources.Count > _selectedSourcePosition ? Sources[_selectedSourcePosition] : null;

        public int SelectedSourcePosition
        {
            get { return _selectedSourcePosition; }
            set
            {
                if (RaiseAndUpdate(ref _selectedSourcePosition, value))
                {
                    Raise(nameof(SelectedSource));
                    OnSelectedCategoryPositionChanged();
                }
            }
        }

        /// <summary>
        /// Preselected popular list of available sources
        /// </summary>
        public ObservableCollection<SourceNewsViewModel> Sources { get; } = new ObservableCollection<SourceNewsViewModel>()
        {
            new SourceNewsViewModel(Constants.Sources.CNN, "cnn"),
            new SourceNewsViewModel(Constants.Sources.TheNewYorkTimes, "the-new-york-times"),
            new SourceNewsViewModel(Constants.Sources.ABCNews, "abc-news"),
            new SourceNewsViewModel(Constants.Sources.TheWashingtonPost, "the-washington-post"),
            new SourceNewsViewModel(Constants.Sources.FoxNews, "fox-news"),
            new SourceNewsViewModel(Constants.Sources.CBSNews, "cbs-news"),
            new SourceNewsViewModel(Constants.Sources.NBCNews, "nbc-news"),
            new SourceNewsViewModel(Constants.Sources.Reuters, "reuters"),
            new SourceNewsViewModel(Constants.Sources.USAToday, "usa-today"),
            new SourceNewsViewModel(Constants.Sources.Bloomberg, "bloomberg"),
            new SourceNewsViewModel(Constants.Sources.Wired, "wired"),
            new SourceNewsViewModel(Constants.Sources.CNBC, "cnbc"),
        };

        public NewsBySourceViewModel()
        {
        }

        public async override Task InitAsync()
        {
            await SelectedSource.InitAsync();
            await base.InitAsync();
        }

        private void OnSelectedCategoryPositionChanged()
        {
            System.Diagnostics.Debug.WriteLine($"SelectedCategoryPosition changed to {SelectedSourcePosition}");
            SelectedSource.InitAsync().HandleResult();
        }
    }
}
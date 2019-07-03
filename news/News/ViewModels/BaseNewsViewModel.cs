using System;
using System.Threading.Tasks;
using Microsoft.MobCAT.MVVM;
using MobCAT.MVVM;
using News.Helpers;
using News.Models;
using News.Services.Abstractions;
using Xamarin.Forms;

namespace News.ViewModels
{
    /// <summary>
    /// Base news view model which manages list of articles with lazy loading ability regardless of the data source.
    /// </summary>
    public abstract class BaseNewsViewModel : BaseNavigationViewModel
    {
        private bool _isRefreshing;
        private bool _isLoadingMore;
        private bool _initialized;

        public INewsDataService NewsDataService { get; } = DependencyService.Resolve<INewsDataService>();

        public VirtualCollection<ArticleViewModel> Articles { get; } = new VirtualCollection<ArticleViewModel>();

        public bool IsEmpty => _initialized && Articles.IsNullOrEmpty();

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                if (RaiseAndUpdate(ref _isRefreshing, value))
                {
                    LoadMoreCommand.ChangeCanExecute();
                    RefreshCommand.ChangeCanExecute();
                }
            }
        }

        public bool IsLoadingMore
        {
            get { return _isLoadingMore; }
            set
            {
                if (RaiseAndUpdate(ref _isLoadingMore, value))
                {
                    LoadMoreCommand.ChangeCanExecute();
                    RefreshCommand.ChangeCanExecute();
                }
            }
        }

        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand LoadMoreCommand { get; }

        public Microsoft.MobCAT.MVVM.Command<ArticleViewModel> ArticleSelectedCommand { get; }

        public BaseNewsViewModel()
        {
            RefreshCommand = new AsyncCommand(OnRefreshCommandExecutedAsync, () => !IsRefreshing && !IsLoadingMore);
            LoadMoreCommand = new AsyncCommand(OnLoadMoreCommandExecuted, () => !IsRefreshing && !IsLoadingMore && !Articles.FullyLoaded);
            ArticleSelectedCommand = new Microsoft.MobCAT.MVVM.Command<ArticleViewModel>(OnArticleSelectedCommandExecuted);
        }

        public async override Task InitAsync()
        {
            await base.InitAsync();
            await InitNewsAsync();
            _initialized = true;
        }

        private async Task InitNewsAsync(bool forceRefresh = false)
        {
            if (!Articles.IsNullOrEmpty() && !forceRefresh)
                return;

            var articles = await FetchArticlesAsync();
            if (articles != null)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Articles.Clear();
                    LoadArticlesPage(articles);
                });
            }
        }

        private async Task LoadMoreAsync()
        {
            if (Articles.FullyLoaded)
                return;
                
            var articles = await FetchArticlesAsync(Articles.VirtualPage + 1, Articles.VirtualPageSize);
            if (articles?.Articles != null)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    LoadArticlesPage(articles);
                });
            }
        }

        private void LoadArticlesPage(FetchArticlesResult articles)
        {
            var nextPage = Articles.VirtualPage + 1;
            if (nextPage != articles.PageNumber)
            {
                // Already loaded, discarding the result
                return;
            }

            Articles.AddPage(articles.Articles, articles.TotalCount, articles.PageNumber, articles.PageSize);
            LoadMoreCommand.ChangeCanExecute();
            Raise(nameof(IsEmpty));
        }

        private async Task OnRefreshCommandExecutedAsync()
        {
            try
            {
                IsRefreshing = true;
                await InitNewsAsync(true);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task OnLoadMoreCommandExecuted()
        {
            try
            {
                IsLoadingMore = true;
                await LoadMoreAsync();
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        private void OnArticleSelectedCommandExecuted(ArticleViewModel selectedArticle)
        {
            if (string.IsNullOrWhiteSpace(selectedArticle?.UrlToArticle))
            {
                return;
            }

            if (Uri.TryCreate(selectedArticle.UrlToArticle, UriKind.Absolute, out Uri validUri))
            {
                Device.OpenUri(validUri);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Unable to parse urlToArticle provided: {selectedArticle.UrlToArticle}");
            }
        }

        protected abstract Task<FetchArticlesResult> FetchArticlesAsync(int pageNumber = 1, int pageSize = Constants.DefaultArticlesPageSize);
    }
}
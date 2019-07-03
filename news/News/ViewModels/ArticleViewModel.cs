using System;
using System.Text;
using Microsoft.MobCAT.MVVM;
using News.Models;
using News.Services.Abstractions;
using Xamarin.Forms;

namespace News.ViewModels
{
    /// <summary>
    /// Article view model.
    /// </summary>
    public class ArticleViewModel : BaseNavigationViewModel
    {
        protected IFavoritesService FavoritesService { get; } = DependencyService.Resolve<IFavoritesService>();

        public Article Article { get; }

        public string Title => Article.Title;

        public string UrlToImage => Article.UrlToImage;

        public string UrlToArticle => Article.Url;

        public string PublishedAgo
        {
            get
            {
                if (Article.PublishedAt == null)
                    return null;
                    
                var publishedHoursAgo = Math.Round((DateTime.UtcNow - Article.PublishedAt.Value).TotalHours);
                if (publishedHoursAgo <= 1)
                    return "now";

                if (publishedHoursAgo < 24)
                    return $"{publishedHoursAgo}h";

                return $"{publishedHoursAgo / 24:0}d";
            }
        }

        public string Footer
        {
            get
            {
                // Author includes source and author
                var footerBuilder = new StringBuilder(Article.Author);
                if (!string.IsNullOrWhiteSpace(Article.Source?.Name))
                {
                    if (footerBuilder.Length > 0)
                        footerBuilder.Append(" | ");

                    footerBuilder.Append(Article.Source.Name);
                }

                var when = PublishedAgo;
                if (!string.IsNullOrWhiteSpace(when))
                {
                    if (footerBuilder.Length > 0)
                        footerBuilder.Append(" | ");

                    footerBuilder.Append(when);
                }


                var result = footerBuilder.ToString();
                return result;
            }
        }

        public bool IsFavorite
        {
            get { return FavoritesService.IsFavorite(Article); }
            set
            {
                if (IsFavorite != value)
                {
                    if (value)
                    {
                        FavoritesService.Add(Article);
                    }
                    else
                    {
                        FavoritesService.Remove(Article);
                    }
                    Raise(nameof(IsFavorite));
                }
            }
        }

        public Microsoft.MobCAT.MVVM.Command SwitchFavoriteArticleCommand { get; }

        public ArticleViewModel(Article article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            Article = article;
            SwitchFavoriteArticleCommand = new Microsoft.MobCAT.MVVM.Command(OnSwitchFavoriteArticleCommandExecuted);
        }


        private void OnSwitchFavoriteArticleCommandExecuted()
        {
            IsFavorite = !IsFavorite;
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using News.Helpers;
using News.Models;
using News.Services;
using News.Services.Abstractions;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(FavoritesService))]
namespace News.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly ConcurrentDictionary<string, FavoriteEntry> _favorites = new ConcurrentDictionary<string, FavoriteEntry>();

        public FavoritesService()
        {
            LoadFavorites();
        }

        public bool IsFavorite(Article article)
        {
            var key = BuildArticleKey(article);
            return _favorites.ContainsKey(key);
        }

        public void Add(Article article)
        {
            var key = BuildArticleKey(article);
            var favEntry = new FavoriteEntry { Article = article, FavoritedAt = DateTime.UtcNow };
            if (_favorites.TryAdd(key, favEntry))
            {
                SaveFavorites();
            }
        }

        public void Remove(Article article)
        {
            var key = BuildArticleKey(article);
            if (_favorites.TryRemove(key, out FavoriteEntry removed))
            {
                SaveFavorites();
            }
        }

        public IEnumerable<Article> Get()
        {
            return _favorites.Values
                .OrderByDescending(f => f.FavoritedAt)
                .Select(f => f.Article)
                .ToList();
        }

        private string BuildArticleKey(Article article)
        {
            if (string.IsNullOrWhiteSpace(article?.Url))
                return null;

            return article.Url;
        }

        private void SaveFavorites()
        {
            var state = JsonConvert.SerializeObject(_favorites);
            Preferences.Set(GetType().FullName, state);
        }

        private void LoadFavorites()
        {
            var state = Preferences.Get(GetType().FullName, null);
            if (string.IsNullOrWhiteSpace(state))
                return;

            var favoritesState = JsonConvert.DeserializeObject<Dictionary<string, FavoriteEntry>>(state);
            if (!favoritesState.IsNullOrEmpty())
            {
                _favorites.Clear();
                foreach (var entry in favoritesState)
                {
                    _favorites.TryAdd(entry.Key, entry.Value);
                }
            }
        }
    }
}
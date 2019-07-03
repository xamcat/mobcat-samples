using System;
namespace News.Services.Abstractions
{
    public enum VisualTheme
    {
        Dark,
        Light
    }

    public interface IVisualThemeService
    {
        VisualTheme CurrentVisualTheme { get; }

        void SetVisualTheme(VisualTheme theme);

        void LoadSelectedVisualTheme();
    }
}

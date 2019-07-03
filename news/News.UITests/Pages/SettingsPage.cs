using System;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace News.UITests.Pages
{
    public class SettingsPage : BasePage
    {
        protected readonly Query LightModeSwitch;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked(nameof(SettingsPage)),
            iOS = x => x.Marked(nameof(SettingsPage))
        };

        public SettingsPage()
        {
            LightModeSwitch = x => x.Marked(nameof(LightModeSwitch));
        }

        public SettingsPage SetTheme(Themes theme)
        {
            app.WaitForElement(LightModeSwitch);
            var state = "";
            if (OnAndroid)
            {
                state = app.Query(x => x.Marked("LightModeSwitch").Invoke("isChecked"))[0].ToString();
                state = string.Equals(state, "true") ? "1" : "0";
            }
            if(OniOS)
                state = app.Query(x => x.Id(nameof(LightModeSwitch)).Invoke("isOn"))[0].ToString();


            switch (theme)
            {
                case Themes.dark:
                    if(state == "1")
                        app.Tap(LightModeSwitch);
                    break;

                case Themes.light:
                    if (state == "0")
                        app.Tap(LightModeSwitch);
                    break;
            }

            app.Screenshot("Toggled light mode");
            return this;
        }
    }
}

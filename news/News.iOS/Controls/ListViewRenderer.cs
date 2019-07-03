using System;
using News.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRender))]
namespace News.iOS.Controls
{
    public class ListViewRender : ListViewRenderer
    {
        private NewsPullToRefreshView _refreshControlView;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (this.Element != null)
            {
                SetupRefreshControl();
                this.Element.Refreshing += NewElement_Refreshing;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Element != null)
                this.Element.Refreshing -= NewElement_Refreshing;

            _refreshControlView = null;
            base.Dispose(disposing);
        }

        private void NewElement_Refreshing(object sender, EventArgs e)
        {
            _refreshControlView?.Play();
        }

        private void SetupRefreshControl()
        {
            var tableViewController = (UITableViewController)ViewController;
            var refreshControl = tableViewController.RefreshControl;
            refreshControl.BackgroundColor = UIColor.Clear; //Fixes bug where Animation view layout is independent of tableview
            _refreshControlView = new NewsPullToRefreshView(refreshControl);
        }
    }
}
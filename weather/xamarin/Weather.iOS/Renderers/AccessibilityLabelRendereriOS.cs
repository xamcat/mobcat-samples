using System;
using System.ComponentModel;
using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;
using Weather.Controls;
using Weather.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AccessibilityLabel), typeof(AccessibilityLabelRendereriOS))]
namespace Weather.iOS.Renderers
{
    public class AccessibilityLabelRendereriOS : LabelRenderer
    {
        private readonly NSString NSFontAttributeName = new NSString("NSFont");
        private const float BaseFontSize = 23.0f;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Label.FormattedText)) //For formatted text
            {
                var attributedTextCopy = Control.AttributedText.MutableCopy() as NSMutableAttributedString;
                attributedTextCopy.BeginEditing();
                attributedTextCopy.EnumerateAttribute(attributeName: NSFontAttributeName,
                inRange: new NSRange(0, attributedTextCopy.Length),
                options: 0,
                callback: (NSObject value, NSRange range, ref bool stop) =>
                {
                    if (value != null && value is UIFont oldFont)
                    {
                        var newFont = GetFontFromAccessibilitySize(oldFont);
                        attributedTextCopy.RemoveAttribute(NSFontAttributeName, range);
                        attributedTextCopy.AddAttribute(NSFontAttributeName, value: newFont, range: range);
                    }
                });
                attributedTextCopy.EndEditing();
                Control.AttributedText = attributedTextCopy;
            }
            else if (e.PropertyName == nameof(Label.Text)) //For normal text
            {
                var newFont = GetFontFromAccessibilitySize(Control.Font);
                Control.Font = newFont;
            }
        }

        private UIFont GetFontFromAccessibilitySize(UIFont font)
        {
            var descriptor = UIFontDescriptor.PreferredBody;
            var pointSize = descriptor.PointSize * ((nfloat)Element.FontSize / BaseFontSize);// accessibilityLabel.FontSizeMultiplier;
            return font.WithSize(pointSize);
        }
    }
}
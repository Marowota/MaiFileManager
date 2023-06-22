using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiFileManager.Classes
{
    //public class ProgressBarAlert
    //{
    //    private static Popup _popup;

    //    public static void Show(string message)
    //    {
    //        // Create a new instance of the Xamarin.Forms.ProgressBar
    //        var progressBar = new ProgressBar
    //        {
    //            Progress = 0.5, // Set an initial progress value if desired
    //            HorizontalOptions = LayoutOptions.FillAndExpand
    //        };

    //        // Create a new instance of the Xamarin.Forms.Label to display the message
    //        var messageLabel = new Label
    //        {
    //            Text = message,
    //            HorizontalOptions = LayoutOptions.Center,
    //            VerticalOptions = LayoutOptions.Center
    //        };

    //        // Create a new instance of the Xamarin.Forms.StackLayout to hold the progress bar and message label
    //        var stackLayout = new StackLayout
    //        {
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand,
    //            Padding = new Thickness(20),
    //            Spacing = 10,
    //            Children = { progressBar, messageLabel }
    //        };

    //        // Create a new instance of the Xamarin.Forms.AbsoluteLayout to position the stack layout at the center of the screen
    //        var absoluteLayout = new AbsoluteLayout
    //        {
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand,
    //        };

    //        // Add the stack layout to the absolute layout with constraints to center it
    //        AbsoluteLayout.SetLayoutBounds(stackLayout, new Rect(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
    //        AbsoluteLayout.SetLayoutFlags(stackLayout, AbsoluteLayoutFlags.PositionProportional);
    //        absoluteLayout.Children.Add(stackLayout);

    //        // Create a new instance of the Xamarin.Forms.ContentView to contain the absolute layout
    //        var contentView = new ContentView
    //        {
    //            Content = absoluteLayout,
    //            BackgroundColor = new Color(0, 0, 0, 0.5f), // Semi-transparent background color for overlay effect
    //            Padding = new Thickness(40),
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand
    //        };

    //        // Create a new instance of the Xamarin.Forms.Popup to display the progress bar alert
    //        _popup = new Popup
    //        {
    //            Content = contentView,
    //            CanBeDismissedByTappingOutsideOfPopup = false
    //        };

    //        // Show the progress bar alert
    //        _popup
    //    }

    //    public static void Hide()
    //    {
    //        // Dismiss the progress bar alert by closing the popup
    //        if (_popup != null)
    //        {
    //            Application.Current.MainPage.Navigation.PopModalAsync(_popup);
    //            _popup = null;
    //        }
    //    }
    //}

}

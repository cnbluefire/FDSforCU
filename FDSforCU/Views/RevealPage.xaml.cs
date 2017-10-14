using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FDSforCU.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RevealPage : Page
    {
        public RevealPage()
        {
            this.InitializeComponent();
            Lists = new ObservableCollection<Uri>();
            Lists.Add(new Uri("ms-appx:///Assets/Images/1.jpg"));
            Lists.Add(new Uri("ms-appx:///Assets/Images/2.jpg"));
            Lists.Add(new Uri("ms-appx:///Assets/Images/3.jpg"));
            Lists.Add(new Uri("ms-appx:///Assets/Images/4.jpg"));
            Lists.Add(new Uri("ms-appx:///Assets/Images/5.jpg"));
            Suggestions = new ObservableCollection<string>();
        }

        public ObservableCollection<Uri> Lists { get; private set; }
        public ObservableCollection<string> Suggestions { get; private set; }

        private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch tSwitch)
            {
                if (this.RequestedTheme == ElementTheme.Light) tSwitch.IsOn = true;
                else tSwitch.IsOn = false;
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch tSwitch)
            {
                if (tSwitch.IsOn) this.RequestedTheme = ElementTheme.Light;
                else this.RequestedTheme = ElementTheme.Dark;
            }
        }

        private void rootScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var pVisual = ElementCompositionPreview.GetElementVisual(BackgroundImage);
            var mVisual = ElementCompositionPreview.GetElementVisual(mainStackPanel);

            var sPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(sender as ScrollViewer);

            var compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
            var pAnimation = compositor.CreateExpressionAnimation("Matrix4x4.CreateFromTranslation(Vector3(sPropSet.Translation.X ,-0.1 * mVisual.Size.Y -0.6 * sPropSet.Translation.Y , 0.0f))");
            pAnimation.SetReferenceParameter("sPropSet", sPropSet);
            pAnimation.SetReferenceParameter("mVisual", mVisual);
            pVisual.StartAnimation("TransformMatrix", pAnimation);
        }

        private void autoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                Suggestions.Clear();
                foreach(var item in Lists)
                {
                    if (item.ToString().Contains(sender.Text))
                        Suggestions.Add(item.ToString());
                }
                sender.ItemsSource = Suggestions;
            }
        }

        private void autoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            foreach (var item in Lists)
            {
                if (item.ToString() == args.SelectedItem.ToString())
                    comboBox.SelectedItem = item;
            }
        }
    }
}

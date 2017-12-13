using FDSforCU.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            MDsource = new MainPageModel();
        }

        MainPageModel MDsource;


        private async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        private async void rootScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///README.md"));
            using (var stream = await file.OpenStreamForReadAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    MDsource.Title = await reader.ReadToEndAsync();
                }
            }

            var pVisual = ElementCompositionPreview.GetElementVisual(BackgroundImage);
            var mVisual = ElementCompositionPreview.GetElementVisual(mainStackPanel);

            var sPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(sender as ScrollViewer);

            var compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
            var pAnimation = compositor.CreateExpressionAnimation("Matrix4x4.CreateFromTranslation(Vector3(sPropSet.Translation.X ,-0.2 * mVisual.Size.Y -0.6 * sPropSet.Translation.Y , 0.0f))");
            pAnimation.SetReferenceParameter("sPropSet", sPropSet);
            pAnimation.SetReferenceParameter("mVisual", mVisual);
            pVisual.StartAnimation("TransformMatrix", pAnimation);
        }
    }
}

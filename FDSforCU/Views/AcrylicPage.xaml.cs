using System;
using System.Collections.Generic;
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
    public sealed partial class AcrylicPage : Page
    {
        public AcrylicPage()
        {
            this.InitializeComponent();
        }

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

        private void AcrylicTopGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var trans = AcrylicTopGrid.RenderTransform as TranslateTransform;
            var temp = trans.Y + e.Delta.Translation.Y;
            if (temp < 0 && temp > -AcrylicTopGrid.ActualHeight)
            {
                trans.Y = temp;
            }
        }

        private void AcrylicTopGrid_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            RectToButtom.SkipToFill();
        }

        private void AcrylicTopGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            RectToButtom.Begin();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ElementBrushGridClip.Rect = new Rect(new Point(0, 0), e.NewSize);
        }

        private void AcrylicTopGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var trans = AcrylicTopGrid.RenderTransform as TranslateTransform;
            if (trans.Y == 0)
                RectToJump.Begin();
        }

        private void rootScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
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

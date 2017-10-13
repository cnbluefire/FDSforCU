using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Brushes
{
    public class AcrylicBrush : XamlCompositionBrushBase
    {
        Compositor compositor;
        CompositionEffectBrush Brush;
        ScalarKeyFrameAnimation TintOpacityFillAnimation;
        ScalarKeyFrameAnimation HostOpacityZeroAnimation;
        ColorKeyFrameAnimation TintToFallBackAnimation;

        protected override void OnConnected()
        {
            if (DesignMode.DesignModeEnabled) return;
            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content as UIElement).Compositor;
            var tintOpacity = Convert.ToSingle(TintOpacity);
            if (tintOpacity < 0f) tintOpacity = 0f;
            if (tintOpacity > 1f) tintOpacity = 1f;
            var arithmeticEffect = new ArithmeticCompositeEffect()
            {
                Name = "arithmetic",
                MultiplyAmount = 0,
                Source1Amount = 1f - tintOpacity,
                Source2Amount = tintOpacity,
                Source1 = new ArithmeticCompositeEffect()
                {
                    MultiplyAmount = 0f,
                    Source1Amount = 0.95f,
                    Source2Amount = 0.05f,
                    Source1 = new GaussianBlurEffect()
                    {
                        Name = "blur",
                        BlurAmount = Convert.ToSingle(BlurAmount),
                        BorderMode = EffectBorderMode.Hard,
                        Optimization = EffectOptimization.Balanced,
                        Source = new CompositionEffectSourceParameter("source"),
                    },
                    Source2 = new BorderEffect()
                    {
                        Source = new CompositionEffectSourceParameter("image"),
                        ExtendX = CanvasEdgeBehavior.Wrap,
                        ExtendY = CanvasEdgeBehavior.Wrap,
                    }
                },
                Source2 = new ColorSourceEffect()
                {
                    Name = "tintcolor",
                    Color = TintColor
                }
            };

            Brush = compositor.CreateEffectFactory(arithmeticEffect, new[] { "arithmetic.Source1Amount", "arithmetic.Source2Amount", "tintcolor.Color" }).CreateBrush();

            var imagesurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///FluentDesignSystem/Sketch/SketchTexture.jpg"));

            var imagebrush = compositor.CreateSurfaceBrush(imagesurface);
            imagebrush.Stretch = CompositionStretch.None;
            Brush.SetSourceParameter("image", imagebrush);

            switch (BackgroundSource)
            {
                case AcrylicBackgroundSource.Backdrop:
                    Brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
                    break;
                case AcrylicBackgroundSource.Hostbackdrop:
                    Brush.SetSourceParameter("source", compositor.CreateHostBackdropBrush());
                    break;
            }

            CompositionBrush = Brush;

            var line = compositor.CreateLinearEasingFunction();

            TintOpacityFillAnimation = compositor.CreateScalarKeyFrameAnimation();
            TintOpacityFillAnimation.InsertExpressionKeyFrame(0f, "TintOpacity", line);
            TintOpacityFillAnimation.InsertKeyFrame(1f, 1f, line);
            TintOpacityFillAnimation.Duration = TimeSpan.FromSeconds(0.1d);
            TintOpacityFillAnimation.Target = "arithmetic.Source2Amount";

            HostOpacityZeroAnimation = compositor.CreateScalarKeyFrameAnimation();
            HostOpacityZeroAnimation.InsertExpressionKeyFrame(0f, "1f - TintOpacity", line);
            HostOpacityZeroAnimation.InsertKeyFrame(1f, 0f, line);
            HostOpacityZeroAnimation.Duration = TimeSpan.FromSeconds(0.1d);
            HostOpacityZeroAnimation.Target = "arithmetic.Source1Amount";

            TintToFallBackAnimation = compositor.CreateColorKeyFrameAnimation();
            TintToFallBackAnimation.InsertKeyFrame(0f, TintColor, line);
            TintToFallBackAnimation.InsertKeyFrame(1f, FallbackColor, line);
            TintToFallBackAnimation.Duration = TimeSpan.FromSeconds(0.1d);
            TintToFallBackAnimation.Target = "tintcolor.Color";

            Window.Current.Activated += Current_Activated;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;
        }

        protected override void OnDisconnected()
        {
            CompositionBrush.Dispose();
            CompositionBrush = null;
        }

        void SetCompositionFocus(bool IsGotFocus)
        {
            if (CompositionBrush == null) return;
            if (BackgroundSource == AcrylicBackgroundSource.Backdrop) return;
            var tintOpacity = Convert.ToSingle(TintOpacity);
            if (tintOpacity < 0f) tintOpacity = 0f;
            if (tintOpacity > 1f) tintOpacity = 1f;
            HostOpacityZeroAnimation.SetScalarParameter("TintOpacity", tintOpacity);
            TintOpacityFillAnimation.SetScalarParameter("TintOpacity", tintOpacity);
            if (IsGotFocus)
            {
                CompositionBrush = Brush;
                TintOpacityFillAnimation.Direction = AnimationDirection.Reverse;
                HostOpacityZeroAnimation.Direction = AnimationDirection.Reverse;
                TintToFallBackAnimation.Direction = AnimationDirection.Reverse;
                CompositionBrush.StartAnimation("arithmetic.Source2Amount", TintOpacityFillAnimation);
                CompositionBrush.StartAnimation("arithmetic.Source1Amount", HostOpacityZeroAnimation);
                CompositionBrush.StartAnimation("tintcolor.Color", TintToFallBackAnimation);
            }
            else if (CompositionBrush == Brush)
            {
                var scopedBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                TintOpacityFillAnimation.Direction = AnimationDirection.Normal;
                HostOpacityZeroAnimation.Direction = AnimationDirection.Normal;
                TintToFallBackAnimation.Direction = AnimationDirection.Normal;
                CompositionBrush.StartAnimation("arithmetic.Source2Amount", TintOpacityFillAnimation);
                CompositionBrush.StartAnimation("arithmetic.Source1Amount", HostOpacityZeroAnimation);
                CompositionBrush.StartAnimation("tintcolor.Color", TintToFallBackAnimation);
                scopedBatch.Completed += (s, a) => CompositionBrush = compositor.CreateColorBrush(FallbackColor);
                scopedBatch.End();
            }
            else CompositionBrush = compositor.CreateColorBrush(FallbackColor);
        }

        private void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (BackgroundSource == AcrylicBackgroundSource.Hostbackdrop)
                SetCompositionFocus(e.Visible);
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (BackgroundSource == AcrylicBackgroundSource.Hostbackdrop)
                SetCompositionFocus(e.WindowActivationState != CoreWindowActivationState.Deactivated);
        }

        public AcrylicBackgroundSource BackgroundSource
        {
            get { return (AcrylicBackgroundSource)GetValue(BackgroundSourceProperty); }
            set { SetValue(BackgroundSourceProperty, value); }
        }

        public double BlurAmount
        {
            get { return (double)GetValue(BlurAmountProperty); }
            set { SetValue(BlurAmountProperty, value); }
        }

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        public double TintOpacity
        {
            get { return (double)GetValue(TintOpacityProperty); }
            set { SetValue(TintOpacityProperty, value); }
        }

        public static readonly DependencyProperty BackgroundSourceProperty =
            DependencyProperty.Register("BackgroundSource", typeof(AcrylicBackgroundSource), typeof(AcrylicBrush), new PropertyMetadata(AcrylicBackgroundSource.Backdrop, OnBackgroundSourcePropertyChanged));
        public static readonly DependencyProperty BlurAmountProperty =
            DependencyProperty.Register("BlurAmount", typeof(double), typeof(AcrylicBrush), new PropertyMetadata(20d, OnBlurAmountPropertyChanged));
        public static readonly DependencyProperty TintColorProperty =
            DependencyProperty.Register("TintColor", typeof(Color), typeof(AcrylicBrush), new PropertyMetadata((Color)Application.Current.Resources["SystemChromeHighColor"], OnTintColorPropertyChanged));
        public static readonly DependencyProperty TintOpacityProperty =
            DependencyProperty.Register("TintOpacity", typeof(double), typeof(AcrylicBrush), new PropertyMetadata(0.7d, OnTintOpacityPropertyChanged));

        private static void OnBackgroundSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue != e.OldValue)
                {
                    var sender = d as AcrylicBrush;
                    if (DesignMode.DesignModeEnabled) return;
                    else
                    {
                        if (sender.CompositionBrush == null) return;
                        switch (e.NewValue)
                        {
                            case AcrylicBackgroundSource.Backdrop:
                                (sender.CompositionBrush as CompositionEffectBrush).SetSourceParameter("source", sender.compositor.CreateBackdropBrush());
                                break;
                            case AcrylicBackgroundSource.Hostbackdrop:
                                (sender.CompositionBrush as CompositionEffectBrush).SetSourceParameter("source", sender.compositor.CreateHostBackdropBrush());
                                break;
                        }
                    }
                }

            }
        }
        private static void OnBlurAmountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var sender = d as AcrylicBrush;
                if (DesignMode.DesignModeEnabled) return;
                else
                {
                    if (sender.CompositionBrush == null) return;
                    sender.CompositionBrush.Properties.InsertScalar("blur.BlurAmount", Convert.ToSingle(e.NewValue));
                }
            }
        }
        private static void OnTintColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var sender = d as AcrylicBrush;
                if (DesignMode.DesignModeEnabled) return;
                else
                {
                    if (sender.CompositionBrush == null) return;
                    sender.CompositionBrush.Properties.InsertColor("tintcolor.Color", (Color)e.NewValue);
                }
            }
        }
        private static void OnTintOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var sender = d as AcrylicBrush;
                if (DesignMode.DesignModeEnabled) return;
                else
                {
                    if (sender.CompositionBrush == null) return;
                    var tintOpacity = Convert.ToSingle(e.NewValue);
                    if (tintOpacity < 0f) tintOpacity = 0f;
                    if (tintOpacity > 1f) tintOpacity = 1f;
                    sender.CompositionBrush.Properties.InsertScalar("arithmetic.Source1Amount", 1f - tintOpacity);
                    sender.CompositionBrush.Properties.InsertScalar("arithmetic.Source2Amount", tintOpacity);
                }
            }
        }
    }
}

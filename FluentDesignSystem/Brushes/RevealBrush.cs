﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Brushes
{
    public class RevealBrush : XamlCompositionBrushBase
    {
        Compositor compositor;

        protected override void OnConnected()
        {
            if (DesignMode.DesignModeEnabled) return;
            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content as UIElement).Compositor;
            var borderEffect = new BorderEffect()
            {
                Source = new CompositionEffectSourceParameter("color"),
                ExtendX = CanvasEdgeBehavior.Clamp,
                ExtendY = CanvasEdgeBehavior.Clamp
            };

            var Brush = compositor.CreateEffectFactory(borderEffect).CreateBrush();
            Brush.SetSourceParameter("color", compositor.CreateColorBrush(Color));
            CompositionBrush = Brush;
        }

        protected override void OnDisconnected()
        {
            CompositionBrush.Dispose();
            CompositionBrush = null;
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public ApplicationTheme TargetTheme
        {
            get { return (ApplicationTheme)GetValue(TargetThemeProperty); }
            set { SetValue(TargetThemeProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RevealBrush), new PropertyMetadata(Colors.Transparent, OnColorPropertyChanged));
        public static readonly DependencyProperty TargetThemeProperty =
            DependencyProperty.Register("TargetTheme", typeof(ApplicationTheme), typeof(RevealBrush), new PropertyMetadata(Application.Current.RequestedTheme, OnTargetThemePropertyChanged));

        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (DesignMode.DesignModeEnabled) return;
            //if (e.NewValue != e.OldValue)
            //{
            //    var sender = d as RevealBrush;
            //    if (sender.CompositionBrush == null) return;
            //    sender.CompositionBrush.Properties.InsertColor("color.Color", (Color)e.NewValue);
            //}

            if (DesignMode.DesignModeEnabled) return;
            if (e.NewValue != e.OldValue)
            {
                var sender = d as RevealBrush;
                if (sender.CompositionBrush == null) return;
                (sender.CompositionBrush as CompositionEffectBrush).SetSourceParameter("color", sender.compositor.CreateColorBrush(sender.Color));
            }
        }

        private static void OnTargetThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            if (e.NewValue != e.OldValue)
            {
                var sender = d as RevealBrush;
                if (sender.CompositionBrush == null) return;
            }
        }
    }
}

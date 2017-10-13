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
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Brushes
{
    public class RevealBorderBrush : RevealBrush
    {
        Compositor compositor;

        protected override void OnConnected()
        {
            if (DesignMode.DesignModeEnabled) return;
            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content as UIElement).Compositor;
            var compositeEffect = new CompositeEffect() { Mode = CanvasComposite.Add };
            compositeEffect.Sources.Add(new BorderEffect()
            {
                Source = new CompositionEffectSourceParameter("backdrop"),
                ExtendX = CanvasEdgeBehavior.Clamp,
                ExtendY = CanvasEdgeBehavior.Clamp
            });
            compositeEffect.Sources.Add(new BorderEffect()
            {
                Source = new CompositionEffectSourceParameter("color"),
                ExtendX = CanvasEdgeBehavior.Clamp,
                ExtendY = CanvasEdgeBehavior.Clamp
            });

            //var blendEffect = new BlendEffect()
            //{
            //    Name = "blend",
            //    Mode = BlendEffectMode.Color,
            //    Background = new BorderEffect()
            //    {
            //        Source = new CompositionEffectSourceParameter("color"),
            //        ExtendX = CanvasEdgeBehavior.Clamp,
            //        ExtendY = CanvasEdgeBehavior.Clamp
            //    },
            //    Foreground = new BorderEffect()
            //    {
            //        Source = new CompositionEffectSourceParameter("backdrop"),
            //        ExtendX = CanvasEdgeBehavior.Clamp,
            //        ExtendY = CanvasEdgeBehavior.Clamp
            //    }
            //};

            var Brush = compositor.CreateEffectFactory(compositeEffect).CreateBrush();
            Brush.SetSourceParameter("backdrop", compositor.CreateBackdropBrush());
            Brush.SetSourceParameter("color", compositor.CreateColorBrush(Color));
            CompositionBrush = Brush;

            XamlLight.AddTargetBrush(Lights.RevealBorderSpotLight.GetIdStatic(), this);
        }
        protected override void OnDisconnected()
        {
            base.OnDisconnected();
        }
    }
}

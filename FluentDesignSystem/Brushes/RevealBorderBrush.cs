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
using Windows.UI.Composition.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Brushes
{
    public class RevealBorderBrush : RevealBrush
    {
        protected override void OnConnected()
        {
            if (DesignMode.DesignModeEnabled) return;
            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content as UIElement).Compositor;

            var arithmeticCompositeEffect = new ArithmeticCompositeEffect()
            {
                Source1 = new CompositionEffectSourceParameter("backdrop"),
                Source2 = new BorderEffect()
                {
                    Source = new CompositionEffectSourceParameter("color"),
                    ExtendX = CanvasEdgeBehavior.Clamp,
                    ExtendY = CanvasEdgeBehavior.Clamp
                },
                Source1Amount = 0.9f,
                Source2Amount = 0.1f
            };
            var Brush = compositor.CreateEffectFactory(arithmeticCompositeEffect).CreateBrush();
            Brush.SetSourceParameter("backdrop", compositor.CreateBackdropBrush());
            Brush.SetSourceParameter("color", compositor.CreateColorBrush(Color));

            CompositionBrush = Brush;

            XamlLight.AddTargetBrush(Lights.RevealAmbientLight.GetIdStatic(), this);
            XamlLight.AddTargetBrush(Lights.RevealBorderSpotLight.GetIdStatic(), this);
        }
        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            XamlLight.RemoveTargetBrush(Lights.RevealAmbientLight.GetIdStatic(), this);
            XamlLight.RemoveTargetBrush(Lights.RevealBorderSpotLight.GetIdStatic(), this);
        }
    }
}

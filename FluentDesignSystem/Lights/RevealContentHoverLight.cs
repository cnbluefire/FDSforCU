using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Lights
{
    public class RevealContentHoverLight : RevealLightBase
    {
        ScalarKeyFrameAnimation OnConnectionAnimation;
        CubicBezierEasingFunction cbEasing;

        protected override void OnConnected(UIElement newElement)
        {
            base.OnConnected(newElement);

            var spotlight = compositor.CreateSpotLight();
            spotlight.InnerConeAngleInDegrees = 2f;
            spotlight.OuterConeAngleInDegrees = 35f;
            spotlight.InnerConeColor = Colors.FloralWhite;
            spotlight.OuterConeColor = Colors.FloralWhite;
            spotlight.ConstantAttenuation = 2f;
            spotlight.LinearAttenuation = 0f;
            spotlight.QuadraticAttenuation = 0f;
            CompositionLight = spotlight;

            spotlight.StartAnimation("Offset", OffsetAnimation);

            cbEasing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.42f, 0f), new Vector2(1f, 1f));

            OnConnectionAnimation = compositor.CreateScalarKeyFrameAnimation();
            OnConnectionAnimation.InsertKeyFrame(0f, 200f, cbEasing);
            OnConnectionAnimation.InsertKeyFrame(1f, 400f, cbEasing);
            OnConnectionAnimation.Duration = TimeSpan.FromSeconds(0.05d);
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        public static string GetIdStatic()
        {
            return typeof(RevealContentHoverLight).FullName;
        }

        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }
        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(RevealContentHoverLight), new PropertyMetadata(false,IsConnectedChanged));
        private static void IsConnectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != e.OldValue)
            {
                var sender = (RevealContentHoverLight)d;
                var value = (bool)e.NewValue;

                if (value)
                {
                    sender.propSet.StartAnimation("height", sender.OnConnectionAnimation);
                }
                else
                {
                    sender.propSet.StopAnimation("height");
                }
            }

        }
    }
}

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
    public class RevealContentPressedLight : RevealLightBase
    {
        CubicBezierEasingFunction cbEasing;
        LinearEasingFunction line;
        ExpressionAnimation LinearAttenuationAnimation;
        ColorKeyFrameAnimation ColorAnimation;
        ScalarKeyFrameAnimation HeightAnimation;

        protected override void OnConnected(UIElement newElement)
        {
            Height = 50f;
            base.OnConnected(newElement);

            var spotlight = compositor.CreateSpotLight();
            spotlight.InnerConeAngleInDegrees = 2f;
            spotlight.OuterConeAngleInDegrees = 35f;
            spotlight.InnerConeColor = Colors.Black;
            spotlight.OuterConeColor = Colors.Black;
            spotlight.ConstantAttenuation = 3f;
            spotlight.LinearAttenuation = 0f;
            spotlight.QuadraticAttenuation = 2f;
            CompositionLight = spotlight;

            spotlight.StartAnimation("Offset", OffsetAnimation);

            cbEasing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.42f, 0f), new Vector2(1f, 1f));
            line = compositor.CreateLinearEasingFunction();

            ColorAnimation = compositor.CreateColorKeyFrameAnimation();
            ColorAnimation.InsertExpressionKeyFrame(0f, "this.StartingValue", cbEasing);
            ColorAnimation.InsertKeyFrame(1f, Colors.Black, cbEasing);
            ColorAnimation.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;

            HeightAnimation = compositor.CreateScalarKeyFrameAnimation();
            HeightAnimation.InsertExpressionKeyFrame(0f, "this.StartingValue", cbEasing);
            HeightAnimation.InsertKeyFrame(1f, 500f, cbEasing);
            HeightAnimation.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;

            LinearAttenuationAnimation = compositor.CreateExpressionAnimation("PropSet.height / 1600");
            LinearAttenuationAnimation.SetReferenceParameter("PropSet", propSet);
            CompositionLight.StartAnimation("LinearAttenuation", LinearAttenuationAnimation);
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        public static string GetIdStatic()
        {
            return typeof(RevealContentPressedLight).FullName;
        }

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(RevealContentPressedLight), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    var IsPressed = (bool)a.NewValue;
                    var sender = s as RevealContentPressedLight;

                    if (IsPressed)
                    {
                        sender.HeightAnimation.Duration = TimeSpan.FromSeconds(2d);
                        sender.ColorAnimation.Duration = TimeSpan.FromSeconds(2d);
                        ((SpotLight)sender.CompositionLight).InnerConeColor = Colors.FloralWhite;
                        ((SpotLight)sender.CompositionLight).OuterConeColor = Colors.FloralWhite;
                        sender.propSet.InsertScalar("height", 50f);
                        sender.propSet.StartAnimation("height", sender.HeightAnimation);
                        sender.CompositionLight.StartAnimation("InnerConeColor", sender.ColorAnimation);
                        sender.CompositionLight.StartAnimation("OuterConeColor", sender.ColorAnimation);
                    }
                    else
                    {
                        sender.propSet.StopAnimation("height");
                        sender.CompositionLight.StopAnimation("InnerConeColor");
                        sender.CompositionLight.StopAnimation("OuterConeColor");
                        sender.HeightAnimation.Duration = TimeSpan.FromSeconds(0.3d);
                        sender.ColorAnimation.Duration = TimeSpan.FromSeconds(0.3d);
                        sender.propSet.StartAnimation("height", sender.HeightAnimation);
                        sender.CompositionLight.StartAnimation("InnerConeColor", sender.ColorAnimation);
                        sender.CompositionLight.StartAnimation("OuterConeColor", sender.ColorAnimation);
                    }
                }
            }));
    }
}

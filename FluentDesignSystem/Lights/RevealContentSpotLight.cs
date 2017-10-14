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
    public class RevealContentSpotLight : XamlLight
    {
        public bool IsConnected
        {
            get => compositor != null;
        }

        UIElement element;
        Compositor compositor;
        CubicBezierEasingFunction cbEasing;
        LinearEasingFunction line;
        public CompositionPropertySet PropSet;
        ExpressionAnimation SpotLightOffsetAnimation;
        ExpressionAnimation LinearAttenuationAnimation;
        ColorKeyFrameAnimation ReleasedInnerConeColorAnimation;
        ScalarKeyFrameAnimation PressedPropSetOffsetZAnimation;
        ScalarKeyFrameAnimation ReleasedPropSetOffsetZAnimation;

        protected override void OnConnected(UIElement newElement)
        {
            element = newElement;

            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
            var spotlight = compositor.CreateSpotLight();
            spotlight.InnerConeAngleInDegrees = 10f;
            spotlight.OuterConeAngleInDegrees = 35f;
            spotlight.InnerConeColor = Colors.FloralWhite;
            spotlight.OuterConeColor = Colors.FloralWhite;
            spotlight.ConstantAttenuation = 2f;
            spotlight.LinearAttenuation = 0f;
            spotlight.QuadraticAttenuation = 0f;
            CompositionLight = spotlight;

            PropSet = compositor.CreatePropertySet();
            PropSet.InsertScalar("OffsetX", 0f);
            PropSet.InsertScalar("OffsetY", 0f);
            PropSet.InsertScalar("OffsetZ", 500f);

            SpotLightOffsetAnimation = compositor.CreateExpressionAnimation("Vector3(PropSet.OffsetX ,PropSet.OffsetY ,PropSet.OffsetZ)");
            SpotLightOffsetAnimation.SetReferenceParameter("PropSet", PropSet);
            CompositionLight.StartAnimation("Offset", SpotLightOffsetAnimation);

            //newElement.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(Content_PointerMoved), true);
            //newElement.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(Content_PointerExited), true);
            //newElement.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(Content_PointerPressed), true);
            //newElement.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(Content_PointerReleased), true);

            cbEasing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.42f, 0f), new Vector2(1f, 1f));
            line = compositor.CreateLinearEasingFunction();
            var an = compositor.CreateScalarKeyFrameAnimation();
            an.InsertKeyFrame(0f, 0f, cbEasing);
            an.InsertKeyFrame(1f, 60f, cbEasing);
            an.Duration = TimeSpan.FromSeconds(10);
            an.IterationBehavior = AnimationIterationBehavior.Forever;

            LinearAttenuationAnimation = compositor.CreateExpressionAnimation("PropSet.OffsetZ / 1600");
            LinearAttenuationAnimation.SetReferenceParameter("PropSet", PropSet);
            CompositionLight.StartAnimation("LinearAttenuation", LinearAttenuationAnimation);

            PressedPropSetOffsetZAnimation = compositor.CreateScalarKeyFrameAnimation();
            PressedPropSetOffsetZAnimation.InsertKeyFrame(0f, 200f, line);
            PressedPropSetOffsetZAnimation.InsertKeyFrame(0.005f, 45f, cbEasing);
            PressedPropSetOffsetZAnimation.InsertKeyFrame(1f, 800f, cbEasing);
            PressedPropSetOffsetZAnimation.Duration = TimeSpan.FromSeconds(10);
            PressedPropSetOffsetZAnimation.Target = "OffsetZ";

            ReleasedInnerConeColorAnimation = compositor.CreateColorKeyFrameAnimation();
            ReleasedInnerConeColorAnimation.InsertExpressionKeyFrame(0.9f, "this.StartingValue", line);
            ReleasedInnerConeColorAnimation.InsertKeyFrame(1f, Colors.FloralWhite, line);
            ReleasedInnerConeColorAnimation.Duration = TimeSpan.FromSeconds(1.5d);
            ReleasedInnerConeColorAnimation.Target = "InnerConeColor";

            ReleasedPropSetOffsetZAnimation = compositor.CreateScalarKeyFrameAnimation();
            ReleasedPropSetOffsetZAnimation.InsertExpressionKeyFrame(0f, "this.StartingValue", cbEasing);
            ReleasedPropSetOffsetZAnimation.InsertKeyFrame(1f, 500f, cbEasing);
            ReleasedPropSetOffsetZAnimation.Duration = TimeSpan.FromSeconds(0.15);
            ReleasedPropSetOffsetZAnimation.Target = "OffsetZ";
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            //oldElement.RemoveHandler(UIElement.PointerMovedEvent, new PointerEventHandler(Content_PointerMoved));
            //oldElement.RemoveHandler(UIElement.PointerExitedEvent, new PointerEventHandler(Content_PointerExited));
            //oldElement.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(Content_PointerPressed));
            //oldElement.RemoveHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(Content_PointerReleased));
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        public static string GetIdStatic()
        {
            return typeof(RevealContentSpotLight).FullName;
        }

        public void SetPosition(Vector2 position)
        {
            PropSet.InsertScalar("OffsetX", position.X);
            PropSet.InsertScalar("OffsetY", position.Y);
        }

        void ContentPressed()
        {
            (CompositionLight as SpotLight).StopAnimation("InnerConeColor");
            PropSet.StopAnimation("OffsetZ");
            (CompositionLight as SpotLight).InnerConeColor = Colors.Black;
            PropSet.StartAnimation("OffsetZ", PressedPropSetOffsetZAnimation);
        }

        void ContentReleased()
        {
            (CompositionLight as SpotLight).StopAnimation("InnerConeColor");
            PropSet.StopAnimation("OffsetZ");
            (CompositionLight as SpotLight).StartAnimation("InnerConeColor", ReleasedInnerConeColorAnimation);
            PropSet.StartAnimation("OffsetZ", ReleasedPropSetOffsetZAnimation);
        }



        public bool IsPressedEnable
        {
            get { return (bool)GetValue(IsPressedEnableProperty); }
            set { SetValue(IsPressedEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedEnableProperty =
            DependencyProperty.Register("IsPressedEnable", typeof(bool), typeof(RevealContentSpotLight), new PropertyMetadata(false, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    var isPressEnable = (bool)a.NewValue;
                    var sender = s as RevealContentSpotLight;
                    if (isPressEnable) sender.ContentPressed();
                    else sender.ContentReleased();
                }
            }));




        //private void Content_PointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    var position = e.GetCurrentPoint(sender as UIElement).Position.ToVector2();
        //    PropSet.InsertScalar("OffsetX", position.X);
        //    PropSet.InsertScalar("OffsetY", position.Y);
        //}

        //private void Content_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    if (Window.Current.Content.Lights.Count != 0)
        //    {
        //        Window.Current.Content.Lights.Clear();
        //    }
        //}


        //private void Content_PointerReleased(object sender, PointerRoutedEventArgs e)
        //{
        //    ContentReleased();
        //}

        //private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    ContentPressed();
        //}
    }
}

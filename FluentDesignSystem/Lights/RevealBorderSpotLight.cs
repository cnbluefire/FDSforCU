using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Lights
{
    public class RevealBorderSpotLight : XamlLight
    {
        UIElement element;
        Compositor compositor;
        CubicBezierEasingFunction cbEasing;
        LinearEasingFunction line;
        CompositionPropertySet PropSet;
        ExpressionAnimation SpotLightOffsetAnimation;


        protected override void OnConnected(UIElement newElement)
        {
            element = newElement;

            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
            var spotlight = compositor.CreateSpotLight();
            spotlight.InnerConeAngleInDegrees = 10f;
            spotlight.OuterConeAngleInDegrees = 20f;
            spotlight.InnerConeColor = Colors.FloralWhite;
            spotlight.OuterConeColor = Colors.FloralWhite;
            spotlight.ConstantAttenuation = 0f;
            spotlight.LinearAttenuation = 0f;
            spotlight.QuadraticAttenuation = 0f;
            CompositionLight = spotlight;

            PropSet = compositor.CreatePropertySet();
            PropSet.InsertScalar("OffsetX", 0f);
            PropSet.InsertScalar("OffsetY", 0f);
            PropSet.InsertScalar("OffsetZ", 300f);

            SpotLightOffsetAnimation = compositor.CreateExpressionAnimation("Vector3(PropSet.OffsetX ,PropSet.OffsetY ,PropSet.OffsetZ)");
            SpotLightOffsetAnimation.SetReferenceParameter("PropSet", PropSet);
            CompositionLight.StartAnimation("Offset", SpotLightOffsetAnimation);
            cbEasing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.42f, 0f), new Vector2(1f, 1f));
            line = compositor.CreateLinearEasingFunction();
            var an = compositor.CreateScalarKeyFrameAnimation();
            an.InsertKeyFrame(0f, 0f, cbEasing);
            an.InsertKeyFrame(1f, 60f, cbEasing);
            an.Duration = TimeSpan.FromSeconds(10);
            an.IterationBehavior = AnimationIterationBehavior.Forever;
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            element = null;
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        public static string GetIdStatic()
        {
            return typeof(RevealBorderSpotLight).FullName;
        }

        public void SetPosition(Vector2 position)
        {
            PropSet.InsertScalar("OffsetX", position.X);
            PropSet.InsertScalar("OffsetY", position.Y);
        }


        //private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    var windowposition = e.GetCurrentPoint(sender as UIElement).Position;
        //    var position = (sender as UIElement).TransformToVisual(element).TransformPoint(windowposition).ToVector2();
        //    PropSet.InsertScalar("OffsetX", position.X);
        //    PropSet.InsertScalar("OffsetY", position.Y);
        //}

        //private void Border_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    //if (element is Panel)
        //    //{
        //    //    if ((element as Grid).BorderBrush == null) return;
        //    //    RemoveTargetBrush(GetId(), (element as Grid).BorderBrush);
        //    //}
        //    //if (element is Border)
        //    //{
        //    //    if ((element as Border).BorderBrush == null) return;
        //    //    RemoveTargetBrush(GetId(), (element as Border).BorderBrush);
        //    //}
        //    //if (element is Button)
        //    //{
        //    //    if ((element as Button).BorderBrush == null) return;
        //    //    RemoveTargetBrush(GetId(), (element as Button).BorderBrush);
        //    //}
        //}

        //private void Border_PointerMoved(CoreWindow sender, PointerEventArgs e)
        //{
        //    var position = e.CurrentPoint.Position.ToVector2();
        //    //var position = Window.Current.Content.TransformToVisual(element).TransformPoint(windowposition).ToVector2();
        //    PropSet.InsertScalar("OffsetX", position.X);
        //    PropSet.InsertScalar("OffsetY", position.Y);
        //}

        //private void Border_PointerExited(CoreWindow sender, PointerEventArgs e)
        //{

        //}
    }

}

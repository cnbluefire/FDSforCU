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
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static FluentDesignSystem.StaticValue;

namespace FluentDesignSystem.Lights
{
    public class RevealBorderSpotLight : RevealLightBase
    {
        protected override void OnConnected(UIElement newElement)
        {
            Height = 200f;
            base.OnConnected(newElement);

            var spotlight = compositor.CreateSpotLight();
            spotlight.InnerConeAngleInDegrees = 7f;
            spotlight.OuterConeAngleInDegrees = 20f;
            spotlight.InnerConeColor = Colors.FloralWhite;
            spotlight.OuterConeColor = Colors.FloralWhite;
            spotlight.ConstantAttenuation = 0f;
            spotlight.LinearAttenuation = 0f;
            spotlight.QuadraticAttenuation = 0f;
            CompositionLight = spotlight;


            spotlight.StartAnimation("Offset", OffsetAnimation);
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            base.OnDisconnected(oldElement);
        }

        public static string GetIdStatic()
        {
            return typeof(RevealBorderSpotLight).FullName;
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }
    }

}

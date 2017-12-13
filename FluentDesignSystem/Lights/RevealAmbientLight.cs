using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Lights
{
    public class RevealAmbientLight : XamlLight
    {
        Compositor compositor;

        protected override void OnConnected(UIElement newElement)
        {
            compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
            var ambientlight = compositor.CreateAmbientLight();
            ambientlight.Color = Colors.White;
            CompositionLight = ambientlight;
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            base.OnDisconnected(oldElement);
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        public static string GetIdStatic()
        {
            return typeof(RevealAmbientLight).FullName;
        }
    }
}

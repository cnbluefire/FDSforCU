using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Lights
{
    public class RevealLightBase : XamlLight
    {
        protected bool IsConnected = false;
        protected UIElement element;
        protected Compositor compositor;
        protected CompositionPropertySet pointPropSet;
        protected CompositionPropertySet propSet;
        protected ExpressionAnimation OffsetAnimation;
        protected float Height;
        protected bool oldIsPoint;

        protected override void OnConnected(UIElement newElement)
        {
            base.OnConnected(newElement);
            element = newElement;
            compositor = Window.Current.Compositor;
            pointPropSet = ElementCompositionPreview.GetPointerPositionPropertySet(element);
            propSet = compositor.CreatePropertySet();

            propSet.InsertScalar("height", Height);
            propSet.InsertVector2("Position", new Vector2(0f, 0f));
            OffsetAnimation = compositor.CreateExpressionAnimation("Vector3(PointPropSet.Position.X,PointPropSet.Position.Y,PropSet.height)");
            OffsetAnimation.SetReferenceParameter("PointPropSet", pointPropSet);
            OffsetAnimation.SetReferenceParameter("PropSet", propSet);
            IsConnected = true;
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            IsConnected = false;
            base.OnDisconnected(oldElement);
            CompositionLight.StopAnimation("Offset");
            element = null;
            OffsetAnimation.Dispose();
            OffsetAnimation = null;
            propSet.Dispose();
            propSet = null;
            pointPropSet.Dispose();
            pointPropSet = null;
        }

        public void SetPosition(Vector2 position,bool IsPoint)
        {
            if (!IsConnected) return;
            if (element == null) return;
            if (CompositionLight is SpotLight spotLight)
            {
                if (IsPoint)
                {
                    if (!oldIsPoint)
                    {
                        spotLight.StopAnimation("Offset");
                        OffsetAnimation.SetReferenceParameter("PointPropSet", pointPropSet);
                        spotLight.StartAnimation("Offset", OffsetAnimation);
                    }
                }
                else
                {
                    if (oldIsPoint)
                    {
                        spotLight.StopAnimation("Offset");
                        OffsetAnimation.SetReferenceParameter("PointPropSet", propSet);
                        spotLight.StartAnimation("Offset", OffsetAnimation);
                    }
                    propSet.InsertVector2("Position", position);
                }
                oldIsPoint = IsPoint;
            }
        }
    }
}

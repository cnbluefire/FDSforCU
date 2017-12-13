using FluentDesignSystem.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml;

namespace FluentDesignSystem.Lights
{
    public class LightCollection
    {
        public RevealAmbientLight AmbientLight;
        public RevealBorderSpotLight BorderLight;
        public RevealContentHoverLight HoverLight;
        public RevealContentPressedLight PressedLight;

        public bool PointerEnteredHandled = false;
        public bool PointerExitedHandled = false;
        public bool PointerMovedHandled = false;
        public bool UnloadedHandled = false;

        public LightCollection()
        {
            AmbientLight = new RevealAmbientLight();
            BorderLight = new RevealBorderSpotLight();
            HoverLight = new RevealContentHoverLight();
            PressedLight = new RevealContentPressedLight();
        }

        public static void AddLightCollection(UIElement element)
        {
            if (StaticValue.LightCollectionDictionary.ContainsKey(element)) return;

            var lightCollection = new LightCollection();
            StaticValue.LightCollectionDictionary.Add(element, lightCollection);
        }

        public static void UpdateLightCollection(UIElement element, PointerPoint point)
        {
            var IsPoint = true;
            if (point.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch) IsPoint = false;
            UpdateLightCollection(element, IsPoint);
        }
        public static void UpdateLightCollection(UIElement element, bool? IsPoint = null, bool? IsPressed = null, bool? IsConnected = null)
        {
            AddLightCollection(element);

            var lightCollection = StaticValue.LightCollectionDictionary[element];

            if (!element.Lights.Contains(lightCollection.AmbientLight)) element.Lights.Add(lightCollection.AmbientLight);
            if (!element.Lights.Contains(lightCollection.PressedLight)) element.Lights.Add(lightCollection.PressedLight);
            if (IsPoint.HasValue)
            {
                if (IsPoint.Value)
                {
                    if (!element.Lights.Contains(lightCollection.BorderLight)) element.Lights.Add(lightCollection.BorderLight);
                    if (!element.Lights.Contains(lightCollection.HoverLight)) element.Lights.Add(lightCollection.HoverLight);
                }
                else
                {
                    if (element.Lights.Contains(lightCollection.BorderLight)) element.Lights.Remove(lightCollection.BorderLight);
                    if (element.Lights.Contains(lightCollection.HoverLight)) element.Lights.Remove(lightCollection.HoverLight);
                }
            }
            if (IsPressed.HasValue)
            {
                lightCollection.PressedLight.IsPressed = IsPressed.Value;
            }
            if (IsConnected.HasValue)
            {
                lightCollection.HoverLight.IsConnected = IsConnected.Value;
            }
        }

        public static LightCollection GetLightCollection(UIElement element)
        {
            if (StaticValue.LightCollectionDictionary.ContainsKey(element))
                return StaticValue.LightCollectionDictionary[element];
            return null;
        }

        public static void DisableLightCollection(UIElement element)
        {
            AddLightCollection(element);
            element.Lights.Clear();
        }

        public static void DisableLightCollection()
        {
            foreach (var item in StaticValue.LightCollectionDictionary)
            {
                DisableLightCollection(item.Key);
            }
        }

        public static void RemoveLightCollection(UIElement element)
        {
            StaticValue.LightCollectionDictionary[element] = null;
            StaticValue.LightCollectionDictionary.Remove(element);
        }

        public static void SetPosition(UIElement element, PointerPoint point)
        {
            var position = point.Position.ToVector2();
            var IsPoint = true;
            if (point.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch) IsPoint = false;

            UpdateLightCollection(element, IsPoint);

            GetLightCollection(element).PressedLight.SetPosition(position, IsPoint);
        }
    }
}

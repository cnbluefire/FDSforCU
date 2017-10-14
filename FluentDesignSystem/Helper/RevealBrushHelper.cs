using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static FluentDesignSystem.StaticValue;

namespace FluentDesignSystem.Helper
{
    public static class RevealBrushHelper
    {
        public static RevealBrushHelperState GetState(UIElement element)
        {
            return (RevealBrushHelperState)element.GetValue(StateProperty);
        }

        public static void SetState(UIElement element, RevealBrushHelperState value)
        {
            element.SetValue(StateProperty, value);
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(RevealBrushHelperState), typeof(RevealBrushHelper), new PropertyMetadata(RevealBrushHelperState.Normal, OnStatePropertyChanged));

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                var element = (d as UIElement);
                if (element == null) return;
                InitLights();
                switch ((RevealBrushHelperState)e.NewValue)
                {
                    case RevealBrushHelperState.Normal:
                        {
                            if (element is Grid grid && InPopupGridNameList.Contains(grid.Name))
                            {
                                element.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(element_PointerExited), true);
                                element.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(element_PointerMoved), true);
                                SetPopupLight();
                            }
                            else
                            {
                                RevealBrushHelper.SetTargetLight(element, SetLightMode.Border);
                            }
                            SetElementContentLightPressed(element, false);
                        }
                        return;
                    case RevealBrushHelperState.PointerOver:
                        {
                            if (element is Grid grid && InPopupGridNameList.Contains(grid.Name))
                            {
                                SetPopupLight();
                            }
                            RevealBrushHelper.SetTargetLight(element, SetLightMode.All);
                            SetElementContentLightPressed(element, false);
                        }
                        break;
                    case RevealBrushHelperState.Pressed:
                        {
                            if (element is Grid grid && InPopupGridNameList.Contains(grid.Name))
                            {
                                SetPopupLight();
                            }
                            RevealBrushHelper.SetTargetLight(element, SetLightMode.All);
                            SetElementContentLightPressed(element, true);
                        }
                        break;
                }
            }
        }

        private static void PointerExited(CoreWindow sender, PointerEventArgs e)
        {
            RemoveLights();
        }

        private static void PointerEntered(CoreWindow sender, PointerEventArgs e)
        {
            InitLights();
        }

        private static void PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            var position = args.CurrentPoint.Position.ToVector2();
            if (BorderLight != null)
                BorderLight.SetPosition(position);
            if (ContentLight != null)
                ContentLight.SetPosition(position);
        }

        private static void InitLights()
        {
            if (AmbientLight == null) AmbientLight = new Lights.RevealAmbientLight();
            if (BorderLight == null) BorderLight = new Lights.RevealBorderSpotLight();
            if (ContentLight == null) ContentLight = new Lights.RevealContentSpotLight();

            if (Window.Current.Content.Lights.Count == 0)
            {
                Window.Current.Content.Lights.Add(AmbientLight);
                Window.Current.Content.Lights.Add(BorderLight);
                Window.Current.Content.Lights.Add(ContentLight);
            }

            if (!IsWindowPointerEnteredHandled)
            {
                CoreWindow.GetForCurrentThread().PointerEntered += PointerEntered;
                IsWindowPointerEnteredHandled = true;
            }
            if (!IsWindowPointerExitedHandled)
            {
                CoreWindow.GetForCurrentThread().PointerExited += PointerExited;
                IsWindowPointerExitedHandled = true;

            }
            if (!IsWindowPointerMovedHandled)
            {
                CoreWindow.GetForCurrentThread().PointerMoved += PointerMoved;
                IsWindowPointerMovedHandled = true;
            }
        }

        private static void RemoveLights()
        {
            Window.Current.Content.Lights.Clear();
        }

        private static void SetElementContentLightPressed(UIElement element, bool IsPressed)
        {
            ContentLight.IsPressedEnable = IsPressed;
            foreach (var popup in VisualTreeHelper.GetOpenPopups(Window.Current))
            {
                var contentLight = GetContentLight(popup);
                if (contentLight != null) contentLight.IsPressedEnable = IsPressed;
            }
        }

        private static void SetPopupLight()
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopups(Window.Current))
            {
                if (popup.Lights.Count == 0)
                {
                    popup.Lights.Add(ChildAmbientLight);
                    popup.Lights.Add(ChildContentLight);
                }
            }
        }

        private static void RemovePopupLight()
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopups(Window.Current))
            {
                popup.Lights.Clear();
            }
        }

        private static Lights.RevealContentSpotLight GetContentLight(UIElement element)
        {
            foreach (var light in element.Lights)
            {
                if (light is Lights.RevealContentSpotLight ContentLight && ContentLight.IsConnected)
                    return ContentLight;
            }
            return null;
        }

        public static void SetTargetLight(UIElement Target, SetLightMode Mode)
        {
            switch (Target)
            {
                case Grid element:
                    switch (Mode)
                    {
                        case SetLightMode.None:
                            XamlLight.RemoveTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.Border:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.All:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            XamlLight.AddTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                    }
                    break;
            }

        }

        private static void element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var element = sender as UIElement;
            RevealBrushHelper.SetTargetLight(element, SetLightMode.Border);
            RemovePopupLight();
            element.RemoveHandler(UIElement.PointerMovedEvent, new PointerEventHandler(element_PointerMoved));
            element.RemoveHandler(UIElement.PointerExitedEvent, new PointerEventHandler(element_PointerExited));
        }

        private static void element_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopups(Window.Current))
            {
                var position = (sender as UIElement).TransformToVisual(popup).TransformPoint(e.GetCurrentPoint(sender as UIElement).Position).ToVector2();
                var contentLight = GetContentLight(popup);
                if (contentLight != null) contentLight.SetPosition(position);
            }
        }

        public enum SetLightMode
        {
            None = -1,
            Border = 0,
            All = 1
        }
    }
}

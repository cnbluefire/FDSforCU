using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Input;
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
                var baseElement = element.FindBaseElement();
                if (baseElement == null) return;
                element.AddBaseElement(baseElement);
                AddBaseHandle(baseElement);
                Lights.LightCollection.UpdateLightCollection(baseElement, true);
                var mode = (RevealBrushHelperState)e.NewValue;
                SetTargetLight(element, mode);

                if (!CoreWindowHandled)
                {
                    CoreWindow.GetForCurrentThread().Activated += CoreWindow_Activated;
                    Window.Current.VisibilityChanged += Current_VisibilityChanged;
                }
            }
        }

        private static void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                Lights.LightCollection.DisableLightCollection();
            }
        }

        private static void CoreWindow_Activated(CoreWindow sender, WindowActivatedEventArgs args)
        {
            if(args.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                Lights.LightCollection.DisableLightCollection();
            }
        }

        private static void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is UIElement element)
            {
                Lights.LightCollection.DisableLightCollection(element);
            }
        }

        private static void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is UIElement element)
            {
                Lights.LightCollection.DisableLightCollection(element);
            }
        }

        private static void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is UIElement element)
            {
                Lights.LightCollection.SetPosition(element, e.GetCurrentPoint(element));
            }
        }

        private static void Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element)
            {
                RemoveBaseHandle(element);
                Lights.LightCollection.RemoveLightCollection(element);
            }
        }

        private static void AddBaseHandle(UIElement element)
        {
            var lightCollection = Lights.LightCollection.GetLightCollection(element);
            if (lightCollection != null)
            {
                if (!lightCollection.PointerEnteredHandled)
                {
                    element.AddHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(PointerEntered), true);
                    lightCollection.PointerEnteredHandled = true;
                }
                if (!lightCollection.PointerExitedHandled)
                {
                    element.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(PointerExited), true);
                    lightCollection.PointerExitedHandled = true;
                }
                if (!lightCollection.PointerMovedHandled)
                {
                    element.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(PointerMoved), true);
                    lightCollection.PointerMovedHandled = true;
                }
                if (!lightCollection.UnloadedHandled)
                {
                    ((FrameworkElement)element).Unloaded += Unloaded;
                    lightCollection.UnloadedHandled = true;
                }
            }
        }

        private static void RemoveBaseHandle(UIElement element)
        {
            var lightCollection = Lights.LightCollection.GetLightCollection(element);
            if (lightCollection != null)
            {
                if (lightCollection.PointerEnteredHandled)
                {
                    element.RemoveHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(PointerEntered));
                    lightCollection.PointerEnteredHandled = false;
                }
                if (lightCollection.PointerExitedHandled)
                {
                    element.RemoveHandler(UIElement.PointerExitedEvent, new PointerEventHandler(PointerExited));
                    lightCollection.PointerExitedHandled = false;
                }
                if (lightCollection.PointerMovedHandled)
                {
                    element.RemoveHandler(UIElement.PointerMovedEvent, new PointerEventHandler(PointerMoved));
                    lightCollection.PointerMovedHandled = false;
                }
                if (lightCollection.UnloadedHandled)
                {
                    ((FrameworkElement)element).Unloaded -= Unloaded;
                    lightCollection.UnloadedHandled = false;
                }
            }
        }

        public static void SetTargetLight(UIElement Target, RevealBrushHelperState Mode)
        {
            var baseElement = Target.GetBaseElement();
            switch (Mode)
            {
                case RevealBrushHelperState.Normal:
                    XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), Target);
                    XamlLight.RemoveTargetElement(Lights.RevealContentHoverLight.GetIdStatic(), Target);
                    XamlLight.RemoveTargetElement(Lights.RevealContentPressedLight.GetIdStatic(), Target);
                    Lights.LightCollection.UpdateLightCollection(baseElement, null, false, false);
                    break;
                case RevealBrushHelperState.PointerOver:
                    XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), Target);
                    XamlLight.AddTargetElement(Lights.RevealContentHoverLight.GetIdStatic(), Target);
                    XamlLight.AddTargetElement(Lights.RevealContentPressedLight.GetIdStatic(), Target);
                    Lights.LightCollection.UpdateLightCollection(baseElement, null, false, true);
                    break;
                case RevealBrushHelperState.Pressed:
                    XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), Target);
                    XamlLight.AddTargetElement(Lights.RevealContentHoverLight.GetIdStatic(), Target);
                    XamlLight.AddTargetElement(Lights.RevealContentPressedLight.GetIdStatic(), Target);
                    Lights.LightCollection.UpdateLightCollection(baseElement, null, true);
                    break;
            }
        }

        public static UIElement FindBaseElement(this UIElement element)
        {
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups)
            {
                if (popup.IsOpen)
                {
                    if (StaticValue.PopupNameList.Contains(popup.Name))
                    {
                        if (popup.Child != null)
                            return popup.Child;
                    }
                }
            }
            return Window.Current.Content;
        }

        public enum SetLightMode
        {
            None = -1,
            Border = 0,
            All = 1
        }
    }
}

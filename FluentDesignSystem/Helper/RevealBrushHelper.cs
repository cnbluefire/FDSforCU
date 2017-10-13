using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace FluentDesignSystem.Helper
{
    public static class RevealBrushHelper
    {
        private static bool IsHandleAdded;

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
                if (IsHandleAdded)
                {
                    Window.Current.Content.RemoveHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(OnPointerEntered));
                    Window.Current.Content.RemoveHandler(UIElement.PointerExitedEvent, new PointerEventHandler(OnPointerExited));
                }
                Window.Current.Content.AddHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(OnPointerEntered), true);
                Window.Current.Content.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(OnPointerExited), true);
                IsHandleAdded = true;

                switch ((RevealBrushHelperState)e.NewValue)
                {
                    case RevealBrushHelperState.Normal:
                        //if (Window.Current.Content.Lights.Count != 0)
                        //{
                        //    Window.Current.Content.Lights.Clear();
                        //}
                        if (Window.Current.Content.Lights.Count == 0)
                        {
                            Window.Current.Content.Lights.Add(new Lights.RevealAmbientLight());
                            Window.Current.Content.Lights.Add(new Lights.RevealContentSpotLight());
                            Window.Current.Content.Lights.Add(new Lights.RevealBorderSpotLight());
                        }
                        foreach (var light in Window.Current.Content.Lights)
                        {
                            if (light is Lights.RevealContentSpotLight)
                            {
                                (light as Lights.RevealContentSpotLight).IsPressedEnable = false;
                            }
                        }
                        if ((RevealBrushHelperState)e.OldValue == RevealBrushHelperState.Pressed ||
                            (RevealBrushHelperState)e.OldValue == RevealBrushHelperState.PointerOver)
                        {
                            if (element is Grid grid && grid.Name == "ContentBorder")
                                element.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(element_PointerExited), true);
                            else RevealBrushHelper.SetTargetLight(element, SetLightMode.Border);
                        }
                        else
                            RevealBrushHelper.SetTargetLight(element, SetLightMode.Border);
                        return;
                    case RevealBrushHelperState.PointerOver:
                        if (Window.Current.Content.Lights.Count == 0)
                        {
                            Window.Current.Content.Lights.Add(new Lights.RevealAmbientLight());
                            Window.Current.Content.Lights.Add(new Lights.RevealContentSpotLight());
                            Window.Current.Content.Lights.Add(new Lights.RevealBorderSpotLight());
                        }
                        foreach (var light in Window.Current.Content.Lights)
                        {
                            if (light is Lights.RevealContentSpotLight)
                            {
                                (light as Lights.RevealContentSpotLight).IsPressedEnable = false;
                            }
                        }
                        RevealBrushHelper.SetTargetLight(element, SetLightMode.All);
                        break;
                    case RevealBrushHelperState.Pressed:
                        if (Window.Current.Content.Lights.Count == 0)
                        {
                            Window.Current.Content.Lights.Add(new Lights.RevealAmbientLight());
                            Window.Current.Content.Lights.Add(new Lights.RevealContentSpotLight());
                            Window.Current.Content.Lights.Add(new Lights.RevealBorderSpotLight());
                        }
                        foreach (var light in Window.Current.Content.Lights)
                        {
                            if (light is Lights.RevealContentSpotLight)
                            {
                                (light as Lights.RevealContentSpotLight).IsPressedEnable = true;
                            }
                        }
                        RevealBrushHelper.SetTargetLight(element, SetLightMode.All);
                        break;
                }
            }
        }

        private static void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (Window.Current.Content.Lights.Count != 0)
            {
                Window.Current.Content.Lights.Clear();
            }
        }

        private static void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (Window.Current.Content.Lights.Count == 0)
            {
                Window.Current.Content.Lights.Add(new Lights.RevealAmbientLight());
                Window.Current.Content.Lights.Add(new Lights.RevealContentSpotLight());
                Window.Current.Content.Lights.Add(new Lights.RevealBorderSpotLight());
            }
        }

        public static void SetTargetLight(UIElement Target, SetLightMode Mode)
        {
            switch (Target)
            {
                case Border element:
                    switch (Mode)
                    {
                        case SetLightMode.None:
                            XamlLight.RemoveTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.Border:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.RemoveTargetBrush(Lights.RevealContentSpotLight.GetIdStatic(), element.Background);
                            break;
                        case SetLightMode.All:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.AddTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                    }
                    break;

                case Control element:
                    switch (Mode)
                    {
                        case SetLightMode.None:
                            XamlLight.RemoveTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.Border:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.All:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.AddTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                    }
                    break;

                case ContentPresenter element:
                    switch (Mode)
                    {
                        case SetLightMode.None:
                            XamlLight.RemoveTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.Border:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            if (element.Background is Brushes.RevealBrush)
                                XamlLight.RemoveTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                        case SetLightMode.All:
                            XamlLight.AddTargetElement(Lights.RevealAmbientLight.GetIdStatic(), element);
                            XamlLight.AddTargetElement(Lights.RevealContentSpotLight.GetIdStatic(), element);
                            break;
                    }
                    break;

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
            element.RemoveHandler(UIElement.PointerExitedEvent, new PointerEventHandler(element_PointerExited));
        }

        public enum SetLightMode
        {
            None = -1,
            Border = 0,
            Content = 1,
            All = 2
        }
    }
}

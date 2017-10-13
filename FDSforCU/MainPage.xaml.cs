using FDSforCU.Models;
using FDSforCU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace FDSforCU
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            HamburgerMenuList.Add(new MainPageModel() { Title = "亚克力笔刷", PageType = typeof(AcrylicPage) });
            HamburgerMenuList.Add(new MainPageModel() { Title = "展示效果", PageType = typeof(RevealPage) });
            ButtomMenuList.Add(new MainPageModel() { Title = "关于", PageType = typeof(AboutPage) });
            selectedModel = new MainPageModel();

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
        MainPageModel selectedModel;
        ObservableCollection<MainPageModel> HamburgerMenuList = new ObservableCollection<MainPageModel>();
        ObservableCollection<MainPageModel> ButtomMenuList = new ObservableCollection<MainPageModel>();

        private void HamburgerIconButton_Click(object sender, RoutedEventArgs e)
        {
            rootSplitView.IsPaneOpen = !rootSplitView.IsPaneOpen;
        }

        private void HamburgerMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (listView.SelectedIndex == -1) return;
                if (listView == HamburgerMenu)
                {
                    ButtomMenu.SelectedIndex = -1;
                }
                else if (listView == ButtomMenu)
                {
                    HamburgerMenu.SelectedIndex = -1;
                }
                var item = listView.SelectedItem as MainPageModel;
                selectedModel.Title = item.Title;
                selectedModel.PageType = item.PageType;
                ContentFrame.Navigate(selectedModel.PageType);
            }

        }

        private async void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListView listView)
            {
                await listView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => listView.SelectedIndex = 0);
            }
        }
    }
}

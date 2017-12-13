Fluent Design System for Creators Update V0.3
==============
更新内容:
1.Reveal Light效果与RedStone4同步;
2.大幅度优化光照逻辑(虽然不知道优化到底有没有效);
3.稳定性还是欠佳(别打我).

==============
Fluent Design System for Creators Update(FDS4CU)是一个UWP的资源库，将Windows10 Fall Creators Update(10.0;版本16299)SDK中的部分效果(Acrylic和Reveal)移植到Windows10 15063中。  
目前支持全部的Acrylic笔刷和Button，ToggleButton，RepeatButton，SemanticZoom，AutoSuggestBox,ComboBox,ListView和GridView的Reveal效果。  

Acrylic笔刷如下(详细参数可见/ResourceDictionarys/ThemeResources.xaml):  

    SystemControlAcrylicWindowBrush
    SystemControlAcrylicElementBrush
    SystemControlAccentAcrylicWindowAccentMediumHighBrush
    SystemControlAccentAcrylicElementAccentMediumHighBrush
    SystemControlAccentDark1AcrylicWindowAccentDark1Brush
    SystemControlAccentDark1AcrylicElementAccentDark1Brush
    SystemControlAccentDark2AcrylicWindowAccentDark2MediumHighBrush
    SystemControlAccentDark2AcrylicElementAccentDark2MediumHighBrush
    SystemControlAcrylicWindowMediumHighBrush
    SystemControlAcrylicElementMediumHighBrush
    SystemControlAcrylicWindowMediumBrush
    SystemControlAcrylicElementMediumBrush
    SystemControlChromeMediumLowAcrylicWindowMediumBrush
    SystemControlChromeMediumLowAcrylicElementMediumBrush
    SystemControlBaseHighAcrylicWindowBrush
    SystemControlBaseHighAcrylicElementBrush
    SystemControlBaseHighAcrylicWindowMediumHighBrush
    SystemControlBaseHighAcrylicElementMediumHighBrush
    SystemControlBaseHighAcrylicWindowMediumBrush
    SystemControlBaseHighAcrylicElementMediumBrush
    SystemControlChromeLowAcrylicWindowBrush
    SystemControlChromeLowAcrylicElementBrush
    SystemControlChromeMediumAcrylicWindowMediumBrush
    SystemControlChromeMediumAcrylicElementMediumBrush
    SystemControlChromeHighAcrylicWindowMediumBrush
    SystemControlChromeHighAcrylicElementMediumBrush
    SystemControlBaseLowAcrylicWindowBrush
    SystemControlBaseLowAcrylicElementBrush
    SystemControlBaseMediumLowAcrylicWindowMediumBrush
    SystemControlBaseMediumLowAcrylicElementMediumBrush
    SystemControlAltLowAcrylicWindowBrush
    SystemControlAltLowAcrylicElementBrush
    SystemControlAltMediumLowAcrylicWindowMediumBrush
    SystemControlAltMediumLowAcrylicElementMediumBrush
    SystemControlAltHighAcrylicWindowBrush
    SystemControlAltHighAcrylicElementBrush

开启Acrylic的方法如下:  
在App.xaml中添加:  

    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="FluentDesignSystem/ResourceDictionarys/Styles.xaml" />
                <ResourceDictionary Source="FluentDesignSystem/ResourceDictionarys/ThemeResources.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
引入相关资源字典即可使用所有笔刷。

***

Reveal样式如下(详细参数可见/ResourceDictionarys/Styles.xaml，AutoSuggestBox,ComboBox,ListView和GridView不需多做处理，其他需要设置Style="...RevealStyle"):  

    ButtonRevealStyle
    RepeatButtonRevealStyle
    ToggleButtonRevealStyle
    SemanticZoomRevealStyle
    AutoSuggestBoxRevealStyle
    ComboBoxItemRevealStyle
    ListViewItemRevealStyle
    GridViewItemRevealStyle

自定义控件开启Reveal方式如下:  
首先同上引入资源字典，然后编辑UserControl的VisualState:  

    <UserControl
        ...
        xmlns:fdsh="using:FluentDesignSystem.Helper">
        <Grid fdsh.RevealBrushHelper.State="Normal">
            <VisualStateManager.VisualStateManager>
                <VisualStateGroup x:Name="CommonStates">
                    VisualState x:Name="Normal" />
                    <VisualState x:Name="PointerOver">
                        <VisualState.Setters>
                            <Setter Target="RootGrid.(fdsh:RevealBrushHelper.State)" Value="PointerOver" />
                            <Setter Target="RootGrid.Background" Value="{ThemeResource ButtonRevealBackgroundPointerOver}"
                            <Setter Target="ContentPresenter.BorderBrush" Value="{ThemeResource ButtonRevealBorderBrushPointerOver}"
                            <Setter Target="ContentPresenter.Foreground"
                            Value="{ThemeResource ButtonForegroundPointerOver}" 
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Pressed">
                        <VisualState.Setters>
                            <Setter Target="RootGrid.(fdsh:RevealBrushHelper.State)" Value="Pressed" />
                            <Setter Target="RootGrid.Background" Value="{ThemeResource ButtonRevealBackgroundPressed}"
                            <Setter Target="ContentPresenter.BorderBrush" Value="{ThemeResource ButtonRevealBorderBrushPressed}"
                            <Setter Target="ContentPresenter.Foreground" Value="{ThemeResource ButtonForegroundPressed}"
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateManager.VisualStateGroup>
            ...
        </Grid>
    </UserControl>
然后在后台代码中处理VisualState状态转换。  

***

由于SDK接口的局限性，还有一些实现上的问题，Acrylic和Reveal表现并不是太稳定，欢迎大家Fork和反馈。  

By:叫我蓝火火  
Blog:[叫我蓝火火](http://www.cnblogs.com/blue-fire/)  
微博:[Blue_Fire蓝火](http://www.weibo.com/2255001067/profile)  
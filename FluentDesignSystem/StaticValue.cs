using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FluentDesignSystem
{

    public static class StaticValue
    {
        #region LightCollections
        private static Dictionary<UIElement, Lights.LightCollection> _LightCollectionDictionary;

        public static Dictionary<UIElement, Lights.LightCollection> LightCollectionDictionary
        {
            get
            {
                if (_LightCollectionDictionary == null) _LightCollectionDictionary = new Dictionary<UIElement, Lights.LightCollection>();
                return _LightCollectionDictionary;
            }
        }
        #endregion

        #region BaseElementCollection
        private static Dictionary<UIElement, UIElement> _BaseElementDictionary;

        public static Dictionary<UIElement, UIElement> BaseElementDictionary
        {
            get
            {
                if (_BaseElementDictionary == null) _BaseElementDictionary = new Dictionary<UIElement, UIElement>();
                return _BaseElementDictionary;
            }
        }
        #endregion

        #region BaseElementOperation
        public static Collection<string> PopupNameList = new Collection<string>()
        {
            "Popup","SuggestionsPopup","OverflowPopup"
        };

        public static void AddBaseElement(this UIElement element, UIElement BaseElement)
        {
            if (StaticValue.BaseElementDictionary.ContainsKey(element)) return;
            StaticValue.BaseElementDictionary.Add(element, BaseElement);
        }

        public static void UpdateBaseElement(this UIElement element, UIElement BaseElement)
        {
            if (StaticValue.BaseElementDictionary.ContainsKey(element)) StaticValue.BaseElementDictionary[element] = BaseElement;
            else StaticValue.BaseElementDictionary.Add(element, BaseElement);
        }

        public static void RemoveBaseElement(this UIElement element)
        {
            if (StaticValue.BaseElementDictionary.ContainsKey(element)) StaticValue.BaseElementDictionary.Remove(element);
        }

        public static UIElement GetBaseElement(this UIElement element)
        {
            if (StaticValue.BaseElementDictionary.ContainsKey(element)) return StaticValue.BaseElementDictionary[element];
            return null;
        }
        #endregion

        #region CoreWindowHandle
        private static bool _CoreWindowHandled = false;
        public static bool CoreWindowHandled
        {
            get => _CoreWindowHandled;
            set => _CoreWindowHandled = value;
        }
        #endregion
    }
}


/* 设计思路
 * LightCollection给Base添加基础光照
 * DPChanged给每个element添加target
 * DPChanged内判断IsOpen的Popup并设置状态
 * 如果IsOpen的Popup为0个，则为Window设置状态
 * Base的Pointer事件控制Base的基础光照种类
 */

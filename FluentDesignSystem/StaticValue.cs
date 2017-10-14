using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDesignSystem
{

    public static class StaticValue
    {
        #region Handlers
        public static bool IsWindowPointerMovedHandled = false;
        public static bool IsWindowPointerEnteredHandled = false;
        public static bool IsWindowPointerExitedHandled = false;
        #endregion

        #region Lights
        public static Lights.RevealAmbientLight AmbientLight;
        public static Lights.RevealBorderSpotLight BorderLight;
        public static Lights.RevealContentSpotLight ContentLight;
        #endregion

        #region ChildLights
        public static Lights.RevealAmbientLight ChildAmbientLight
        {
            get => new Lights.RevealAmbientLight();
        }
        public static Lights.RevealBorderSpotLight ChildBorderLight
        {
            get => new Lights.RevealBorderSpotLight();
        }
        public static Lights.RevealContentSpotLight ChildContentLight
        {
            get => new Lights.RevealContentSpotLight();
        }
        #endregion

        public static List<string> InPopupGridNameList = new List<string>()
        {
            "ContentBorder","LayoutRoot","Root"
        };
    }
}

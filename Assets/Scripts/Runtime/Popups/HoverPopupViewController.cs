using RIEVES.GGJ2026.Core.Views;

namespace RIEVES.GGJ2026.Runtime.Popups
{
    internal sealed class HoverPopupViewController : ViewController<HoverPopupView>
    {
        public string TitleText
        {
            set => View.TitleText = value;
        }
    }
}

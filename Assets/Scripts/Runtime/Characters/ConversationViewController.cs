using RIEVES.GGJ2026.Core.Views;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationViewController : ViewController<ConversationView>
    {
        public void Initialize(string title, string content)
        {
            View.TitleText = title;
            View.ContentText = content;
        }
    }
}

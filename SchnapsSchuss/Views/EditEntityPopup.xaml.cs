using CommunityToolkit.Maui.Views;

namespace SchnapsSchuss.Views;

public partial class EditEntityPopup : Popup<string>
{
    public EditEntityPopup()
    {
        InitializeComponent();
    }

    public View EditorContent
    {
        set => Properties.Content = value;
    } 
}
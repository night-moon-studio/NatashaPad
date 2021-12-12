namespace NatashaPad.Mvvm.MessageBox;

public static class MessageBoxServiceExtensions
{
    public static void Show(this IErrorMessageBoxService messageBoxService, string content)
    {
        messageBoxService.ShowError(Properties.Resource.ErrorMessageBoxTitleString, content);
    }
}
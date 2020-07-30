using MediatR;

namespace NatashaPad.MvvmServices.MessageBox
{
    public class MessageNotificationHandler : NotificationHandler<MessageNotification>
    {
        protected override void Handle(MessageNotification notification)
        {
            System.Windows.MessageBox.Show(notification.Message,
                Properties.Resource.ErrorMessageBoxTitleString);
        }
    }
}

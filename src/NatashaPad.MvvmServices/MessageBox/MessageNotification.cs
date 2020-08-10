using MediatR;

using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices.MessageBox
{
    public class MessageNotification : INotification
    {
        public MessageNotification(string message) : this(message, Level.Info)
        {
        }

        public MessageNotification(string message, Level level)
        {
            Message = message;
            Level = level;
        }

        public string Message { get; }
        public Level Level { get; }
    }

    public enum Level
    {
        Info,
        Warn,
        Error
    }
}

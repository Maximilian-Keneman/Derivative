using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Derivative
{
    [Serializable]
    public class TextException : Exception, ISerializable
    {
        public char? InvalidChar { get; }
        public int Position { get; }
        public override string Message
        {
            get
            {
                if (Position < 0)
                    return base.Message;
                else
                {
                    string addMessage = $"{(InvalidChar != null ? $"'{InvalidChar}' i" : "I")}n position {Position}.";
                    return $"{base.Message + (base.Message.Last() == '.' ? "" : ".")} " + addMessage;
                }
            }
        }
        private string BaseMessage => base.Message;

        public TextException()
        {
            Position = -1;
            InvalidChar = null;
        }

        public TextException(string message) : base(message)
        {
            Position = -1;
            InvalidChar = null;
        }

        public TextException(string message, int position) : base(message)
        {
            Position = position;
            InvalidChar = null;
        }

        public TextException(string message, char invalidChar, int position) : base(message)
        {
            Position = position;
            InvalidChar = invalidChar;
        }

        public TextException(int position, TextException innerException) : base(innerException.BaseMessage, innerException)
        {
            if (innerException.Position > -1)
            {
                Position = innerException.Position + position;
                InvalidChar = innerException.InvalidChar;
            }
            else
            {
                Position = -1;
                InvalidChar = null;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Position", Position);
            if (InvalidChar != null)
                info.AddValue("InvalidChar", InvalidChar);
        }
        protected TextException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Position = info.GetInt32("Position");
            try
            {
                InvalidChar = info.GetChar("InvalidChar");
            }
            catch (SerializationException)
            {
                InvalidChar = null;
            }
        }
    }
}

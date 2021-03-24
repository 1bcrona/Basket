using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace Basket.Library.Exception
{
    public abstract class BaseException : System.Exception
    {
        #region Private Fields

        private static readonly object _MessageFieldLock = new object();
        private static FieldInfo _MessageField;
        private string _Message;

        #endregion Private Fields

        #region Protected Constructors

        protected BaseException()
        {
        }

        protected BaseException(string message) : base(message)
        {
            Init(message, null);
        }

        protected BaseException(string message, System.Exception innerException) : base(message, innerException)
        {
            Init(message, null);
        }

        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected BaseException(string message, Dictionary<string, string> args) : base(message)
        {
            Init(message, args);
        }

        #endregion Protected Constructors

        #region Public Properties

        public override string Message =>
            string.IsNullOrEmpty((_Message ?? string.Empty).Trim()) ? base.Message : _Message;

        public abstract string Name { get; }

        #endregion Public Properties

        #region Private Methods

        private static FieldInfo GetMessageMemberInfo()
        {
            lock (_MessageFieldLock)
            {
                if (_MessageField == null)
                {
                    var messageField = typeof(System.Exception).GetField("_message",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

                    Interlocked.Exchange(ref _MessageField, messageField);
                }

                return _MessageField;
            }
        }

        private string GetParameterKey(string parameter)
        {
            return $"PRM_{parameter}";
        }

        private void Init(string message, Dictionary<string, string> args)
        {
            if (args != null)
                message = args.Aggregate(message,
                    (current, arg) => current.Replace(GetParameterKey(arg.Key), arg.Value));
            SetMessageToException(message);
        }

        private void SetMessageToException(string message)
        {
            if (TrySetMessage(message)) _Message = (message ?? string.Empty).Trim();
        }

        private bool TrySetMessage(string message)
        {
            try
            {
                var fieldInfo = GetMessageMemberInfo();

                if (fieldInfo == null) return false;

                fieldInfo.SetValue(this, message);
                return Message == message;
            }
            catch (System.Exception)
            {
                //Ignored
            }

            return false;
        }

        #endregion Private Methods
    }
}
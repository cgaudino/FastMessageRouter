using System;

namespace FastMessageRouter
{
    public partial class MessageRouter
    {
        private static class MessageBinding<TMessage> where TMessage : new()
        {
            private static TMessage _defaultInstance;

            private static event Action<TMessage> _delegate;
            private static event Action _paramaterlessDelegate;

            static MessageBinding()
            {
                _defaultInstance = new TMessage();
            }

            public static void BindMessageHandler(Action<TMessage> handler)
            {
                _delegate += handler;
            }

            public static void BindMessageHandler(Action handler)
            {
                _paramaterlessDelegate += handler;
            }

            public static void RemoveMessageHandler(Action<TMessage> handler)
            {
                _delegate -= handler;
            }

            public static void RemoveMessageHandler(Action handler)
            {
                _paramaterlessDelegate -= handler;
            }

            public static void RaiseMessage(TMessage handler)
            {
                _delegate?.Invoke(handler);
                _paramaterlessDelegate?.Invoke();
            }

            public static void RaiseMessage()
            {
                _delegate?.Invoke(_defaultInstance);
                _paramaterlessDelegate?.Invoke();
            }
        }
    }
}

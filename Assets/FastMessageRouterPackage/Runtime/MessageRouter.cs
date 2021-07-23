using System.Collections.Generic;
using System;

namespace FastMessageRouter
{
    public partial class MessageRouter 
    {
        private List<Action> _autoUnbindDelegates;
        private List<Action> _queuedUnbindDelegates;

        private bool _isRaisingMessage;

#if FMR_USE_SINGLETON 
        public static MessageRouter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageRouter();
                return _instance;
            }
        }

        private static MessageRouter _instance;
#endif

        public MessageRouter()
        {
            _autoUnbindDelegates = new List<Action>();
            _queuedUnbindDelegates = new List<Action>();
        }

        ~MessageRouter()
        {
            ClearAllHandlers();
        }

        public void ClearAllHandlers()
        {
            if (_isRaisingMessage)
            {
                _queuedUnbindDelegates.AddRange(_autoUnbindDelegates);
            }
            else
            {
                for (int i = 0; i < _autoUnbindDelegates.Count; ++i)
                {
                    _autoUnbindDelegates[i].Invoke();
                }
            }
            _autoUnbindDelegates.Clear();
        }

        public void BindMessageHandler<TMessage>(Action<TMessage> handler)
            where TMessage : new()
        {
            MessageBinding<TMessage>.BindMessageHandler(handler);
            _autoUnbindDelegates.Add(() => MessageBinding<TMessage>.RemoveMessageHandler(handler));
        }

        public void BindMessageHandler<TMessage>(Action handler)
            where TMessage : new()
        {
            MessageBinding<TMessage>.BindMessageHandler(handler);
            _autoUnbindDelegates.Add(() => MessageBinding<TMessage>.RemoveMessageHandler(handler));
        }

        public void RemoveMessageHandler<TMessage>(Action<TMessage> handler)
            where TMessage : new()
        {
            if (_isRaisingMessage)
            {
                _queuedUnbindDelegates.Add(() => MessageBinding<TMessage>.RemoveMessageHandler(handler));
                return;
            }

            MessageBinding<TMessage>.RemoveMessageHandler(handler);
        }

        public void RemoveMessageHandler<TMessage>(Action handler)
            where TMessage : new()
        {
            if (_isRaisingMessage)
            {
                _queuedUnbindDelegates.Add(() => MessageBinding<TMessage>.RemoveMessageHandler(handler));
                return;
            }
            MessageBinding<TMessage>.RemoveMessageHandler(handler);
        }

        public void RaiseMessage<TMessage>() 
            where TMessage : new()
        {
            _isRaisingMessage = true;
            MessageBinding<TMessage>.RaiseMessage();
            _isRaisingMessage = false;

            ProcessQueuedUnbinds();
        }

        public void RaiseMessage<TMessage>(TMessage message) 
            where TMessage : new()
        {
            _isRaisingMessage = true;
            MessageBinding<TMessage>.RaiseMessage(message);
            _isRaisingMessage = false;

            ProcessQueuedUnbinds();
        }

        public TMessage GetSharedMessageInstance<TMessage>()
            where TMessage : new()
        {
            return MessageBinding<TMessage>.GetSharedInstance();
        }

        private void ProcessQueuedUnbinds()
        {
            for(int i = 0; i < _queuedUnbindDelegates.Count; ++i)
            {
                _queuedUnbindDelegates[i].Invoke();
            }
            _queuedUnbindDelegates.Clear();
        }
    }
}

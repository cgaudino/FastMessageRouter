using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace FastMessageRouter.Samples
{
    public class BenchmarkAgainstTypeDictionary : MonoBehaviour
    {
        private class DummyMessage { }

        private class DictionaryRouter
        {
            private Dictionary<Type, List<Delegate>> _handlers;
            private object[] _messageBuffer;

            public DictionaryRouter()
            {
                _messageBuffer = new object[1];
                _handlers = new Dictionary<Type, List<Delegate>>();
            }

            public void BindMessageHandler<T>(Action<T> handler)
            {
                if(!_handlers.TryGetValue(typeof(T), out var delegates))
                {
                    delegates = new List<Delegate>();
                    _handlers[typeof(T)] = delegates;
                }
                delegates.Add(handler);
            }

            public void RaiseMessage<T>(T message)
            {
                if(_handlers.TryGetValue(typeof(T), out var delegates))
                {
                    _messageBuffer[0] = message;
                    for(int i = 0; i < delegates.Count; ++i)
                    {
                        delegates[i].Method.Invoke(delegates[i].Target, _messageBuffer);
                    }
                    _messageBuffer[0] = null;
                }
            }

            public void ClearAllHandlers()
            {
                _handlers.Clear();
            }
        }

        [SerializeField] private InputField _iterationsInputField = default;
        [SerializeField] private Button _runButton = default;
        [SerializeField] private Text _resultsText = default;

        private DummyMessage _message;
        private DictionaryRouter _dictionaryRouter;
        private MessageRouter _messageRouter;
        private int _counter;

        private void Start()
        {
            _message = new DummyMessage();
            _dictionaryRouter = new DictionaryRouter();
            _messageRouter = new MessageRouter();

            _runButton.onClick.AddListener(RunBenchmark);
        }

        private void OnDestroy()
        {
            _messageRouter.ClearAllHandlers();
            _dictionaryRouter.ClearAllHandlers();
            _runButton.onClick.RemoveListener(RunBenchmark);
        }

        private void RunBenchmark()
        {
            _messageRouter.BindMessageHandler<DummyMessage>(HandleMessage);
            _dictionaryRouter.BindMessageHandler<DummyMessage>(HandleMessage);

            if(!int.TryParse(_iterationsInputField.text, out int iterations))
            {
                UnityEngine.Debug.LogError($"Unable to parse number of iterations.");
                return;
            }

            double dictionaryTime = Profile(iterations, SendDictionaryMessage);
            double messageRouterTime = Profile(iterations, SendRouterMessage);

            UnityEngine.Debug.Log(_counter);

            _resultsText.text = $"Elapsed Times:\r\nMessage Router: {messageRouterTime}ms\r\nDictionary: {dictionaryTime}ms";

            _messageRouter.ClearAllHandlers();
            _dictionaryRouter.ClearAllHandlers();
        }

        private void SendRouterMessage()
        {
            _messageRouter.RaiseMessage(_message);
        }

        private void SendDictionaryMessage()
        {
            _dictionaryRouter.RaiseMessage(_message);
        }

        private void HandleMessage(DummyMessage dummy)
        {
            _counter += 1;
        }

        static double Profile(int iterations, Action func)
        {
            //Run at highest priority to minimize fluctuations caused by other processes/threads
#if !ENABLE_IL2CPP
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
#endif
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

            // warm up 
            func();

            var watch = new Stopwatch();

            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            watch.Start();
            for (int i = 0; i < iterations; i++)
            {
                func();
            }
            watch.Stop();

            return watch.Elapsed.TotalMilliseconds;
        }
    }
}

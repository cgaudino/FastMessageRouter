using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FastMessageRouter.Tests
{
    public class MessageRouterTests
    {
        public class TestMessageA { }
        public class TestMessageB { }

        private MessageRouter _messageRouter;

        [SetUp]
        public void SetUp()
        {
            _messageRouter = new MessageRouter();
        }

        [TearDown]
        public void TearDown()
        {
            _messageRouter?.ClearAllHandlers();
            _messageRouter = null;
        }

        [Test]
        public void Handlers_With_Parameters_Are_Bound_And_Raised_And_Removed()
        {
            int number = 0;

            System.Action<TestMessageA> handler = (TestMessageA message) =>
            {
                number += 1;
            };

            _messageRouter.BindMessageHandler<TestMessageA>(handler);
            _messageRouter.RaiseMessage<TestMessageA>();

            Assert.AreEqual(number, 1);

            _messageRouter.RemoveMessageHandler<TestMessageA>(handler);
            _messageRouter.RaiseMessage<TestMessageA>();

            Assert.AreEqual(number, 1);
        }

        [Test]
        public void Handlers_Without_Parameters_Are_Bound_And_Raised_And_Removed()
        {
            int number = 0;

            System.Action handler = () =>
            {
                number += 1;
            };

            _messageRouter.BindMessageHandler<TestMessageA>(handler);
            _messageRouter.RaiseMessage<TestMessageA>();

            Assert.AreEqual(number, 1);

            _messageRouter.RemoveMessageHandler<TestMessageA>(handler);
            _messageRouter.RaiseMessage<TestMessageA>();

            Assert.AreEqual(number, 1);
        }

        [Test]
        public void Messages_Raised_Without_Parameters_Do_Not_Send_Null()
        {
            TestMessageA parameter = null;

            System.Action<TestMessageA> handler = (TestMessageA message) =>
            {
                parameter = message;
            };

            _messageRouter.BindMessageHandler<TestMessageA>(handler);
            _messageRouter.RaiseMessage<TestMessageA>();

            Assert.IsNotNull(parameter);
        }

        [Test]
        public void Messages_Raised_With_Parameters_Do_Not_Send_Default_Instance()
        {
            TestMessageA parameter = new TestMessageA();
            TestMessageA receivedMessage = null;

            System.Action<TestMessageA> handler = (message) => receivedMessage = message;

            _messageRouter.BindMessageHandler(handler);
            _messageRouter.RaiseMessage(parameter);

            Assert.AreSame(parameter, receivedMessage);
        }

        [Test]
        public void Messages_Are_Delivered_To_Multiple_Handlers()
        {
            bool handlerAWasCalled = false;
            System.Action handlerA = () => handlerAWasCalled = true;

            bool handlerBWasCalled = false;
            System.Action handlerB = () => handlerBWasCalled = true;

            _messageRouter.BindMessageHandler<TestMessageA>(handlerA);
            _messageRouter.BindMessageHandler<TestMessageA>(handlerB);

            _messageRouter.RaiseMessage<TestMessageA>();

            Assert.IsTrue(handlerAWasCalled);
            Assert.IsTrue(handlerBWasCalled);
        }

        [Test]
        public void Clear_All_Removes_All_Bound_Handlers()
        {
            int a = 0, b = 0;

            System.Action handlerA = () => a += 1;
            System.Action handlerB = () => b += 1;

            _messageRouter.BindMessageHandler<TestMessageA>(handlerA);
            _messageRouter.BindMessageHandler<TestMessageB>(handlerB);

            _messageRouter.RaiseMessage<TestMessageA>();
            _messageRouter.RaiseMessage<TestMessageB>();

            _messageRouter.ClearAllHandlers();

            _messageRouter.RaiseMessage<TestMessageA>();
            _messageRouter.RaiseMessage<TestMessageB>();

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 1);
        }

        [Test]
        public void Messages_Are_Only_Send_To_Handlers_Bound_To_Same_Type()
        {
            int a = 0, b = 0;

            System.Action handlerA = () => a += 1;
            System.Action handlerB = () => b += 1;

            _messageRouter.BindMessageHandler<TestMessageA>(handlerA);
            _messageRouter.BindMessageHandler<TestMessageB>(handlerB);

            _messageRouter.RaiseMessage<TestMessageA>();

            _messageRouter.ClearAllHandlers();

            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 0);
        }
    }
}

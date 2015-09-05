﻿namespace NServiceBus.AcceptanceTests.Mutators
{
    using System;
    using NServiceBus.AcceptanceTesting;
    using NServiceBus.AcceptanceTests.EndpointTemplates;
    using NServiceBus.MessageMutator;
    using NUnit.Framework;

    public class When_outgoing_mutator_replaces_instance : NServiceBusAcceptanceTest
    {
        static Context testContext = new Context();
        [Test]
        public void Message_sent_should_be_new_instance()
        {
            Scenario.Define(testContext)
                .WithEndpoint<Endpoint>(b => b.Given((bus, c) => bus.SendLocal(new V1Message())))
                .Done(c => c.V2MessageReceived)
                .Run();

            Assert.IsTrue(testContext.V2MessageReceived);
            Assert.IsFalse(testContext.V1MessageReceived);
        }

        public class Context : ScenarioContext
        {
            public bool V1MessageReceived { get; set; }
            public bool V2MessageReceived { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(
                    b => b.RegisterComponents(r => r.ConfigureComponent<MutateOutgoingMessages>(DependencyLifecycle.InstancePerCall)));
            }

            class MutateOutgoingMessages : IMutateOutgoingMessages
            {
                public void MutateOutgoing(MutateOutgoingMessageContext context)
                {
                    if (context.MessageInstance is V1Message)
                    {
                        context.MessageInstance = new V2Message();
                    }
                }
            }

            class V2Handler : IHandleMessages<V2Message>
            {
                public void Handle(V2Message message)
                {
                    testContext.V2MessageReceived = true;
                }
            }

            class V1Handler : IHandleMessages<V1Message>
            {
                public void Handle(V1Message message)
                {
                    testContext.V1MessageReceived = true;
                }
            }
        }

        [Serializable]
        public class V1Message : ICommand
        {
        }

        [Serializable]
        public class V2Message : ICommand
        {
        }
    }
}
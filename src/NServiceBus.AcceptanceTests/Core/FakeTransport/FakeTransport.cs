﻿using System.Threading.Tasks;
#pragma warning disable 1998

namespace NServiceBus.AcceptanceTests.Core.FakeTransport
{
    using System;
    using System.Collections.Generic;
    using Settings;
    using Transport;

    public class FakeTransport : TransportDefinition
    {
        public TransportTransactionMode? SupportedTransactionMode { get; set; }

        public List<string> StartUpSequence { get; } = new List<string>();

        public bool ThrowOnInfrastructureStop { get; set; }

        public bool RaiseCriticalErrorDuringStartup { get; set; }

        public bool ThrowOnPumpStop { get; set; }

        public Exception ExceptionToThrow { get; set; } = new Exception();

        public Action<QueueBindings> OnQueueCreation { get; set; }

        public override async Task<TransportInfrastructure> Initialize(Settings settings)
        {
            StartUpSequence.Add($"{nameof(TransportDefinition)}.{nameof(Initialize)}");

            ////if (settings.TryGet<Action<ReadOnlySettings>>("FakeTransport.AssertSettings", out var assertion))
            ////{
            ////    assertion(settings);
            ////}

            return new FakeTransportInfrastructure(settings, this);
        }

        public override string ToTransportAddress(EndpointAddress logicalAddress)
        {
            return logicalAddress.ToString();
        }

        public override TransportTransactionMode MaxSupportedTransactionMode => this.SupportedTransactionMode ?? TransportTransactionMode.TransactionScope;

    }
}
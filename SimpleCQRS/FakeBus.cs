﻿using System;
using System.Collections.Generic;
using System.Threading;
using SimpleCQRS.Commands;
using SimpleCQRS.Events;

namespace SimpleCQRS
{
    public class FakeBus : ICommandSender, IEventPublisher
    {
        private readonly Dictionary<Type, List<Action<Message>>> _routes = new Dictionary<Type, List<Action<Message>>>();

        public void RegisterHandler<T>(Action<T> handler) where T : Message
        {
            List<Action<Message>> handlers;
            if(!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<Message, T>(x => handler(x)));
        }

        public void Send<T>(T command) where T : Command
        {
            if(command == null)
                throw new ArgumentNullException("command");

            List<Action<Message>> handlers; 
            if (_routes.TryGetValue(command.GetType(), out handlers))
            {
                if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }

        public void Publish<T>(T @event) where T : Event
        {
            List<Action<Message>> handlers; 
            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;
            foreach(var handler in handlers)
            {
                handler(@event);
            }
        }
    }

    public interface Handles<T>
    {
        void Handle(T message);
    }

    public interface ICommandSender
    {
        void Send<T>(T command) where T : Command;

    }
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : Event;
    }
}

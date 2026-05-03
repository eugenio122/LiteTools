using System;
using System.Collections.Generic;
using LiteTools.Interfaces; // IMPORTANTE: Referencia a interface do contrato

namespace LiteTools.Core
{
    /// <summary>
    /// O motor central de mensagens (Mediator). 
    /// Implementa o IEventBus para ser injetado nos plugins.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }
            _subscribers[eventType].Add(handler);
        }

        public void Publish<TEvent>(TEvent eventItem)
        {
            var eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                foreach (var handler in _subscribers[eventType])
                {
                    ((Action<TEvent>)handler)(eventItem);
                }
            }
        }
    }
}
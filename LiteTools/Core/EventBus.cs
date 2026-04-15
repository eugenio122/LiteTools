using System;
using System.Collections.Generic;

namespace LiteTools.Core
{
    /// <summary>
    /// O motor central de mensagens. 
    /// Agora vive APENAS no Host. Os plugins não sabem da sua existência, 
    /// eles comunicam apenas através das interfaces.
    /// </summary>
    public class EventBus
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
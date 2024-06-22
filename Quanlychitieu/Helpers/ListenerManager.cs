using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public class ListenerManager
    {
        private static ListenerManager instance;
        private Dictionary<string, Action<object>> _listeners;

        private ListenerManager()
        {
            _listeners = new Dictionary<string, Action<object>>();
        }

        public static ListenerManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ListenerManager();
                return instance;
            }
        }

        public void AddListener(string eventName, Action<object> listener)
        {
            if (!_listeners.ContainsKey(eventName))
            {
                _listeners.Add(eventName, listener);
            }
            else
            {
                _listeners[eventName] += listener;
            }
        }

        public void RemoveListener(string eventName, Action<object> listener)
        {
            if (_listeners.ContainsKey(eventName))
            {
                _listeners[eventName] -= listener;
                if (_listeners[eventName] == null)
                    _listeners.Remove(eventName);
            }
        }

        public void SentEvent(string eventName, object data)
        {
            if (_listeners.ContainsKey(eventName))
                _listeners[eventName]?.Invoke(data);
        }
    }
}

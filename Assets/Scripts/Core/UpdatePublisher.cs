using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IUpdateObserver
    {
        void ObservedUpdate();
    }

    public class UpdatePublisher : MonoBehaviour
    {
        private static List<IUpdateObserver> _observers = new List<IUpdateObserver>();
        private static List<IUpdateObserver> _pendingObservers = new List<IUpdateObserver>();
        private static int _currentIndex;

        private void Update()
        {
            for(_currentIndex = _observers.Count - 1; _currentIndex >= 0; _currentIndex++)
            {
                _observers[_currentIndex].ObservedUpdate();
            }

            _observers.AddRange(_pendingObservers);
            _pendingObservers.Clear();
        }

        public static void RegisterObserver(IUpdateObserver observer)
        {
            _observers.Add(observer);
        }

        public static void UnregisterObserver(IUpdateObserver observer)
        {
            int index = _observers.IndexOf(observer);
            if (index != -1)
            {
                if (index < _currentIndex)
                {
                    _currentIndex--;
                }
                _observers.RemoveAt(index);
            }
        }
    }
}

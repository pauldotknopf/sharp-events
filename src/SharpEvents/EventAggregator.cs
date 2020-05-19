using System;
using WeakEvent;

namespace SharpEvents
{
    public class EventAggregator<TEvent> : IEventAggregatorConsumer<TEvent>
    {
        private readonly SharpEventDispatcherDel _dispatcher;
        private readonly WeakEventSource<TEvent> _eventSource = new WeakEventSource<TEvent>();

        public EventAggregator(SharpEventDispatcherDel dispatcher = null)
        {
            _dispatcher = dispatcher;
        }
        
        public IDisposable Subscribe(EventHandler<TEvent> action)
        {
            _eventSource.Subscribe(action);
            return new Disposable(_eventSource, action);
        }

        public void Publish(object sender, TEvent args)
        {
            if (_dispatcher != null)
            {
                _dispatcher(() =>
                {
                    _eventSource.Raise(sender, args);
                });
            }
            else
            {
                _eventSource.Raise(sender, args);
            }
        }
        
        public void Unsubscribe(EventHandler<TEvent> action)
        {
            _eventSource.Unsubscribe(action);
        }

        private class Disposable : IDisposable
        {
            private readonly WeakEventSource<TEvent> _eventSource;
            private readonly EventHandler<TEvent> _handler;
            private bool _isDisposed;

            public Disposable(WeakEventSource<TEvent> eventSource, EventHandler<TEvent> handler)
            {
                _eventSource = eventSource;
                _handler = handler;
            }
            
            public void Dispose()
            {
                var isDisposed = _isDisposed;
                _isDisposed = true;
                if (!isDisposed)
                {
                    _eventSource.Unsubscribe(_handler);
                }
            }
        }
    }

    public interface IEventAggregatorConsumer<TEvent>
    {
        IDisposable Subscribe(EventHandler<TEvent> action);

        void Unsubscribe(EventHandler<TEvent> action);
    }
}
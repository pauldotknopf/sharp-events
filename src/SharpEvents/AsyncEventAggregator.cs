using System;
using System.Threading.Tasks;
using WeakEvent;

namespace SharpEvents
{
    public class AsyncEventAggregator<TEvent> : IAsyncEventAggregatorConsumer<TEvent>
    {
        private readonly SharpEventDispatcherAsyncDel _dispatcher;
        private readonly AsyncWeakEventSource<TEvent> _eventSource = new AsyncWeakEventSource<TEvent>();

        public AsyncEventAggregator(SharpEventDispatcherAsyncDel dispatcher = null)
        {
            _dispatcher = dispatcher;
        }
        
        public IDisposable Subscribe(AsyncEventHandler<TEvent> action)
        {
            _eventSource.Subscribe(action);
            return new Disposable(_eventSource, action);
        }

        public async Task Publish(object sender, TEvent args)
        {
            if (_dispatcher != null)
            {
                await _dispatcher(() =>
                {
                    _eventSource.RaiseAsync(sender, args);
                });
            }
            else
            {
                await _eventSource.RaiseAsync(sender, args);
            }
        }
        
        public void Unsubscribe(AsyncEventHandler<TEvent> action)
        {
            _eventSource.Unsubscribe(action);
        }
        
        private class Disposable : IDisposable
        {
            private readonly AsyncWeakEventSource<TEvent> _eventSource;
            private readonly AsyncEventHandler<TEvent> _handler;
            private bool _isDisposed;

            public Disposable(AsyncWeakEventSource<TEvent> eventSource, AsyncEventHandler<TEvent> handler)
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

    public interface IAsyncEventAggregatorConsumer<TEvent>
    {
        IDisposable Subscribe(AsyncEventHandler<TEvent> action);

        void Unsubscribe(AsyncEventHandler<TEvent> action);
    }
}
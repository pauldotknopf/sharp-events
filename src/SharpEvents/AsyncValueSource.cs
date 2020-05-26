using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpEvents
{
    public class AsyncValueSource<T> : IAsyncValueSourceReadOnly<T>
    {
        private T _value;
        private readonly AsyncEventAggregator<T> _valueChanged;

        public AsyncValueSource(SharpEventDispatcherAsyncDel dispatcher = null)
        {
            _valueChanged = new AsyncEventAggregator<T>(dispatcher);
        }

        public T Value => _value;

        public IAsyncEventAggregatorConsumer<T> ValueChanged => _valueChanged;
        
        public async Task<bool> Change(T value)
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return false;

            _value = value;

            await _valueChanged.Publish(this, _value);

            return true;
        }
    }
    
    public interface IAsyncValueSourceReadOnly<T>
    {
        T Value { get; }

        IAsyncEventAggregatorConsumer<T> ValueChanged { get; }
    }
}
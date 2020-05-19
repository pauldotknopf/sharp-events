using System.Collections.Generic;

namespace SharpEvents
{
    public class ValueSource<T> : IValueSourceReadOnly<T>
    {
        private T _value;
        private readonly EventAggregator<T> _valueChanged;

        public ValueSource(SharpEventDispatcherDel dispatcher)
        {
            _valueChanged = new EventAggregator<T>(dispatcher);
        }

        public T Value => _value;

        public IEventAggregatorConsumer<T> ValueChanged => _valueChanged;
        
        public bool Change(T value)
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return false;

            _value = value;

            _valueChanged.Publish(this, _value);

            return true;
        }
    }
    
    public interface IValueSourceReadOnly<T>
    {
        T Value { get; }

        IEventAggregatorConsumer<T> ValueChanged { get; }
    }
}
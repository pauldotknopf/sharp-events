# SharpEvents

Some common types that make working with events a bit easier to work with and understand.

[![SharpEvents](https://img.shields.io/nuget/v/SharpEvents.svg?style=flat-square&label=SharpEvents)](http://www.nuget.org/packages/SharpEvents/)

# Why

That standard ```event``` keyword in C# lacked (suitable) async support. It also produced memory leaks if handlers weren't removed, which was solved by using [this](https://github.com/thomaslevesque/WeakEvent) library internally.

## Event aggregators

With the ```AsyncEventAggregator<T> : IAsyncEventAggregatorConsumer<T>``` and non-async ```EventAggregator<T> : IEventAggregatorConsumer<T>``` types, you can do the following:

```c#
public class MyEventData
{
    public string Name { get; set; }
}

public interface IService
{
    // Exposes .Subscribe() and .Unsubscribe()
    IAsyncEventAggregatorConsumer<MyEventData> MyEvent { get; }

    Task Hello(string name);
}

public class Service : IService
{
    // Implements IAsyncEventAggregatorConsumer, but also
    // has a .Publish() method, meant to be used internally.
    private readonly AsyncEventAggregator<MyEventData> _myEvent = new AsyncEventAggregator<MyEventData>();
    
    public IAsyncEventAggregatorConsumer<MyEventData> MyEvent => _myEvent;
    
    public async Task Hello(string name)
    {
        await _myEvent.Publish(this, new MyEventData
        {
            Name = name
        });
    }
}
```

Then, when you consume it:

```c#
IService service = new Service();

IDisposable subscriptionScope = service.MyEvent.Subscribe((sender, data) =>
{
    Console.WriteLine($"Hello {data.Name}!");
    return Task.CompletedTask;
});

// Do something else, idk.

// All done with event handler, destroy.
subscriptionScope.Dispose();
```

## Value sources

With the ```ValueSource<T> : IValueSourceReadOnly<T>``` type, you can do the following:

```c#
public interface IService
{
    // Exposes .Subscribe() and .Unsubscribe()
    IValueSourceReadOnly<string> Name { get; }

    void ChangeName(string name);
}

public class Service : IService
{
    private readonly ValueSource<string> _name = new ValueSource<string>();

    public IValueSourceReadOnly<string> Name => _name;
    
    public void ChangeName(string name)
    {
        _name.Change(name);
    }
}
```

Then, when you consume it:

```c#
IService service = new Service();

IDisposable subscriptionScope = service.Name.ValueChanged.Subscribe((sender, name) =>
{
    Console.WriteLine($"Name changed to {name}!");
    Console.WriteLine($"Name: {service.Name.Value}");
});

service.ChangeName("Paul");

// All done with event handler, destroy.
subscriptionScope.Dispose();
```
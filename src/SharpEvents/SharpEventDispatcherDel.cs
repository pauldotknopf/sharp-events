using System;
using System.Threading.Tasks;

namespace SharpEvents
{
    public delegate void SharpEventDispatcherDel(Action action);
    
    public delegate Task SharpEventDispatcherAsyncDel(Func<Task> action);
}
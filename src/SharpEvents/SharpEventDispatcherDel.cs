using System;
using System.Threading.Tasks;

namespace SharpEvents
{
    public delegate void SharpEventDispatcherDel(Action action);
    
    public delegate Task SharpEventDispatcherAsyncDel(Action action);
}
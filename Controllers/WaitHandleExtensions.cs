
using System.Threading;
using System.Threading.Tasks;

namespace AnimaApi.Controllers
{

    public static class WaitHandleExtensions
    {
        public static Task<bool> WaitOneAsync(this WaitHandle waitHandle)
        {
            var tcs = new TaskCompletionSource<bool>();

            RegisteredWaitHandle registeredWaitHandle = null;
            registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                (state, timedOut) =>
                {
                    tcs.TrySetResult(!timedOut);
                    registeredWaitHandle.Unregister(null);
                },
                null,
                Timeout.Infinite,
                true
            );

            return tcs.Task;
        }
    }

}

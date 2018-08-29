using System;
using System.Threading;

namespace SmallReport.Tool
{
    public class LockHelper: IDisposable
    {
        private const int DefaultMillisecondsTimeout = 10000;

        private object _obj;

        public bool IsTimeout => _obj == null;

        public LockHelper(object obj)
        {
            TryGet(obj, DefaultMillisecondsTimeout);
        }

        public LockHelper(object obj, int millisecondsTimeout)
        {
            TryGet(obj, millisecondsTimeout);
        }

        private void TryGet(object obj, int millisecondsTimeout)
        {
            if (Monitor.TryEnter(obj, millisecondsTimeout))
            {
                _obj = obj;
            }
        }

        public void Dispose()
        {
            if (_obj != null)
            {
                Monitor.Exit(_obj);
            }
        }
    }
}

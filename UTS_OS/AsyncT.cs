using System;
using System.Threading;

namespace UTS_OS
{
    public class AsyncT : Thread
    {
        public AsyncT()
        {
        }

        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

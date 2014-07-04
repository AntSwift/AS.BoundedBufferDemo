using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AS.BoundedBufferDemo.ConsoleUI
{
    public class BoundedBuffer<T>
    {
        private Queue<T> queue = new Queue<T>();
        private int consumersWaiting;
        private int producersWaiting;
        private const int maxBufferSize = 128;
        private AutoResetEvent are = new AutoResetEvent(false);

        public int Count
        {
            get { return queue.Count; }
        }

        public void Add(T obj)
        {
            Monitor.Enter(queue);
            try
            {
                while (queue.Count == (maxBufferSize - 1))
                {
                    producersWaiting++;
                    Monitor.Exit(queue);
                    are.WaitOne();
                    Monitor.Enter(queue);
                    producersWaiting--;
                }
                queue.Enqueue(obj);
                if (consumersWaiting > 0)
                    are.Set();
            }
            finally
            {
                Monitor.Exit(queue);
            }
        }

        public T Take()
        {
            T item;
            Monitor.Enter(queue);
            try
            {
                while (queue.Count == 0)
                {
                    consumersWaiting++;
                    Monitor.Exit(queue);
                    are.WaitOne();
                    Monitor.Enter(queue);
                    consumersWaiting--;
                }
                item = queue.Dequeue();
                if (producersWaiting > 0)
                    are.Set();
            }
            finally
            {
                Monitor.Exit(queue);
            }
            return item;
        }

        public bool TryTake(out T item)
        {
            item = default(T);

            Monitor.Enter(queue);
            try
            {
                if (queue.Count == 0) return false;

                item = Take();
            }
            finally
            {
                Monitor.Exit(queue);
            }
            return true;
        }
    }
}

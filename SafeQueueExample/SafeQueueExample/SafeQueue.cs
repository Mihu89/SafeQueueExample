using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SafeQueueExample
{
    class SafeQueue<T>
    {
        // protected queue by Monitor
        private Queue<T> _myInputQueue = new Queue<T>();

        // add object to the queue and lock it
        public void Enqueue(T qValue)
        {
            Monitor.Enter(_myInputQueue);

            try
            {
                _myInputQueue.Enqueue(qValue);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
            finally
            {
                //Release the lock
                Monitor.Exit(_myInputQueue);
            }
        }

        // add element to the queue if lock is available
        public bool TryEnqueue(T qValue)
        {
            if (Monitor.TryEnter(_myInputQueue))
            {
                try
                {
                    _myInputQueue.Enqueue(qValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception " + e.Message );
                }
                finally
                {
                    Monitor.Exit(_myInputQueue);
                }
                return true;
            }
            return false;
        }

        // try to add object in the queue after delay
        public bool TryEnqueue(T qValue, int waitTime)
        {
            if (Monitor.TryEnter(_myInputQueue, waitTime))
            {
                try
                {
                    _myInputQueue.Enqueue(qValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                }
                finally
                {
                    Monitor.Exit(_myInputQueue);
                }
                return true;
            }
            return false;
        }

        // Lock the queue and dequeue an element
        public T Dequeue()
        {
            T returnedValue = default(T);

            Monitor.Enter(_myInputQueue);

            try
            {
                returnedValue = _myInputQueue.Dequeue();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
            finally
            {
                Monitor.Exit(_myInputQueue);
            }
            return returnedValue;
        }

        // remove all elements that are equal with inputObject
        public int Remove(T qValue)
        {
            int removeCounter = 0;
            Monitor.Enter(_myInputQueue);
            try
            {
                int queueCounter = _myInputQueue.Count;
                while (queueCounter > 0)
                {
                    T element = _myInputQueue.Dequeue();
                    if (element.Equals(qValue))
                    {
                        removeCounter++;
                    }
                    else
                    {
                        _myInputQueue.Enqueue(element);
                    }
                    queueCounter--;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
            finally
            {
                Monitor.Exit(_myInputQueue);
            }
            return removeCounter;
        }

        // Print all queue
        public string PrintAllElements()
        {
            StringBuilder result = new StringBuilder();
            Monitor.Enter(_myInputQueue);

            try
            {
                foreach (T item in _myInputQueue)
                {
                    result.AppendLine(item.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
            finally
            {
                Monitor.Exit(_myInputQueue);
            }

            return result.ToString();
        }
    }
}

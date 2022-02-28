using System;
using System.Text;
using System.Threading;

namespace SafeQueueExample
{
    class Program
    {
        // Used for safe queue
        private static SafeQueue<int> myQueue = new SafeQueue<int>();
        private static int runningThreads = 0;
        private static int[][] results = new int[10][];

        // Used for Mutex example

        private static Mutex mutex = new Mutex();
        private static int iterationCounter = 1;
        private static int threadCounter = 30;

        static void Main(string[] args)
        {
            //UseSafeQueue();
            UseMutex();
        }

        private static void UseMutex()
        {
            for (int i = 0; i < threadCounter; i++)
            {
                Thread thread = new Thread(new ThreadStart(ThreadMethodTwo));
                //thread.Name = string.Format("Thread{0}", i + 1);
                thread.Name = $"Thread{i + 1}";
                thread.Start();
            }
        }

        private static void ThreadMethodTwo()
        {
            for (int i = 0; i < iterationCounter; i++)
            {
                UseResource();
            }
        }

        private static void UseResource()
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} is requesting Mutex");
            mutex.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name} has entered in the protected area");
            // some code
            Thread.Sleep(800);
            Console.WriteLine($"{Thread.CurrentThread.Name} has leaving the protected area");

            mutex.ReleaseMutex();
            Console.WriteLine($"{Thread.CurrentThread.Name} has released mutex");
        }

        private static void UseSafeQueue()
        {
            Console.WriteLine("Start!");

            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(ThreadMethod);
                thread.Start(i);
                Interlocked.Increment(ref runningThreads);
            }
        }

        private static void ThreadMethod(object state)
        {
            DateTime finish = DateTime.Now.AddSeconds(10);
            Random random = new Random();
            int threadNr = (int)state;
            int[] result = new int[10]; // {0,0,0,0,0,0,0,0,0,0}

            while (DateTime.Now < finish)
            {
                int numberOne = random.Next(500);
                int numberTwo = random.Next(180);

                if (numberTwo < 20)
                {
                    myQueue.Enqueue(numberOne);
                    result[(int)ThreadResultIndex.EnqueueCounter] += 1;
                }
                else if (numberTwo < 40)
                {
                    if (myQueue.TryEnqueue(numberOne))
                    {
                        result[(int)ThreadResultIndex.TryEnqueueSuccessCounter] += 1;
                    }
                    else
                    {
                        result[(int)ThreadResultIndex.TryEnqueueFailCounter] += 1;
                    }
                }
                else if (numberTwo < 60)
                {
                    if (myQueue.TryEnqueue(numberOne, 10))
                    {
                        result[(int)ThreadResultIndex.TryEnqueueWaitSuccessCounter] += 1;
                    }
                    else
                    {
                        result[(int)ThreadResultIndex.TryEnqueueWaitFailCounter] += 1;
                    }
                }
                else if (numberTwo < 80)
                {
                    result[(int)ThreadResultIndex.DequeueSuccessCounter] += 1;
                    try
                    {
                        myQueue.Dequeue();
                    }
                    catch (Exception)
                    {
                        result[(int)ThreadResultIndex.DequeueFailCounter] += 1;
                    }
                }
                else
                {
                    result[(int)ThreadResultIndex.RemoveCounter] += 1;
                    result[(int)ThreadResultIndex.RemovedCounter] += myQueue.Remove(numberOne);
                }

                results[threadNr] = result;

                if (Interlocked.Decrement(ref runningThreads) == 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < 10; i++)
                    {
                        stringBuilder.Append("Thread " + i + "  ");
                    }

                    stringBuilder.AppendLine();
                    for (int i = 0; i < 9; i++) // methods numbers
                    {
                        int total = 0;
                        stringBuilder.Append(Titles[i]);
                        for (int j = 0; j < 10; j++) // threads numbers
                        {
                            stringBuilder.Append(string.Format("{0,9}", results[j][i]));
                            //stringBuilder.Append($"{{results[j][i]}}");
                            total += results[j][i];
                        }

                        //stringBuilder.AppendLine(total.ToString());
                        stringBuilder.AppendLine(string.Format("{0,9}", total)); // operations
                    }
                    Console.WriteLine(stringBuilder.ToString());
                }

            }

        }
        private static string[] Titles =
        {
        "Enqueue               ",
        "TryEnqueue Success    ",
        "TryEnqueue Fail       ",
        "TryEnqueue WaitSuccess",
        "TryEnqueue WaitFail   ",
        "Dequeue Success       ",
        "Dequeue Fail          ",
        "Remove element        ",
        "Removed elements      "
    };
        private enum ThreadResultIndex
        {
            EnqueueCounter,
            TryEnqueueSuccessCounter,
            TryEnqueueFailCounter,
            TryEnqueueWaitSuccessCounter,
            TryEnqueueWaitFailCounter,
            DequeueSuccessCounter,
            DequeueFailCounter,
            RemoveCounter,
            RemovedCounter
        }
    }
}

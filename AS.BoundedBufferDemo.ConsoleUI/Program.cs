using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace AS.BoundedBufferDemo.ConsoleUI
{
    class Program
    {
        static System.Threading.Timer syncTimer;

        const int TOTAL_PULSES = 5000;
        const int MAX_BUFFER_SIZE = 10;

        static void Main(string[] args)
        {
            // Main thread here (or some Consumer thread).
            var buffer = new BoundedBuffer<Pulse>();

            syncTimer = new Timer(new TimerCallback(TimerElapsed), buffer, 0, 5000);

            // Producer worker. Could be a thread in another class.
            new Thread(() =>
            {
                for (int i = 0; i < TOTAL_PULSES; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        var pulse = new Pulse() { PortId = 1, Timestamp = DateTime.Now };
                        buffer.Add(pulse);
                        Console.WriteLine("Generated: {0} ({1}/{2})", pulse, i + 1, TOTAL_PULSES);
                        i++;
                    }
                    Thread.Sleep(10000);
                }
                buffer.Add(null); // End.
            }).Start();

            Thread.Sleep(Timeout.Infinite);
        }

        static void TimerElapsed(object state)
        {
            var primaryBuffer = (BoundedBuffer<Pulse>)state;
            var localBuffer = new List<Pulse>(MAX_BUFFER_SIZE);


            Pulse pulse = null;
            while (primaryBuffer.TryTake(out pulse))
            {
                localBuffer.Add(pulse);

                if (localBuffer.Count == MAX_BUFFER_SIZE)
                {
                    break;
                }
            }

            ProcessItems(localBuffer);
            localBuffer.Clear();
        }

        static void ProcessItems(List<Pulse> buffer)
        {
            // TODO: Process items accordingly.
            Console.WriteLine("Worker: Processing {0} pulses", buffer.Count);
        }
    }
}
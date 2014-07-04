using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AS.BoundedBufferDemo.ConsoleUI
{
    class Pulse
    {
        public int PortId { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return string.Format("Port: {0} Time: {1}", PortId, Timestamp.Ticks);
        }
    }
}

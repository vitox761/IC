using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingWithXAML.Classes
{
    public class ReceivedDataEventArgs
    {
        private Packet packet;

        public ReceivedDataEventArgs(Packet _packet)
        {
            this.packet = _packet;
        }

        public Packet Packet
        {
            get { return packet; }
        }
    }
}

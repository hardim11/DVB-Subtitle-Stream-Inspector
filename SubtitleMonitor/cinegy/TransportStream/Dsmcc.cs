using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McastCinergy.cinergy.TransportStream
{
    public class Dsmcc
    {
        public short Pid { get; set; }
        public byte PointerField { get; set; }
        //public byte TableId { get; set; }
        public short SectionLength { get; set; }
        //public List<Descriptor> Descriptors { get; set; }
    }
}

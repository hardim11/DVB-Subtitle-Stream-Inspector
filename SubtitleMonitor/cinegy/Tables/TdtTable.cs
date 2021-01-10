using Cinegy.TsDecoder.Tables;
using Cinegy.TsDecoder.TransportStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McastCinergy.cinergy.TransportStream
{
    public class TdtTable : Table
    {

        public DateTime UtcTime { get; set; }
        // public byte[] _data { get; set; }

    }
}

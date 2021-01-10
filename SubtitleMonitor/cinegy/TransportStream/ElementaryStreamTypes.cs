using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McastCinergy.cinergy.TransportStream
{
    public static class ElementaryStreamTypes
    {
        public static readonly Dictionary<int, string> ElementaryStreamTypeDescriptions = new Dictionary<int, string>()
        {
            {0x00, "ITU-T | ISO/IEC Reserved"},
            {0x01, "ISO/IEC 11172 Video"},
            {0x02, "ITU-T Rec. H.262 | ISO/IEC 13818-2 Video or ISO/IEC 11172-2 constrained parameter video stream"},
            {0x03, "ISO/IEC 11172 Audio"},
            {0x04, "ISO/IEC 13818-3 Audio"},
            {0x05, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 private_sections"},
            {0x06, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 PES packets containing private data"},
            {0x07, "ISO/IEC 13522 MHEG"},
            {0x08, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Annex A DSM-CC"},
            {0x09, "ITU-T Rec. H.222.1"},
            {0x0A, "ISO/IEC 13818-6 type A"},
            {0x0B, "ISO/IEC 13818-6 type B"},
            {0x0C, "ISO/IEC 13818-6 type C"},
            {0x0D, "ISO/IEC 13818-6 type D"},
            {0x0E, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 auxiliary"},
            {0x0F, "ISO/IEC 13818-7 Audio with ADTS transport syntax"},
            {0x10, "ISO/IEC 14496-2 Visual"},
            {0x11, "ISO/IEC 14496-3 Audio with the LATM transport syntax as defined in ISO/IEC 14496-3 / AMD 1"},
            {0x12, "ISO/IEC 14496-1 SL-packetized stream or FlexMux stream carried in PES packets"},
            {0x13, "ISO/IEC 14496-1 SL-packetized stream or FlexMux stream carried in ISO/IEC14496_sections."},
            {0x14, "ISO/IEC 13818-6 Synchronized Download Protocol"}
//            {0x15-0x7F, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"},
//            {0x80-0xFF, "User Private"},
        };

        public static string GetStreamTypeFromId(byte StreamTypeId)
        {
            if (ElementaryStreamTypes.ElementaryStreamTypeDescriptions.Keys.Contains(StreamTypeId))
            {
                return ElementaryStreamTypes.ElementaryStreamTypeDescriptions[StreamTypeId];
            }

            if ((StreamTypeId >= 0x15) && (StreamTypeId <= 0x7F))
            {
                return "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved";
            }

            if ((StreamTypeId >= 0x80) && (StreamTypeId <= 0xFF))
            {
                return "User Private";
            }

            return "Unknown";

        }
    }
}

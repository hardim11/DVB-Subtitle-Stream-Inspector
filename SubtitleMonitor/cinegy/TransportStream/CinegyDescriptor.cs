using System.Text;

namespace Cinegy.TsDecoder.TransportStream
{
    public class CinegyDaniel2Descriptor //if format-identifier / organization tag == 2LND
    {

        public CinegyDaniel2Descriptor(byte[] stream, int start) 
        {
            //TODO:Set correct length check
//            if (!(AdditionalIdentificationInfo.Length < 30 )) return;

            FourCc = Encoding.ASCII.GetString(stream, start + 2, 4);
            Version = stream[start + 6];
            HdrSize = stream[start + 7];
            Flags = Utils.Convert2BytesToUshort(stream,start+8);
            Width = Utils.Convert4BytesToUint(stream, start + 10);
            Height = Utils.Convert4BytesToUint(stream, start + 14);
            FrameRate = Utils.Convert4BytesToUint(stream, start + 18);
            AspectRatio = Utils.Convert4BytesToUint(stream, start + 22);
            MainQuantizer = Utils.Convert4BytesToUint(stream, start + 26); //this is very wrong, i cannot just jam a float in here like this :-)

            //todo: fill in the rest of all this
            ChromaQuantizerAdd = 0;// 1 percents. ChromaQuantizer = MainQuantizer * (100 + ChromaQuantizerAdd) / 100
            AlphaQuantizerAdd = 0;// 1 percents. AlphaQuantizer = MainQuantizer * (100 + AlphaQuantizerAdd ) / 100
            Orientation = 0; // 1 see ORIENTATION
            InterlaceType = 0; // 1 see INTERLACE_TYPE
            ChromaFormat = 0; // 1 see CHROMA_FORMAT
            BitDepth = 0;       // 1 Main Bitdepth                   
            VideoFormat = 0;     // 1
            ColorPrimaries = 0;    // 1
            TransferCharacteristics = 0;// 1
            MatrixCoefficients = 0;  // 1
            MaxFrameSize = 0; // 4 if 0 - unspecified (VBR), if != 0 - specifies max public byte rate and CBR
            EncodeMethod = 0; // 1
            FrameType = 0; // 1 0 - total Intra, 1 - can have Delta-blocks, 2 - total Delta
            TempRef = 0;       // 1
            TimeCode = 0;       // 4 
            FrameSize = 0; // 4 12 Coded frame size (in bytes), including header
            NumExtraRecords = 0;// 1
            ExtraRecordsSize = 0;// 2
        }

        public string LongName {get{return "Cinegy Daniel2 Coded Video";}}

        public string Name {get{ return "Daniel2 Video";}}

        public string FourCc { get; set; }       // 4 'D''N''L''2'                                      

        public byte Version { get; set;  }        // 1 Coded codec version                               
        public byte HdrSize { get; set; }        // 1 size of header, not including extra records
        public ushort Flags { get; set; }        // 2 see D2_FLAGS

        public uint Width { get; set; }          // 4 Frame width                                       
        public uint Height { get; set; }         // 4 Frame height                                      

        public uint FrameRate { get; set; }      // 4 bits 0..19 are numerator, bits 20..31 are denominator (assuming 0 == 1 for denom).
        public uint AspectRatio { get; set; }    // 4 bits 0..15 are numerator, bits 16..31 are denominator

        public float MainQuantizer { get; set; } // 4 Main Quantizer
        public byte ChromaQuantizerAdd { get; set; } // 1 percents. ChromaQuantizer = MainQuantizer * (100 + ChromaQuantizerAdd) / 100
        public byte AlphaQuantizerAdd { get; set; } // 1 percents. AlphaQuantizer  = MainQuantizer * (100 + AlphaQuantizerAdd ) / 100

        public byte Orientation { get; set; } // 1 see ORIENTATION
        public byte InterlaceType { get; set; } // 1 see INTERLACE_TYPE

        public byte ChromaFormat { get; set; } // 1 see CHROMA_FORMAT
        public byte BitDepth { get; set; } // 1 Main Bitdepth                                     

        public byte VideoFormat { get; set; }
        public byte ColorPrimaries { get; set; }
        public byte TransferCharacteristics { get; set; }
        public byte MatrixCoefficients { get; set; }

        public uint MaxFrameSize { get; set; } // 4 if 0 - unspecified (VBR), if != 0 - specifies max public byte rate and CBR

        // frame parameters
        public byte EncodeMethod { get; set; }   // 1
        public byte FrameType { get; set; }      // 1 0 - total Intra, 1 - can have Delta-blocks, 2 - total Delta
        public byte TempRef { get; set; }        // 1

        public uint TimeCode { get; set; }       // 4 
        public uint FrameSize { get; set; }      // 4 12 Coded frame size (in bytes), including header

        public byte NumExtraRecords { get; set; } // 1
        public ushort ExtraRecordsSize { get; set; } // 2

        //TODO: work out what to do with this - probably just delete...
       // public byte __dummy[8]{ get; };     // 

    }

    public class CinegyTechMetadataDescriptor : RegistrationDescriptor //if format-identifier / organization tag == CNGY
    {
        public CinegyTechMetadataDescriptor(byte[] stream, int start) : base(stream, start)
        {
            if (!(AdditionalIdentificationInfo.Length >= 2)) return;

            //todo: set to extracted values
            CinecoderVersion = "3.28.22";
            MLVersion = "6.7.1";
            AppVersion = "12.0.3.2112";
            AppName = "PlayoutExApp";
        }

        public string CinecoderVersion { get; set; }
        public string MLVersion { get; set; }
        public string AppVersion { get; set; }
        public string AppName { get; set; }
    }

}

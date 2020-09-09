namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class RegionCompositionSegmentObject
    {
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public int ObjectProviderFlag { get; set; }
        public int ObjectHorizontalPosition { get; set; }
        public int ObjectVerticalPosition { get; set; }
        public int? ObjectForegroundPixelCode { get; set; }
        public int? ObjectBackgroundPixelCode { get; set; }


        public string ObjectTypeString
        {
            get
            {
                switch (this.ObjectType)
                {
                    case 0:
                        return "basic_object, bitmap";
                    case 1:
                        return "basic_object, character";
                    case 2:
                        return "composite_object, string of characters";
                    default:
                        return "reserved or unknown";
                }
            }
        }

        public string ObjectProviderFlagString
        {
            get
            {
                switch (ObjectProviderFlag)
                {
                    case 0:
                        return "provided in the subtitling stream";
                    case 1:
                        return "provided by a ROM in the IRD";
                    default:
                        return "reserved or unknown";
                }
            }
        }
    }
}

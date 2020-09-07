using System.Collections.Generic;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class RegionCompositionSegment
    {
        public int RegionId { get; set; }
        public int RegionVersionNumber { get; set; }
        public bool RegionFillFlag { get; set; }
        public int RegionWidth { get; set; }
        public int RegionHeight { get; set; }
        public int RegionLevelOfCompatibility { get; set; }
        public int RegionDepth { get; set; }
        public int RegionClutId { get; set; }
        public int Region8BitPixelCode { get; set; }
        public int Region4BitPixelCode { get; set; }
        public int Region2BitPixelCode { get; set; }

        public List<RegionCompositionSegmentObject> Objects = new List<RegionCompositionSegmentObject>();

        public RegionCompositionSegment(byte[] buffer, int index, int regionLength)
        {
            RegionId = buffer[index];
            RegionVersionNumber = buffer[index + 1] >> 4;
            RegionFillFlag = (buffer[index + 1] & 0b00001000) > 0;
            RegionWidth = Helper.GetEndianWord(buffer, index + 2);
            RegionHeight = Helper.GetEndianWord(buffer, index + 4);
            RegionLevelOfCompatibility = buffer[index + 6] >> 5;
            RegionDepth = (buffer[index + 6] & 0b00011100) >> 2;
            RegionClutId = buffer[index + 7];
            Region8BitPixelCode = buffer[index + 8];
            Region4BitPixelCode = buffer[index + 9] >> 4;
            Region2BitPixelCode = (buffer[index + 9] & 0b00001100) >> 2;
            int i = 0;
            while (i < regionLength && i + index < buffer.Length)
            {
                var regionCompositionSegmentObject = new RegionCompositionSegmentObject { ObjectId = Helper.GetEndianWord(buffer, i + index + 10) };
                i += 2;
                regionCompositionSegmentObject.ObjectType = buffer[i + index + 10] >> 6;
                regionCompositionSegmentObject.ObjectProviderFlag = (buffer[index + i + 10] & 0b00110000) >> 4;
                regionCompositionSegmentObject.ObjectHorizontalPosition = ((buffer[index + i + 10] & 0b00001111) << 8) + buffer[index + i + 11];
                i += 2;
                regionCompositionSegmentObject.ObjectVerticalPosition = ((buffer[index + i + 10] & 0b00001111) << 8) + buffer[index + i + 11];
                i += 2;
                if (regionCompositionSegmentObject.ObjectType == 1 || regionCompositionSegmentObject.ObjectType == 2)
                {
                    i += 2;
                }

                Objects.Add(regionCompositionSegmentObject);
            }
        }

        public string ToHtml()
        {
            string res = "<table>\r\n";

            res += "<tr><td>RegionId:</td<td>" + RegionId.ToString() + "</td></tr>\r\n";

            //res += "<tr><td>RegionId:</td<td>" + RegionId.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionVersionNumber:</td<td>" + RegionVersionNumber.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionFillFlag:</td<td>" + RegionFillFlag.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionWidth:</td<td>" + RegionWidth.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionHeight:</td<td>" + RegionHeight.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionLevelOfCompatibility:</td<td>" + RegionLevelOfCompatibility.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionDepth:</td<td>" + RegionDepth.ToString() + "</td></tr>\r\n";
            res += "<tr><td>RegionClutId:</td<td>" + RegionClutId.ToString() + "</td></tr>\r\n";
            res += "<tr><td>Region8BitPixelCode:</td<td>" + Region8BitPixelCode.ToString() + "</td></tr>\r\n";
            res += "<tr><td>Region4BitPixelCode:</td<td>" + Region4BitPixelCode.ToString() + "</td></tr>\r\n";
            res += "<tr><td>Region2BitPixelCode:</td<td>" + Region2BitPixelCode.ToString() + "</td></tr>\r\n";

            if (this.Objects.Count < 1)
            {
                res += "<tr><td colspan=\"2\"><b>No Objects</b></td></tr>\r\n";
            }
            else
            {
                res += "<tr><td colspan=\"2\"><b>Objects:</b></td></tr>\r\n";

                foreach (RegionCompositionSegmentObject item in this.Objects)
                {
                    res += "<tr><td>&nbsp;</td><td style=\"border: 1px solid black;\"><table>";
                    res += "<tr><td>ObjectId</td><td>" + item.ObjectId.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>ObjectType</td><td>" + item.ObjectType.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>ObjectProviderFlag</td><td>" + item.ObjectProviderFlag.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>ObjectHorizontalPosition</td><td>" + item.ObjectHorizontalPosition.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>ObjectVerticalPosition</td><td>" + item.ObjectVerticalPosition.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>ObjectForegroundPixelCode</td><td>" + item.ObjectForegroundPixelCode.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>ObjectBackgroundPixelCode</td><td>" + item.ObjectBackgroundPixelCode.ToString() + "</td></tr>\r\n";
                    res += "</table></td></tr>";
                }
            }
            res += "</table>\r\n";

            return res;
        }
    }
}

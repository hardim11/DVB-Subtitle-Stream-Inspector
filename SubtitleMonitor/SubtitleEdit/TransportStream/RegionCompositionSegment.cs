using SubtitleMonitor;
using System.Collections.Generic;
using System.Windows.Forms;

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

        public string RegionLevelOfCompatibilityString
        {
            get
            {

                switch (this.RegionLevelOfCompatibility)
                {
                    case 0:
                        return "reserved";
                    case 1:
                        return "2-bit/entry CLUT required";
                    case 2:
                        return "4-bit/entry CLUT required";
                    case 3:
                        return "8-bit/entry CLUT required";
                    default:
                        return "reserved or unknown";
                }

            }
        }

        public string RegionDepthString
        {
            get
            {
                switch (this.RegionDepth)
                {
                    case 0:
                        return "reserved";
                    case 1:
                        return "2 bit";
                    case 2:
                        return "4 bit";
                    case 3:
                        return "8 bit";
                    default:
                        return "reserved or unknown";
                }
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

        public void PopulateListViewDetails(ListView Lv)
        {
            Lv.Items.Clear();
            ListViewGroup grpGeneral = Lv.Groups.Add("General", "General");

            Utils.AddListViewEntry(
                Lv,
                "region_id",
                RegionId.ToString(),
                "This 8-bit field uniquely identifies the region for which information is contained in this region_composition_segment.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_version_number",
                RegionVersionNumber.ToString(),
                "This indicates the version of this region. The version number is incremented (modulo 16)",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_version_number",
                RegionFillFlag.ToString(),
                "If set to '1', signals that the region is to be filled with the background colour defined in the region_n - bit_pixel_code fields in this segment.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_width:",
                RegionWidth.ToString(),
                "Specifies the horizontal length of this region, expressed in number of pixels. For subtitle services which do not include a display definition segment, the value in this field shall be within the range 1 to 720, and the sum of the region_width and the region_horizontal_address(see clause 7.2.1) shall not exceed 720.For subtitle services which include a display definition segment, the value of this field shall be within the range 1 to(display_width + 1) and shall not exceed the value of(display_width + 1) as signalled in the relevant DDS.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_height:",
                RegionHeight.ToString(),
                "Specifies the vertical length of the region, expressed in number of pixels. For subtitle services which do not include a display definition segment, the value in this field shall be within the inclusive range 1 to 576, and the sum of the region_height and the region_vertical_address(see clause 7.2.1) shall not exceed 576.For subtitle services which include a display definition segment, the value of this field shall be within the range 1 to(display_height + 1) and shall not exceed the value of(display_height + 1) as signalled in the relevant DDS.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_level_of_compatibility:",
                RegionLevelOfCompatibility.ToString() + "(" + this.RegionLevelOfCompatibilityString + ")",
                "This indicates the minimum type of CLUT that is necessary in the decoder to decode this region",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_depth",
                this.RegionDepth.ToString() + " (" + RegionDepthString + ")",
                "Identifies the intended pixel depth for this region",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "CLUT_id",
                this.RegionClutId.ToString(),
                "Identifies the family of CLUTs that applies to this region.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "region_8-bit_pixel-code",
                this.Region2BitPixelCode.ToString(),
                " Specifies the entry of the applied 8-bit CLUT as background colour for the region when the region_fill_flag is set, but only if the region depth is 8 bit.The value of this field is undefined if a region depth of 2 or 4 bit applies.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                 Lv,
                 "region_4-bit_pixel-code",
                 this.Region4BitPixelCode.ToString(),
                 "Specifies the entry of the applied 4-bit CLUT as background colour for the region when the region_fill_flag is set, if the region depth is 4 bit, or if the region depth is 8 bit while the region_level_of_compatibility specifies that a 4 - bit CLUT is within the minimum requirements. In any other case the value of this field is undefined.",
                 grpGeneral
             );

            Utils.AddListViewEntry(
                 Lv,
                 "region_2-bit_pixel-code",
                 this.Region2BitPixelCode.ToString(),
                 "Specifies the entry of the applied 2-bit CLUT as background colour for the region when the region_fill_flag is set, if the region depth is 2 bit, or if the region depth is 4 or 8 bit while the region_level_of_compatibility specifies that a 2 - bit CLUT is within the minimum requirements. In any other case the   value of this field is undefined", 
                 grpGeneral
             );

            Utils.AddListViewEntry(
                Lv,
                "number of objects:",
                this.Objects.Count.ToString(),
                "",
                grpGeneral
            );




            if (this.Objects.Count > 0)
            {
                foreach (RegionCompositionSegmentObject item in this.Objects)
                {
                    ListViewGroup grpRegion = Lv.Groups.Add("Object" + item.ObjectId.ToString(), "Object ID " + item.ObjectId.ToString());

                    Utils.AddListViewEntry(
                        Lv,
                        "object_id",
                        item.ObjectId.ToString(),
                        "Identifies an object that is shown in the region.",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "object_type",
                        item.ObjectType.ToString() + " (" + item.ObjectTypeString + ")",
                        " Identifies the type of object ",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "object_provider_flag",
                        item.ObjectProviderFlag.ToString() + " (" + item.ObjectProviderFlagString + ")",
                        "A 2-bit flag indicating how this object is provided",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "object_horizontal_position",
                        item.ObjectHorizontalPosition.ToString(),
                        "Specifies the horizontal position of the top left pixel of this object, expressed in number of horizontal pixels, relative to the left - hand edge of the associated region.The specified horizontal position shall be within the region, hence its value shall be in the range between 0 and(region_width - 1). ",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "object_vertical_position",
                        item.ObjectVerticalPosition.ToString(),
                        "Specifies the vertical position of the top left pixel of this object, expressed in number of lines, relative to the top of the associated region.The specified vertical position shall be within the region, hence its value shall be in the range between 0 and(region_height - 1).",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "foreground_pixel_code",
                        item.ObjectForegroundPixelCode.ToString(),
                        "Specifies the entry in the applied 8-bit CLUT that has been selected as the foreground colour of the character(s).",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "background_pixel_code",
                        item.ObjectBackgroundPixelCode.ToString(),
                        " Specifies the entry in the applied 8-bit CLUT that has been selected as the background colour of the character(s).",
                        grpRegion
                    );

                }
            }
        }
    }
}

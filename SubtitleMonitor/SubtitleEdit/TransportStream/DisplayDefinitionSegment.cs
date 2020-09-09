using SubtitleMonitor;
using System.Windows.Forms;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class DisplayDefinitionSegment
    {
        public int DisplayDefinitionVersionNumber { get; set; }
        public bool DisplayWindowFlag { get; set; }
        public int DisplayWidth { get; set; }
        public int DisplayHeight { get; set; }
        public int? DisplayWindowHorizontalPositionMinimum { get; set; }
        public int? DisplayWindowHorizontalPositionMaximum { get; set; }
        public int? DisplayWindowVerticalPositionMinimum { get; set; }
        public int? DisplayWindowVerticalPositionMaximum { get; set; }

        public DisplayDefinitionSegment(byte[] buffer, int index)
        {
            DisplayDefinitionVersionNumber = buffer[index] >> 4;
            DisplayWindowFlag = (buffer[index] & 0b00001000) > 0;
            DisplayWidth = Helper.GetEndianWord(buffer, index + 1);
            DisplayHeight = Helper.GetEndianWord(buffer, index + 3);
            if (DisplayWindowFlag)
            {
                DisplayWindowHorizontalPositionMinimum = Helper.GetEndianWord(buffer, index + 5);
                DisplayWindowHorizontalPositionMaximum = Helper.GetEndianWord(buffer, index + 7);
                DisplayWindowVerticalPositionMinimum = Helper.GetEndianWord(buffer, index + 9);
                DisplayWindowVerticalPositionMaximum = Helper.GetEndianWord(buffer, index + 11);
            }
        }

        public string ToHtml()
        {
            string res = "<table>\r\n";

            res += "<tr><td>Version:</td<td>" + DisplayDefinitionVersionNumber.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowFlag:</td><td>" + DisplayWindowFlag.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWith:</td><td>" + DisplayWidth.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayHeight:</td><td>" + DisplayHeight.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowHorizontalPositionMinimum:</td><td>" + DisplayWindowHorizontalPositionMinimum.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowHorizontalPositionMaximum:</td><td>" + DisplayWindowHorizontalPositionMaximum.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowVerticalPositionMinimum:</td><td>" + DisplayWindowVerticalPositionMinimum.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowVerticalPositionMaximum:</td><td>" + DisplayWindowVerticalPositionMaximum.ToString() + "</td></tr>\r\n";

            res += "</table>\r\n";

            return res;
        }

        public void PopulateListViewDetails(ListView Lv)
        {
            Lv.Items.Clear();
            ListViewGroup grpGeneral = Lv.Groups.Add("General", "General");

            Utils.AddListViewEntry(
                Lv,
                "dds_version_number",
                this.DisplayDefinitionVersionNumber.ToString(),
                "The version of this display definition segment. When any of the contents of this display definition segment change, this version number is incremented(modulo 16).",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_window_flag",
                this.DisplayWindowFlag.ToString(),
                "If display_window_flag = 1, the DVB subtitle display set associated with this display definition segment is intended to be rendered in a window within the display resolution defined by display_width and display_height.The size and position of this window within the display is defined by the parameters signalled in this display definition segment as display_window_horizontal_position_minimum, display_window_horizontal_position_maximum, display_window_vertical_position_minimum and display_window_vertical_position_maximum. If display_window_flag = 0, the DVB subtitle display set associated with this display_definition_segment is intended to be rendered directly within the display resolution defined by display_width and display_height.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_width",
                this.DisplayWidth.ToString(),
                "Specifies the maximum horizontal width of the display in pixels minus 1 assumed by the subtitling stream associated with this display definition segment.The value in this field shall be in the region 0..4095",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_height",
                this.DisplayHeight.ToString(),
                " Specifies the maximum vertical height of the display in lines minus 1 assumed by the subtitling stream associated with this display definition segment.The value in this field shall be in the region 0..4095.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_window_horizontal_position_minimum",
                this.DisplayWindowHorizontalPositionMinimum.ToString(),
                "Specifies the left-hand most pixel of this DVB subtitle display set with reference to the left - hand most pixel of the display.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_window_horizontal_position_maximum",
                this.DisplayWindowHorizontalPositionMaximum.ToString(),
                "Specifies the right-hand most pixel of this DVB subtitle display set with reference to the left - hand most pixel of the display.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_window_vertical_position_minimum",
                this.DisplayWindowVerticalPositionMinimum.ToString(),
                "Specifies the upper most line of this DVB subtitle display set with reference to the top line of the display.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "display_window_vertical_position_maximum",
                this.DisplayWindowVerticalPositionMaximum.ToString(),
                "Specifies the bottom line of this DVB subtitle display set with reference to the top line of the display.",
                grpGeneral
            );
        }
    }
}

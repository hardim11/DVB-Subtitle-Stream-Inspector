namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class DisplayDefinitionSegment
    {
        public int DisplayDefinitionVersionNumber { get; set; }
        public bool DisplayWindowFlag { get; set; }
        public int DisplayWith { get; set; }
        public int DisplayHeight { get; set; }
        public int? DisplayWindowHorizontalPositionMinimum { get; set; }
        public int? DisplayWindowHorizontalPositionMaximum { get; set; }
        public int? DisplayWindowVerticalPositionMinimum { get; set; }
        public int? DisplayWindowVerticalPositionMaximum { get; set; }

        public DisplayDefinitionSegment(byte[] buffer, int index)
        {
            DisplayDefinitionVersionNumber = buffer[index] >> 4;
            DisplayWindowFlag = (buffer[index] & 0b00001000) > 0;
            DisplayWith = Helper.GetEndianWord(buffer, index + 1);
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
            res += "<tr><td>DisplayWith:</td><td>" + DisplayWith.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayHeight:</td><td>" + DisplayHeight.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowHorizontalPositionMinimum:</td><td>" + DisplayWindowHorizontalPositionMinimum.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowHorizontalPositionMaximum:</td><td>" + DisplayWindowHorizontalPositionMaximum.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowVerticalPositionMinimum:</td><td>" + DisplayWindowVerticalPositionMinimum.ToString() + "</td></tr>\r\n";
            res += "<tr><td>DisplayWindowVerticalPositionMaximum:</td><td>" + DisplayWindowVerticalPositionMaximum.ToString() + "</td></tr>\r\n";

            res += "</table>\r\n";

            return res;
        }
    }
}

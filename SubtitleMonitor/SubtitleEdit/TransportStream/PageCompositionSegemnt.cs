using System.Collections.Generic;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class PageCompositionSegment
    {
        public int PageTimeOut { get; set; }
        public int PageVersionNumber { get; set; }
        public int PageState { get; set; }
        public List<PageCompositionSegmentRegion> Regions;


        public string PageStateString
        {
            get
            {
                switch (PageState)
                {
                    case 0:
                        return "Normal Case (page update)";
                    case 1:
                        return "Aquisitiion Point (page refresh)";
                    case 2:
                        return "Mode Change (new page";
                    case 3:
                        return "reserved";
                    default:
                        return "(unknown page state)";
                }
            }
        }


        public PageCompositionSegment(byte[] buffer, int index, int regionLength)
        {
            PageTimeOut = buffer[index];
            PageVersionNumber = buffer[index + 1] >> 4;
            PageState = (buffer[index + 1] & 0b00001100) >> 2;
            Regions = new List<PageCompositionSegmentRegion>();
            int i = 0;
            while (i < regionLength && i + index < buffer.Length)
            {
                var pageCompositionSegmentRegion = new PageCompositionSegmentRegion { RegionId = buffer[i + index + 2] };
                i += 2;
                pageCompositionSegmentRegion.RegionHorizontalAddress = Helper.GetEndianWord(buffer, i + index + 2);
                i += 2;
                pageCompositionSegmentRegion.RegionVerticalAddress = Helper.GetEndianWord(buffer, i + index + 2);
                i += 2;
                Regions.Add(pageCompositionSegmentRegion);
            }
        }

        public string ToHtml()
        {
            string res = "<table>\r\n";

            res += "<tr><td>PageTimeOut:</td<td>" + PageTimeOut.ToString() + "</td></tr>\r\n";
            res += "<tr><td>PageVersionNumber:</td><td>" + PageVersionNumber.ToString() + "</td></tr>\r\n";
            res += "<tr><td>PageState:</td><td>" + PageState.ToString() + " (" + (PageStateString) + ")</td></tr>\r\n";

            if (this.Regions.Count < 1)
            {
                res += "<tr><td colspan=\"2\"><b>No Regions</b></td></tr>\r\n";
            }
            else
            {
                res += "<tr><td colspan=\"2\"><b>Regions:</b></td></tr>\r\n";

                foreach (PageCompositionSegmentRegion item in this.Regions)
                {
                    res += "<tr><td>&nbsp;</td><td style=\"border: 1px solid black;\"><table>";
                    res += "<tr><td>Region ID</td><td>" + item.RegionId.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>region_horizontal_address:</td><td>" + item.RegionHorizontalAddress.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>region_vertical_address:</td><td>" + item.RegionVerticalAddress.ToString() + "</td></tr>\r\n";
                    res += "</table></td></tr>";
                }
            }
            res += "</table>\r\n";

            return res;
        }
    }

}

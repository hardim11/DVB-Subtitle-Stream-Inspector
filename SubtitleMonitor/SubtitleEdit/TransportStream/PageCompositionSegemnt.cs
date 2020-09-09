using SubtitleMonitor;
using System.Collections.Generic;
using System.Windows.Forms;


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

        public void PopulateListViewDetails(ListView Lv)
        {
            Lv.Items.Clear();
            ListViewGroup grpGeneral = Lv.Groups.Add("General", "General");

            Utils.AddListViewEntry(
                Lv,
                "page_time_out",
                PageTimeOut.ToString(),
                "The period, expressed in seconds, after which a page instance is no longer valid and consequently shall be erased from the screen, should it not have been redefined before that",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "page_version_number",
                PageVersionNumber.ToString(),
                "The version of this page composition segment. When any of the contents of this page composition segment change, this version number is incremented(modulo 16). ",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "page_state:",
                PageState.ToString() + " (" + PageStateString + ")",
                "This field signals the status of the subtitling page instance described in this page composition segment",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "number of regions:",
                this.Regions.Count.ToString(),
                "",
                grpGeneral
            );

            if (this.Regions.Count > 0)
            {
                foreach (PageCompositionSegmentRegion item in this.Regions)
                {

                    ListViewGroup grpRegion = Lv.Groups.Add("Region" + item.RegionId.ToString(), "Region ID " + item.RegionId.ToString());

                    Utils.AddListViewEntry(
                        Lv,
                        "region_id",
                        item.RegionId.ToString(),
                        "This uniquely identifies a region within a page. Each identified region is displayed in the page instance defined in this page composition",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "region_horizontal_address",
                        item.RegionHorizontalAddress.ToString(),
                        "This specifies the horizontal address of the top left pixel of this region. The left-most pixel of the active pixels has horizontal address zero, and the pixel address increases from left to right.",
                        grpRegion
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "region_vertical_address",
                        item.RegionVerticalAddress.ToString(),
                        "This specifies the vertical address of the top line of this region. The top line of the frame is line zero, and the line address increases by one within the frame from top to bottom.",
                        grpRegion
                    );

                }
            }
        }
    
    }

}

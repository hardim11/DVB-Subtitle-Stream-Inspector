using SubtitleMonitor;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream
{
    public class ClutDefinitionSegment
    {
        public int ClutId { get; set; }
        public int ClutVersionNumber { get; set; }
        public List<RegionClutSegmentEntry> Entries = new List<RegionClutSegmentEntry>();

        public ClutDefinitionSegment(byte[] buffer, int index, int segmentLength)
        {
            Entries = new List<RegionClutSegmentEntry>();
            ClutId = buffer[index];
            ClutVersionNumber = buffer[index + 1] >> 4;

            int k = index + 2;
            while (k + 6 <= index + segmentLength)
            {
                var rcse = new RegionClutSegmentEntry();
                rcse.ClutEntryId = buffer[k];
                byte flags = buffer[k + 1];

                rcse.ClutEntry2BitClutEntryFlag = (flags & 0b10000000) > 0;
                rcse.ClutEntry4BitClutEntryFlag = (flags & 0b01000000) > 0;
                rcse.ClutEntry8BitClutEntryFlag = (flags & 0b00100000) > 0;
                rcse.FullRangeFlag = (flags & 0b00000001) > 0;

                if (rcse.FullRangeFlag)
                {
                    rcse.ClutEntryY = buffer[k + 2];
                    rcse.ClutEntryCr = buffer[k + 3];
                    rcse.ClutEntryCb = buffer[k + 4];
                    rcse.ClutEntryT = buffer[k + 5];
                    k += 6;
                }
                else
                {
                    rcse.ClutEntryY = buffer[k + 2] >> 2;
                    rcse.ClutEntryCr = ((buffer[k + 2] & 0b00000011) << 2) + (buffer[k + 2]) >> 6;
                    rcse.ClutEntryCb = ((buffer[k + 3] & 0b00111111) >> 2);
                    rcse.ClutEntryT = buffer[k + 3] & 0b00000011;
                    k += 4;
                }
                Entries.Add(rcse);
            }
        }


        public override string ToString()
        {

            string res = "CLUT:\r\n ID " + ClutId.ToString() + ", Clut Version:" + ClutVersionNumber.ToString() + "\r\n";
            res += "\tTable;\r\n";
            foreach (RegionClutSegmentEntry item in this.Entries)
            {
                res += item.GetColor().ToString();
            }

            return res;
        }

        public string ToHtml()
        {
            string res = "<table>\r\n";

            res += "<tr><td>CLUT ID:</td<td>" + ClutId.ToString() + "</td></tr>\r\n";
            res += "<tr><td>Version:</td><td>" + ClutVersionNumber.ToString() + "</td></tr>\r\n";

            res += "<tr><td colspan=\"2\">Table:</td></tr>\r\n";

            foreach (RegionClutSegmentEntry item in this.Entries)
            {
                res += "<tr><td>" + item.ClutEntryId.ToString() + "</td><td>" + item.GetColor().ToString() + "</td></tr>\r\n";
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
                "CLUT_id",
                ClutId.ToString(),
                "Uniquely identifies within a page the CLUT family whose data is contained in this CLUT_definition_segment field.",
                grpGeneral
            );

            Utils.AddListViewEntry(
                Lv,
                "CLUT_version_number",
                ClutVersionNumber.ToString(),
                "Indicates the version of this segment data. When any of the contents of this segment change this version number is incremented(modulo 16). ",
                grpGeneral
            );


            Utils.AddListViewEntry(
                Lv,
                "number of regional clut entries:",
                this.Entries.Count.ToString(),
                "",
                grpGeneral
            );

            if (this.Entries.Count > 0)
            {
                foreach (RegionClutSegmentEntry item in this.Entries)
                {

                    ListViewGroup grpClutEntry = Lv.Groups.Add("CLUT Entry" + item.ClutEntryId.ToString(), "CLUT Entry ID " + item.ClutEntryId.ToString());

                    Utils.AddListViewEntry(
                        Lv,
                        "CLUT_entry_id",
                        item.ClutEntryId.ToString(),
                        "Specifies the entry number of the CLUT. The first entry of the CLUT has entry number zero",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "2-bit/entry_CLUT_flag",
                        item.ClutEntry2BitClutEntryFlag.ToString(),
                        "If set to '1', this indicates that this CLUT value is to be loaded into the identified entry of the 2 - bit / entry CLUT.This option shall not be used when the CDS accompanies an alternative CLUT segment(ACS).",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "4-bit/entry_CLUT_flag",
                        item.ClutEntry4BitClutEntryFlag.ToString(),
                        "If set to '1', this indicates that this CLUT value is to be loaded into the identified entry of the 4 - bit / entry CLUT.This option shall not be used when the CDS accompanies an alternative CLUT segment(ACS).",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "8-bit/entry_CLUT_flag",
                        item.ClutEntry8BitClutEntryFlag.ToString(),
                        "If set to '1', this indicates that this CLUT value is to be loaded into the identified entry of the 8 - bit / entry CLUT.This option shall be used when the CDS accompanies an alternative CLUT segment(ACS). ",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "full_range_flag",
                        item.FullRangeFlag.ToString(),
                        "If set to '1', this indicates that the Y_value, Cr_value, Cb_value and T_value fields have the full 8-bit resolution.If set to '0', then these fields contain only the most significant bits",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "Y_value",
                        item.ClutEntryY.ToString(),
                        "The Y output value of the CLUT for this entry. A value of zero in the Y_value field signals full transparency. In that case the values in the Cr_value, Cb_value and T_value fields are irrelevant and shall be set to zero.",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "Cr_value",
                        item.ClutEntryCr.ToString(),
                        "The Cr output value of the CLUT for this entry.",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "Cb_value",
                        item.ClutEntryCb.ToString(),
                        "The Cb output value of the CLUT for this entry.",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "T_value",
                        item.ClutEntryT.ToString(),
                        "The Transparency output value of the CLUT for this entry. A value of zero identifies no transparency. The maximum value plus one would correspond to full transparency.For all other values the level of transparency is defined by linear interpolation.",
                        grpClutEntry
                    );

                    Utils.AddListViewEntry(
                        Lv,
                        "Colour",
                        item.GetColor().ToString(),
                        "",
                        grpClutEntry
                    );
                }
            }
        }

    }
}

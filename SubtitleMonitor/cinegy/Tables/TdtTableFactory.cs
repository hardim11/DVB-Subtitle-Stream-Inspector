using Cinegy.TsDecoder.Tables;
using Cinegy.TsDecoder.TransportStream;
using McastCinergy.cinergy.TransportStream;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McastCinergy.cinergy.Tables
{
    public class TdtTableFactory : TableFactory
    {

        public TdtTable TdtTable { get; private set; }


        private new TdtTable InProgressTable
        {
            get { return base.InProgressTable as TdtTable; }
            set { base.InProgressTable = value; }
        }


        public void AddPacket(TsPacket packet)
        {
            CheckPid(packet.Pid);

            if (packet.PayloadUnitStartIndicator)
            {
                InProgressTable = new TdtTable { Pid = packet.Pid, PointerField = packet.Payload[0] };

                if (InProgressTable.PointerField > packet.Payload.Length)
                {
                    Debug.Assert(true, "Splice Info Table has packet pointer outside the packet.");
                }

                var pos = 1 + InProgressTable.PointerField;

                // InProgressTable.VersionNumber = (byte)(packet.Payload[pos + 5] & 0x3E);

                InProgressTable.TableId = packet.Payload[pos];


                if (InProgressTable.TableId != 0x70)
                {
                    InProgressTable = null;
                    return;
                }

                /* if (SpliceInfoTable?.VersionNumber != InProgressTable.VersionNumber)
                 {
                     //if the version number of any section jumps, we need to refresh
                     _sectionsCompleted = new HashSet<int>();
                     NetworkInformationItems = new List<NetworkInformationItem>();
                 }*/

                InProgressTable.SectionLength =
                    (short)(((packet.Payload[pos + 1] & 0x3) << 8) + packet.Payload[pos + 2]);


            }

            if (InProgressTable == null) return;

            /* if (_sectionsCompleted.Contains(InProgressTable.SectionNumber))
             {
                 InProgressTable = null;
                 return;
             }*/

            AddData(packet);

            if (!HasAllBytes()) return;
            /*
             *          https://www.etsi.org/deliver/etsi_en/300400_300499/300468/01.03.01_60/en_300468v010301p.pdf
             *          
                        time_date_section(){
                            table_id 8 uimsbf
                            section_syntax_indicator 1 bslbf
                            reserved_future_use 1 bslbf
                            reserved 2 bslbf
                            section_length 12 uimsbf
                            UTC_time 40 bslbf
                        }

                        table_id: See table 2.
                        section_syntax_indicator: This is a one-bit indicator which shall be set to "0".
                        section_length: This is a 12-bit field, the first two bits of which shall be "00". It specifies the number of bytes of the
                            section, starting immediately following the section_length field and up to the end of the section.
                        UTC_time: This 40-bit field contains the current time and date in UTC and MJD (see annex C). This field is coded as
                            16 bits giving the 16 LSBs of MJD followed by 24 bits coded as 6 digits in 4-bit BCD.
                            EXAMPLE: 93/10/13 12:45:00 is coded as "0xC079124500".]
 
                        40 bits == 5 bytes


                        Console.WriteLine("TDT InProgressTable.TableId = " + InProgressTable.TableId.ToString("X"));
                        Console.WriteLine("TDT InProgressTable.SectionLength = " + InProgressTable.SectionLength.ToString());
            */

            // DATE is 16 bits MJD (modified Julian Date)
            ushort mjd = (ushort)((packet.Payload[4] << 8) + (packet.Payload[5]));
            DateTime dtMjd = MjdToDateTime(mjd);

            // TIME is 24 bits as 6 digits BCD
            int Hour = ((byte)(packet.Payload[6] >> 4) * 10) + (byte)(packet.Payload[6] & 0x7);
            int Min = ((byte)(packet.Payload[7] >> 4) * 10) + (byte)(packet.Payload[7] & 0x7);
            int Sec = ((byte)(packet.Payload[8] >> 4) * 10) + (byte)(packet.Payload[8] & 0x7);

            InProgressTable.UtcTime = dtMjd.Add(new TimeSpan(Hour, Min, Sec));

            TdtTable = InProgressTable;

            Console.WriteLine("TDT = " + TdtTable.UtcTime.ToString());

            OnTableChangeDetected();
        }

        public static DateTime MjdToDateTime(int mjd_in)
        {

            // last attempting converting this page http://www.csgnetwork.com/julianmodifdateconv.html
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
    
            // Julian day
            double jd = Math.Floor((double)mjd_in) + 2400000.5;

            // Integer Julian day
            int jdi = (int)Math.Floor(jd);
    
            // Fractional part of day
            double jdf = jd - jdi + 0.5;
    
            // Really the next calendar day?
            if (jdf >= 1.0) {
               jdf = jdf - 1.0;
               jdi  = jdi + 1;
            }


            hour = (int)Math.Floor(jdf * 24.0);
            int l = jdi + 68569;
            int n = (int)Math.Floor((double)(4 * l / 146097));
   
            l = (int)(Math.Floor((double)l) - Math.Floor((double)((146097 * n + 3) / 4)));
            year = (int)Math.Floor((double)(4000 * (l + 1) / 1461001));

            l = (int)(l - (Math.Floor((double)(1461 * year / 4))) + 31);
            month = (int)Math.Floor((double)(80 * l / 2447));

            day = l - (int)Math.Floor((double)(2447 * month / 80));

            l = (int)Math.Floor((double)(month / 11));

            month = (int)Math.Floor((double)(month + 2 - 12 * l));
            year = (int)Math.Floor((double)(100 * (n - 49) + year + l));


            //year = year - 1900;
    
            return new DateTime(year, month, day);
        }



    }
}

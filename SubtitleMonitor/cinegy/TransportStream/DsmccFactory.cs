using Cinegy.TsDecoder.TransportStream;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McastCinergy.cinergy.TransportStream
{
    public class DsmccFactory
    {
        public Dsmcc Active { get; set; }
        private Dsmcc _InProgress { get; set; }

        private ushort _tableBytes;
        internal byte[] Data;


        public DsmccFactory()
        {

        }

        internal bool HasAllBytes()
        {
            return _tableBytes >= _InProgress.SectionLength + 3 && _InProgress.SectionLength > 0;
        }


        public void AddPacket(TsPacket packet)
        {
            //CheckPid(packet.Pid);

            Console.WriteLine("DsmccFactory packet");

            if (packet.PayloadUnitStartIndicator)
            {
                Console.WriteLine("####DsmccFactory PayloadUnitStartIndicator");

                _InProgress = new Dsmcc { Pid = packet.Pid, PointerField = packet.Payload[0] };

                if (_InProgress.PointerField > packet.Payload.Length)
                {
                    Debug.Assert(true, "Splice Info Table has packet pointer outside the packet.");
                }

                var pos = 1 + _InProgress.PointerField;

                // InProgressTable.VersionNumber = (byte)(packet.Payload[pos + 5] & 0x3E);

                //_InProgress.TableId = packet.Payload[pos];


                //if (_InProgress.TableId != 0x70)
                //{
                //    _InProgress = null;
                //    return;
                //}

                /* if (SpliceInfoTable?.VersionNumber != InProgressTable.VersionNumber)
                 {
                     //if the version number of any section jumps, we need to refresh
                     _sectionsCompleted = new HashSet<int>();
                     NetworkInformationItems = new List<NetworkInformationItem>();
                 }*/

                //_InProgress.SectionLength =
                //    (short)(((packet.Payload[pos + 1] & 0x3) << 8) + packet.Payload[pos + 2]);


            }


            if (_InProgress == null) return;

            /* if (_sectionsCompleted.Contains(InProgressTable.SectionNumber))
             {
                 InProgressTable = null;
                 return;
             }*/

            AddData(packet);

            if (!HasAllBytes()) return;


            //// DATE is 16 bits MJD (modified Julian Date)
            //ushort mjd = (ushort)((packet.Payload[4] << 8) + (packet.Payload[5]));
            //DateTime dtMjd = MjdToDateTime(mjd);

            //// TIME is 24 bits as 6 digits BCD
            //int Hour = ((byte)(packet.Payload[6] >> 4) * 10) + (byte)(packet.Payload[6] & 0x7);
            //int Min = ((byte)(packet.Payload[7] >> 4) * 10) + (byte)(packet.Payload[7] & 0x7);
            //int Sec = ((byte)(packet.Payload[8] >> 4) * 10) + (byte)(packet.Payload[8] & 0x7);

            //_InProgress.UtcTime = dtMjd.Add(new TimeSpan(Hour, Min, Sec));

            Active = _InProgress;

            //Console.WriteLine("TDT = " + Active.UtcTime.ToString());

            OnDsmccChanged();
        }

        protected void AddData(TsPacket packet)
        {
            // CheckPid(packet.Pid);

            if (packet.PayloadUnitStartIndicator)
            {
                //Console.WriteLine("packet.PayloadUnitStartIndicator");

                Data = new byte[_InProgress.SectionLength + 3];
                _tableBytes = 0;
            }

            if (Data == null)
            {
                //Console.WriteLine("packet.PayloadUnitStartIndicator");

                Data = new byte[_InProgress.SectionLength + 3];
                _tableBytes = 0;
            }

            if ((_InProgress.SectionLength + 3 - _tableBytes) > packet.Payload.Length)
            {
                Buffer.BlockCopy(packet.Payload, 0, Data, _tableBytes, packet.Payload.Length);
                _tableBytes += (ushort)(packet.Payload.Length);
            }
            else
            {
                if ((_InProgress.SectionLength + 3 - _tableBytes) > 0)
                {
                    Buffer.BlockCopy(packet.Payload, 0, Data, _tableBytes, (_InProgress.SectionLength + 3 - _tableBytes));
                    _tableBytes += (ushort)(_InProgress.SectionLength + 3 - _tableBytes);
                }
                else
                {
                    Console.WriteLine("Error more than 0 bytes needs copying");
                }
            }
        }


        public delegate void DsmccChanged(object sender, TransportStreamEventArgs args);

        // The associated table has changed / been updated
        public event DsmccChanged DsmccChangeDetected;

        protected void OnDsmccChanged()
        {
            var handler = DsmccChangeDetected;
            if (handler == null) return;

            var generatingPid = -1;

            if (_InProgress != null)
            {
                generatingPid = _InProgress.Pid;
            }

            var args = new TransportStreamEventArgs { TsPid = generatingPid };
            handler(this, args);
        }

    }
}

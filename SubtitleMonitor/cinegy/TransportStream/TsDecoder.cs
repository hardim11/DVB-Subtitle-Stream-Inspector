/* Copyright 2017 Cinegy GmbH.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Cinegy.TsDecoder.Tables;
using McastCinergy.cinergy.Tables;
using McastCinergy.cinergy.TransportStream;

namespace Cinegy.TsDecoder.TransportStream
{
    public class TsDecoder
    {

        public ProgramAssociationTable ProgramAssociationTable()
        {
            return _patFactory.ProgramAssociationTable;
        }
        public ServiceDescriptionTable ServiceDescriptionTable()
        {
            return _sdtFactory.ServiceDescriptionTable;
        }
        public NetworkInformationTable NetworkInformationTable()
        {
            return _nitFactory.NetworkInformationTable;
        }
        public EventInformationTable EventInformationTable()
        {
            return _eitFactory.EventInformationTable;
        }
        public SpliceInfoTable SpliceInfoTable()
        {
            return _sitFactory.SpliceInfoTable;
        }
        public TdtTable TdtTable()
        {
            return _tdtFactory.TdtTable;
        }
        public Dictionary<int, DsmccFactory> Dsmcc()
        {
            return _dsmccDictionary;
        }

        public List<ProgramMapTable> ProgramMapTables { get; private set; }
        
        private ProgramAssociationTableFactory _patFactory;
        private ServiceDescriptionTableFactory _sdtFactory;
        private List<ProgramMapTableFactory> _pmtFactories;

        private EventInformationTableFactory _eitFactory;
        private NetworkInformationTableFactory _nitFactory;
        private SpliceInfoTableFactory _sitFactory;
     
        private TsPacketFactory _packetFactory;

        private TdtTableFactory _tdtFactory;

        private Dictionary<int, DsmccFactory> _dsmccDictionary;


        public delegate void TableChangeEventHandler(object sender, TableChangedEventArgs args);

        public TsDecoder()
        {
/*
#if !NET461
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
 * */
            SetupFactories();
        }

        public void AddData(byte[] data)
        {
            // create a fatory if there isn't one already
            if(_packetFactory == null) _packetFactory = new TsPacketFactory();

            // get packets from the raw buffer
            var tsPackets = _packetFactory.GetTsPacketsFromData(data);
            
            if (tsPackets == null)
            {
                throw new InvalidDataException("Provided data buffer did not contain any TS packets");
            }

            // now process each packet
            foreach (var packet in tsPackets)
            {
                AddPacket(packet);
            }

        }

        public void AddPackets(IEnumerable<TsPacket> newPackets)
        {
            foreach (var newPacket in newPackets)
            {
                AddPacket(newPacket);
            }
        }
        
        public void AddPacket(TsPacket newPacket)
        {
            try
            {

                //if (newPacket.Pid == 16)
                //{
                //    Console.WriteLine("NIT");
                //}


                //if (newPacket.Pid == 17)
                //{
                //    Console.WriteLine("SDT");
                //}

                if (newPacket.TransportErrorIndicator)
                {
                    return;
                }

                switch (newPacket.Pid)
                {
                    case (short)PidType.PatPid:
                        _patFactory.AddPacket(newPacket);
                        break;
                    case (short)PidType.SdtPid:
                        _sdtFactory.AddPacket(newPacket);
                        break;
                    //case (short)PidType.EitPid:
                    //    _eitFactory.AddPacket(newPacket);
                    //    break;
                    case 2048:
                        _sitFactory.AddPacket(newPacket);
                        break;

                    case (short)PidType.TdtPid:
                        _tdtFactory.AddPacket(newPacket);
                        break;

                    default:
                        CheckPmt(newPacket);
                        break;
                }

                if (newPacket.Pid == 0x1929)
                {

                    if (!_dsmccDictionary.ContainsKey(newPacket.Pid))
                    {
                        _dsmccDictionary[newPacket.Pid] = new DsmccFactory();
                    }

                    Console.WriteLine("Arqiva test carousel PayloadUnitStartIndicator = " + newPacket.PayloadUnitStartIndicator.ToString() + ", ContainsPayload = " + newPacket.ContainsPayload.ToString());
                    _dsmccDictionary[newPacket.Pid].AddPacket(newPacket);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Debug.WriteLine("Exception generated within AddPacket method: " + ex.Message);

            }
        }

        public ServiceDescriptor GetServiceDescriptorForProgramNumber(int? programNumber)
        {
            if (programNumber == null)
                programNumber = _patFactory.ProgramAssociationTable.ProgramNumbers.Where(i => i > 0).FirstOrDefault();

            if (programNumber == 0) return null;

            var serviceDescItem = _sdtFactory.ServiceDescriptionItems.SingleOrDefault(i => i.ServiceId == programNumber);

            if (serviceDescItem == null)
            {
                Console.WriteLine("unknown GetServiceDescriptorForProgramNumber");
                return null;
            }

            var serviceDesc = serviceDescItem.Descriptors.SingleOrDefault(sd => (sd as ServiceDescriptor) != null) as ServiceDescriptor;

            return serviceDesc;
        }

        public T GetDescriptorForProgramNumberByTag<T>( int? programNumber, int streamType, int descriptorTag)  where T : class
        {
            if (programNumber == null) return null;
            
            var selectedPmt = ProgramMapTables.FirstOrDefault(t => t.ProgramNumber == programNumber);

            if (selectedPmt == null) return null;

            var selectedDesc = default(T);

            foreach (var esStream in selectedPmt.EsStreams)
            {
                if (esStream.StreamType != streamType) continue;

                selectedDesc = esStream.Descriptors.SingleOrDefault(d => d.DescriptorTag == descriptorTag) as T;

                if (selectedDesc != null) break;
            }
        
            return selectedDesc;
            
        }

        public EsInfo GetEsStreamForProgramNumberByTag(int? programNumber, int streamType, int descriptorTag) 
        {
            if (programNumber == null) return null;

            var selectedPmt = ProgramMapTables.FirstOrDefault(t => t.ProgramNumber == programNumber);

            if (selectedPmt == null) return null;

            foreach (var esStream in selectedPmt.EsStreams)
            {
                if (esStream.StreamType != streamType) continue;

                var desc = esStream.Descriptors.SingleOrDefault(d => d.DescriptorTag == descriptorTag);

                if (desc != null) return esStream;
               
            }

            return null;
        }

        private void CheckPmt(TsPacket tsPacket)
        {
            if (_patFactory.ProgramAssociationTable == null) return;

            if (tsPacket.Pid == (short)PidType.NitPid)
            {
                _nitFactory.AddPacket(tsPacket);
                return;
            }

           // CheckPcr(tsPacket);

            var contains = false;

            foreach (var pid in _patFactory.ProgramAssociationTable.Pids)
            {
                if (pid != tsPacket.Pid) continue;
                contains = true;
                break;
            }

            if (!contains) return;

            ProgramMapTableFactory selectedPmt = null;
            foreach (var t in _pmtFactories)
            {
                if (t.TablePid != tsPacket.Pid) continue;
                selectedPmt = t;
                break;
            }

            if (selectedPmt == null)
            {
                selectedPmt = new ProgramMapTableFactory();
                selectedPmt.TableChangeDetected += _pmtFactory_TableChangeDetected;
                _pmtFactories.Add(selectedPmt);
            }

            selectedPmt.AddPacket(tsPacket);
        }

     
        private void SetupFactories()
        {
            _patFactory = new ProgramAssociationTableFactory();
            _patFactory.TableChangeDetected += _patFactory_TableChangeDetected;
            _pmtFactories = new List<ProgramMapTableFactory>(16);
            ProgramMapTables = new List<ProgramMapTable>(16);

            _sdtFactory = new ServiceDescriptionTableFactory();
            _sdtFactory.TableChangeDetected += _sdtFactory_TableChangeDetected;

            _eitFactory = new EventInformationTableFactory();
            _eitFactory.TableChangeDetected += _eitFactory_TableChangeDetected;

            _nitFactory = new NetworkInformationTableFactory();
            _nitFactory.TableChangeDetected += _nitFactory_TableChangeDetected;

            _sitFactory = new SpliceInfoTableFactory();

            _tdtFactory = new TdtTableFactory();

            _dsmccDictionary = new Dictionary<int, DsmccFactory>();
        }

        public ProgramMapTable GetSelectedPmt(int programNumber = 0)
        {
            ProgramMapTable pmt;

            if (programNumber == 0)
            {
                if (ProgramMapTables.Count == 0) return null;
                if (_patFactory.ProgramAssociationTable == null) return null;
                //without a passed program number, use the default program
                if (ProgramMapTables.Count <
                    (_patFactory.ProgramAssociationTable.Pids.Length - 1)) return null;

                pmt =  ProgramMapTables.OrderBy(t => t.ProgramNumber).FirstOrDefault();
            }
            else
            {
                pmt = ProgramMapTables.SingleOrDefault(t => t.ProgramNumber == programNumber);
            }

            return pmt;
        }

        private void _sdtFactory_TableChangeDetected(object sender, TransportStreamEventArgs e)
        {
            try
            {
                var name = GetServiceDescriptorForProgramNumber(ProgramMapTables.FirstOrDefault().ProgramNumber);

                if (name == null)
                {

                }
                else
                {
                    var message =
                        string.Format("SDT {0} Refreshed: {1} - {2} (Version {3}, Section {4})", e.TsPid, name.ServiceName, name.ServiceProviderName, _sdtFactory.ServiceDescriptionTable.VersionNumber, _sdtFactory.ServiceDescriptionTable.SectionNumber);

                    OnTableChangeDetected(new TableChangedEventArgs() { Message = message, TablePid = e.TsPid, TableType = TableType.Sdt });
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem reading service name: " + ex.Message);   
            }
        }

        private void _pmtFactory_TableChangeDetected(object sender, TransportStreamEventArgs e)
        {
            string message;
            lock (this)
            {
                var fact = sender as ProgramMapTableFactory;

                if (fact == null) return;

                var selectedPmt = ProgramMapTables.FirstOrDefault(t => t.Pid == e.TsPid);

                if (selectedPmt != null)
                {
                    ProgramMapTables.Remove(selectedPmt);
                    message = string.Format("PMT {0} refreshed", e.TsPid);
                }
                else
                {
                    message = string.Format("PMT {0} added", e.TsPid);
                }

                ProgramMapTables.Add(fact.ProgramMapTable);
            }

            OnTableChangeDetected(new TableChangedEventArgs() { Message = message, TablePid = e.TsPid, TableType = TableType.Pmt});
        }

        private void _patFactory_TableChangeDetected(object sender, TransportStreamEventArgs e)
        {
            _pmtFactories = new List<ProgramMapTableFactory>(16);
            ProgramMapTables = new List<ProgramMapTable>(16);

            _sdtFactory = new ServiceDescriptionTableFactory();
            _sdtFactory.TableChangeDetected += _sdtFactory_TableChangeDetected;

            OnTableChangeDetected(new TableChangedEventArgs() {Message = "PAT refreshed - resetting all factories" , TablePid = e.TsPid, TableType = TableType.Pat});
        }

        private void _eitFactory_TableChangeDetected(object sender, TransportStreamEventArgs e)
        {
            string message;
            lock (this)
            {
                var fact = sender as EventInformationTableFactory;

                if (fact == null) return;
                message = string.Format("EIT {0} Refreshed:", e.TsPid);

            }

            OnTableChangeDetected(new TableChangedEventArgs() { Message = message, TablePid = e.TsPid, TableType = TableType.Eit});
        }

        private void _nitFactory_TableChangeDetected(object sender, TransportStreamEventArgs e)
        {
            string message;
            lock (this)
            {
                var fact = sender as NetworkInformationTableFactory;

                if (fact == null) return;
                message = string.Format("NIT {0} Refreshed: (Version {1}, Section {2})", e.TsPid, _nitFactory.NetworkInformationTable.VersionNumber, _nitFactory.NetworkInformationTable.SectionNumber);

            }

            OnTableChangeDetected(new TableChangedEventArgs() { Message = message, TablePid = e.TsPid, TableType = TableType.Nit});
        }

        //A decoded table change has been processed
        public event TableChangeEventHandler TableChangeDetected;

        private void OnTableChangeDetected(TableChangedEventArgs args)
        {   
            var handler = TableChangeDetected;
            if (handler != null)
            {
                handler.Invoke(this, args);
            }
        }
    }


}


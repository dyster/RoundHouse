using PacketDotNet;
using RoundHouse.Annotations;
using RoundHouse.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace RoundHouse
{
    public class NetDevice : INotifyPropertyChanged
    {
        private PingReply _reply;
        private DateTime _lastPing;
        private IPAddress _ip;
        private readonly bool _waitforping;
        private PhysicalAddress _mac;
        private int _arps;

        private bool _lookedUpMac = false;
        private string _macVendor;
        private string _dnsrecord;
        private string _netbios;

        private bool _portScanned = false;
        private string _openPorts;

        private ConcurrentDictionary<int, bool> _scannedPorts = new ConcurrentDictionary<int, bool>();

        public NetDevice(IPAddress ip, bool waitforping)
        {
            _ip = ip;
            _waitforping = waitforping;
            //PortScan();
            Ping();

            if (!_waitforping)
                Init();
        }

        public void Init()
        {
            DnsLookup();
            NetBiosLookup();

            foreach (var port in StandardPorts.NamedPorts.Keys)
                ScanPort(port);

            ScanPort(21);
            ScanPort(23);
            ScanPort(80);
            ScanPort(443);
            ScanPort(8080);
            ScanPort(1080);
            ScanPort(12320);
            ScanPort(12321);
            ScanPort(12322);
        }

        public void Ping()
        {
            ThreadStats.PingQueued++;
            ThreadPool.QueueUserWorkItem((state) =>
            {
                ThreadStats.PingStarted++;
                var ping = new Ping();
                var pingReply = ping.Send(_ip, 5000);
                var pingReplyStatus = pingReply.Status;
                AddPing(pingReply);
                if (_waitforping && pingReplyStatus == IPStatus.Success)
                    Init();
                ThreadStats.PingFinished++;
            });
        }

        public void AddPing(PingReply reply)
        {
            _reply = reply;
            LastPing = DateTime.Now;
        }

        public void NetBiosLookup()
        {
            NetBios = "init";

            ThreadPool.QueueUserWorkItem((state) =>
            {
                NetBios = "querying";
                GetRemoteNetBiosName(_ip, out string netbiosName, out string netbiosDomainName);
                NetBios = netbiosName + "@" + netbiosDomainName;
            });
        }

        public void DnsLookup()
        {
            DNSRecord = "init";

            ThreadPool.QueueUserWorkItem((state) =>
            {
                DNSRecord = "querying";
                try
                {
                    IPHostEntry entry = Dns.GetHostEntry(_ip);
                    if (!string.IsNullOrEmpty(entry.HostName))
                    {
                        DNSRecord = entry.HostName;
                    }
                    else
                        DNSRecord = "null";
                }
                catch (SocketException ex)
                {
                    DNSRecord = ex.Message;
                }
            });
        }

        public async void MacLookup()
        {
            if (_mac != null && !_lookedUpMac)
            {              

                string s = "";
                if (Settings.Default.MacVendors.ContainsKey(_mac.ToString()))
                {
                    MACVendor = Settings.Default.MacVendors.GetVendor(_mac.ToString());
                }
                else
                {
                    MACVendor = "Querying Server";
                    WebClient client = new WebClient();

                    try
                    {
                        
                        s = client.DownloadString("https://api.macvendors.com/" +
                                                                 System.Web.HttpUtility.UrlEncode(_mac.ToString()));
                        MACVendor = s;
                        Settings.Default.MacVendors.AddEntry(_mac.ToString(), s);
                        Settings.Default.Save();
                    }
                    catch (WebException e)
                    {
                        if (e.Message.Contains("404"))
                        {
                            MACVendor = "Not Found";
                        }
                        else
                            MACVendor = e.Message;
                    }
                }

                _lookedUpMac = true;
            }
        }

        public void AddArp(ArpPacket arp)
        {
            var arpSenderHardwareAddress = arp.SenderHardwareAddress;
            var arpSenderProtocolAddress = arp.SenderProtocolAddress;

            IP = arpSenderProtocolAddress;
            MAC = arpSenderHardwareAddress;
            if (Settings.Default.MacVendors.ContainsKey(MAC.ToString()))
                MACVendor = Settings.Default.MacVendors.GetVendor(MAC.ToString());
            Arps++;
        }

        public string MACVendor
        {
            get => _macVendor;
            set
            {
                if (value == _macVendor) return;
                _macVendor = value;
                OnPropertyChanged();
            }
        }

        public string DNSRecord
        {
            get => _dnsrecord;
            set
            {
                if (value == _dnsrecord) return;
                _dnsrecord = value;
                OnPropertyChanged();
            }
        }

        public string NetBios
        {
            get => _netbios;
            set
            {
                if (value == _netbios) return;
                _netbios = value;
                OnPropertyChanged();
            }
        }

        public PhysicalAddress MAC
        {
            get => _mac;
            private set
            {
                if (Equals(value, _mac)) return;
                _mac = value;
                OnPropertyChanged();
            }
        }

        public IPAddress IP
        {
            get => _ip;
            private set
            {
                if (Equals(value, _ip)) return;
                _ip = value;
                OnPropertyChanged();
            }
        }

        //private async void PortScan()
        //{
        //    if (!_portScanned)
        //    {
        //        var list = new List<int>();
        //
        //        foreach (var port in StandardPorts.Ports)
        //        {
        //            var s = await DoesWebClientConnect(_ip, port);
        //            if (s)
        //            {
        //                list.Add(port);
        //                OpenPorts = string.Join(",", list) + " - Scanning";
        //            }
        //        }
        //
        //
        //        if (list.Count == 0)
        //            OpenPorts = "none";
        //        else
        //            OpenPorts = string.Join(",", list);
        //
        //        _portScanned = true;
        //    }
        //}

        public void ScanPort(int scanport)
        {
            ThreadStats.PortQueued++;
            ThreadPool.QueueUserWorkItem((state) =>
            {
                ThreadStats.PortStarted++;
                var doesWebClientConnect = DoesWebClientConnect(_ip, scanport);

                _scannedPorts.AddOrUpdate(scanport, doesWebClientConnect, (i, b) => b);
                var ports = _scannedPorts.Where(pair => pair.Value).Select(pair => pair.Key).ToList();
                var portstrings = new List<string>();
                foreach (var port in ports)
                {
                    if (StandardPorts.NamedPorts.ContainsKey(port))
                        portstrings.Add(StandardPorts.NamedPorts[port].ToString());
                    else
                        portstrings.Add(port.ToString());
                }
                OpenPorts = string.Join(",", portstrings);
                ThreadStats.PortFinished++;
            });
        }

        public static bool DoesWebClientConnect(IPAddress ip, int port)
        {
            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(ip, port);

                    return true;
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        return false;
                    }

                    //An error occurred when attempting to access the socket

                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }

        public DateTime LastPing
        {
            get => _lastPing;
            set
            {
                if (value.Equals(_lastPing)) return;
                _lastPing = value;
                OnPropertyChanged();
            }
        }

        public int Arps
        {
            get => _arps;
            set
            {
                if (value == _arps) return;
                _arps = value;
                OnPropertyChanged();
            }
        }

        public string OpenPorts
        {
            get => _openPorts;
            set
            {
                if (value == _openPorts) return;
                _openPorts = value;
                OnPropertyChanged();
            }
        }

        public ThreadStats ThreadStats { get; set; } = new ThreadStats();

        //public string PingStatus => _reply == null ? "ARP" : _reply.Status.ToString();
        public string PingStatus => _reply == null ? "NULL" : _reply.Status.ToString();

        public long RoundTrip => _reply?.RoundtripTime ?? 0;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Get the NetBIOS machine name and domain / workgroup name by ip address using UPD datagram at port 137. Based on code I found at microsoft
        /// </summary>
        /// <returns>True if getting an answer on port 137 with a result</returns>
        public static bool GetRemoteNetBiosName(IPAddress targetAddress, out string nbName, out string nbDomainOrWorkgroupName, int receiveTimeOut = 5000, int retries = 1)
        {
            // The following byte stream contains the necessary message
            // to request a NetBios name from a machine
            byte[] nameRequest = new byte[]{
        0x80, 0x94, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x20, 0x43, 0x4b, 0x41,
        0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41,
        0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41,
        0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41,
        0x41, 0x41, 0x41, 0x41, 0x41, 0x00, 0x00, 0x21,
        0x00, 0x01 };

            do
            {
                byte[] receiveBuffer = new byte[1024];
                Socket requestSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                requestSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeOut);

                nbName = null;
                nbDomainOrWorkgroupName = null;

                EndPoint remoteEndpoint = new IPEndPoint(targetAddress, 137);
                IPEndPoint originEndpoint = new IPEndPoint(IPAddress.Any, 0);
                requestSocket.Bind(originEndpoint);
                requestSocket.SendTo(nameRequest, remoteEndpoint);
                try
                {
                    int receivedByteCount = requestSocket.ReceiveFrom(receiveBuffer, ref remoteEndpoint);

                    // Is the answer long enough?
                    if (receivedByteCount >= 90)
                    {
                        Encoding enc = new ASCIIEncoding();
                        nbName = enc.GetString(receiveBuffer, 57, 15).Trim();
                        nbDomainOrWorkgroupName = enc.GetString(receiveBuffer, 75, 15).Trim();

                        // the names may be checked if they are really valid NetBIOS names, but I don't care ....

                        return true;
                        //<----------
                    }
                    else
                    {
                        nbName = "response too short";
                        return true;
                    }
                }
                catch (SocketException)
                {
                    // We get this Exception when the target is not reachable
                }

                retries--;
            } while (retries >= 0);

            return false;
            //< ----------
        }
    }

    public class ThreadStats
    {
        public int PingQueued { get; set; }
        public int PingStarted { get; set; }
        public int PingFinished { get; set; }
        public int PortQueued { get; set; }
        public int PortStarted { get; set; }
        public int PortFinished { get; set; }

        public override string ToString()
        {
            return $"PingQue:{PingQueued} PingStart:{PingStarted} PingFinished:{PingFinished} PortQue:{PortQueued} PortStart:{PortStarted} PortFinished:{PortFinished}";
        }
    }

    public static class StandardPorts
    {
        public static Dictionary<int, string> NamedPorts = new Dictionary<int, string> {
            {22, "SSH" },
            {161, "SNMP Client" },
            {162, "SNMP Server" },
            {10161, "SNMP Client" },
            {10162, "SNMP Server" },
        };

        public static int[] Ports = new int[]
        {
            1,
                7    ,
                9    ,
                11   ,
                13   ,
                15   ,
                17   ,
                18   ,
                19   ,
                20   ,
                21   ,
                22   ,
                23   ,
                25   ,
                37   ,
                43   ,
                49   ,
                53   ,
                70   ,
                79   ,
                80   ,
                81   ,
                83   ,
                88   ,
                90   ,
                95   ,
                101  ,
                102  ,
                104  ,
                105  ,
                107  ,
                108  ,
                109  ,
                110  ,
                111  ,
                113  ,
                115  ,
                117  ,
                118  ,
                119  ,
                126  ,
                135  ,
                137  ,
                139  ,
                143  ,
                152  ,
                153  ,
                156  ,
                158  ,
                162  ,
                170  ,
                177  ,
                179  ,
                194  ,
                199  ,
                201  ,
                209  ,
                210  ,
                213  ,
                218  ,
                220  ,
                259  ,
                262  ,
                264  ,
                280  ,
                300  ,
                308  ,
                311  ,
                318  ,
                350  ,
                351  ,
                356  ,
                366  ,
                369  ,
                370  ,
                371  ,
                383  ,
                384  ,
                387  ,
                388  ,
                389  ,
                399  ,
                401  ,
                427  ,
                433  ,
                434  ,
                443  ,
                444  ,
                445  ,
                464  ,
                465  ,
                475  ,
                491  ,
                497  ,
                502  ,
                504  ,
                510  ,
                512  ,
                513  ,
                514  ,
                515  ,
                520  ,
                524  ,
                530  ,
                532  ,
                540  ,
                542  ,
                543  ,
                544  ,
                546  ,
                547  ,
                548  ,
                550  ,
                554  ,
                556  ,
                563  ,
                564  ,
                587  ,
                591  ,
                593  ,
                601  ,
                604  ,
                625  ,
                631  ,
                635  ,
                636  ,
                639  ,
                641  ,
                643  ,
                646  ,
                647  ,
                648  ,
                651  ,
                653  ,
                654  ,
                655  ,
                657  ,
                660  ,
                666  ,
                674  ,
                688  ,
                690  ,
                691  ,
                694  ,
                695  ,
                700  ,
                701  ,
                702  ,
                706  ,
                711  ,
                712  ,
                749  ,
                751  ,
                753  ,
                754  ,
                760  ,
                782  ,
                783  ,
                800  ,
                802  ,
                808  ,
                829  ,
                830  ,
                831  ,
                832  ,
                833  ,
                843  ,
                847  ,
                848  ,
                853  ,
                860  ,
                861  ,
                862  ,
                873  ,
                888  ,
                897  ,
                898  ,
                902  ,
                903  ,
                953  ,
                981  ,
                987  ,
                989  ,
                990  ,
                991  ,
                992  ,
                993  ,
                995  ,
                1010 ,
                1023
        };
    }
}
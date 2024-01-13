using BrightIdeasSoftware;
using PacketDotNet;
using RoundHouse.Annotations;
using RoundHouse.Properties;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace RoundHouse
{
    public partial class Form1 : Form
    {
        private BindingListInvoked _netDevices;
        private readonly object _listAddLock = new object();

        private readonly List<ArpPacket> _arpBuffer = new List<ArpPacket>();
        private readonly List<PingReplyReturn> _pingBuffer = new List<PingReplyReturn>();

        public Form1()
        {
            InitializeComponent();
            Settings.Default.SettingsLoaded += Default_SettingsLoaded;
            if(Settings.Default.MacVendors == null)
                Settings.Default.MacVendors = new MacDictionary();

            ThreadPool.SetMinThreads(50, 50);
        }

        private void Default_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            
        }

        private bool FilterPredicate(object o)
        {
            var device = (NetDevice)o;

            if (device.MAC != null) return true;
            if (device.Arps > 0) return true;
            if (!string.IsNullOrEmpty(device.OpenPorts))
                return true;
            if (device.PingStatus == "Success") return true;
            return false;
        }

        private delegate void AddNetDeviceDelegate(List<NetDevice> netDevices);

        public void AddNetDevice(List<NetDevice> netDevices)
        {
            if (this.InvokeRequired)
                this.Invoke(new AddNetDeviceDelegate(AddNetDevice), netDevices);
            else
            {
                if (_netDevices == null)
                {
                    _netDevices = new BindingListInvoked(netDevices);
                    dataListView1.DataSource = _netDevices;
                    dataListView1.AutoResizeColumns();
                    dataListView1.AdditionalFilter = new ModelFilter(FilterPredicate);
                }
                else
                {
                    foreach (var netDevice in netDevices)
                        _netDevices.Add(netDevice);
                }
            }
        }
               
        private void CaptureDeviceOnOnPacketArrival(object sender, PacketCapture e)
        {
            var raw = e.GetPacket();
            var packet = Packet.ParsePacket(raw.LinkLayerType, raw.Data);
            if (packet.PayloadPacket is ArpPacket)
            {
                var arp = (ArpPacket)packet.PayloadPacket;
                var arpSenderHardwareAddress = arp.SenderHardwareAddress;
                var arpSenderProtocolAddress = arp.SenderProtocolAddress;
                var arpTargetHardwareAddress = arp.TargetHardwareAddress;
                var arpTargetProtocolAddress = arp.TargetProtocolAddress;

                lock (_listAddLock)
                {
                    _arpBuffer.Add(arp);
                }
            }
            else if (packet.PayloadPacket is IPv6Packet)
            {
                // ignore
            }
            else if (packet.PayloadPacket is IPv4Packet)
            {
                var ipv4 = (IPv4Packet)packet.PayloadPacket;

                if (ipv4.Protocol == ProtocolType.Icmp)
                {
                }
                else if (ipv4.Protocol == ProtocolType.Igmp)
                {
                }
                else if (ipv4.Protocol != ProtocolType.Tcp && ipv4.Protocol != ProtocolType.Udp)
                {
                }
            }
            else if (packet.PayloadPacket == null)
            {
            }
            else
            {
            }
        }

        private void startupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var list = new List<IPAddress>();

            foreach (var networkInterface in interfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var description = networkInterface.Description;
                    var name = networkInterface.Name;
                    var ipInterfaceProperties = networkInterface.GetIPProperties();
                    var iPv4Statistics = networkInterface.GetIPv4Statistics();
                    var physicalAddress = networkInterface.GetPhysicalAddress();
                    var supports = networkInterface.Supports(NetworkInterfaceComponent.IPv4);

                    foreach (UnicastIPAddressInformation ip in ipInterfaceProperties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            var mask = ip.IPv4Mask;
                            var ipAdd = ip.Address;
                            if (ipAdd.ToString().StartsWith("169.254"))
                                continue;

                            if (ipAdd.ToString().StartsWith("127.0"))
                                continue;

                            var bytes = ipAdd.GetAddressBytes();

                            for (byte i = 1; i < 255; i++)
                            {
                                var ipAddress = new IPAddress(new byte[] { bytes[0], bytes[1], bytes[2], i });
                                if (!ipAddress.Equals(ipAdd))
                                {
                                    list.Add(ipAddress);

                                    if (ipAddress.ToString() == "192.168.0.15")
                                    {
                                    }

                                    if (ipAddress.Equals(IPAddress.Parse("192.168.0.15")))
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var devices = list.Select(ip => new NetDevice(ip, Settings.Default.WaitForPing)).ToList();

            AddNetDevice(devices);

            //foreach (var netDevice in devices)
            //    netDevice.Init();

            //foreach (var ipAddress in list)
            //{
            //    ThreadPool.QueueUserWorkItem((state) =>
            //    {
            //        return;
            //        var ping = new Ping();
            //        var pingReply = ping.Send(ipAddress, 5000);
            //        var pingReplyStatus = pingReply.Status;
            //        //if (pingReplyStatus != IPStatus.TimedOut && pingReplyStatus != IPStatus.DestinationHostUnreachable)
            //        {
            //            lock (_listAddLock)
            //            {
            //                _pingBuffer.Add(new PingReplyReturn(){Ip = ipAddress, PingReply = pingReply});
            //            }
            //
            //        }
            //    });
            //
            //    ThreadPool.QueueUserWorkItem((state) =>
            //    {
            //        //var doesWebClientConnect = NetDevice.DoesWebClientConnect(ipAddress, 22).Result;
            //
            //    });
            //}
        }

        private void timerAddBuffer_Tick(object sender, EventArgs e)
        {
            if (_arpBuffer.Count > 0)
            {
                lock (_listAddLock)
                {
                    foreach (var arp in _arpBuffer)
                    {
                        var singleOrDefault = _netDevices.SingleOrDefault(pinger => pinger.IP.ToString() == arp.SenderProtocolAddress.ToString());

                        singleOrDefault?.AddArp(arp);
                    }
                    _arpBuffer.Clear();
                }
            }

            //if (_pingBuffer.Count > 0)
            //{
            //    lock (_listAddLock)
            //    {
            //        foreach (var pingReply in _pingBuffer)
            //        {
            //            var singleOrDefault = _netDevices.SingleOrDefault(pinger => pinger.IP.ToString() == pingReply.Ip.ToString());
            //            if (singleOrDefault == null)
            //            {
            //                throw new Exception("IMPOSSIBLE");
            //            }
            //            else
            //            {
            //                singleOrDefault.AddPing(pingReply.PingReply);
            //            }
            //        }
            //        _pingBuffer.Clear();
            //    }
            //}

            ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
            ThreadPool.GetMaxThreads(out var maxworkerThreads, out var maxcompletionPortThreads);

            int pingstarted = 0;
            int pingqueued = 0;
            int pingfinished = 0;
            int portqueued = 0;
            int portstarted = 0;
            int portfinished = 0;

            foreach (var netDevice in _netDevices)
            {
                pingstarted += netDevice.ThreadStats.PingStarted;
                pingqueued += netDevice.ThreadStats.PingQueued;
                pingfinished += netDevice.ThreadStats.PingFinished;
                portqueued += netDevice.ThreadStats.PortQueued;
                portstarted += netDevice.ThreadStats.PortStarted;
                portfinished += netDevice.ThreadStats.PortFinished;
            }

            _status.Text = $"Pings [{pingqueued}/{pingstarted}/{pingfinished}] PortScans [{portqueued}/{portstarted}/{portfinished}]  {maxworkerThreads - workerThreads}/{maxworkerThreads} Threads Used";
        }

        public void PortScan(IPAddress ip)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }

        private void timerSlow_Tick(object sender, EventArgs e)
        {            
            // why did I do this?
            dataListView1.AdditionalFilter = new ModelFilter(FilterPredicate);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CaptureDeviceList devices = CaptureDeviceList.Instance;

            foreach (var captureDevice in devices.Where(d => d is LibPcapLiveDevice))
            {
                captureDevice.OnPacketArrival += CaptureDeviceOnOnPacketArrival;
                captureDevice.Open(DeviceModes.Promiscuous);                
                captureDevice.StartCapture();
            }

            startupWorker.RunWorkerAsync();
            Thread t = new Thread(MaintainerThread);
            t.Start();
        }

        private void MaintainerThread()
        {
            Thread.Sleep(2000); // delay start;
            var run = true;
            while (run)
            {
                var firstOrDefault = _netDevices.FirstOrDefault(netdev => netdev.MAC != null && string.IsNullOrEmpty(netdev.MACVendor));
                if (firstOrDefault != null)
                {
                    firstOrDefault.MacLookup();
                }
                
                Thread.Sleep(3000);
            }
            MessageBox.Show("Maintainer finished");
            
        }
       
    }

    [Serializable]
    public class MacDictionary : List<MacAndVendor>
    {
        public bool ContainsKey(string mac)
        {
            return Exists(m => m.Mac == mac);
        }

        public string GetVendor(string mac)
        {
            return Find(m => m.Mac == mac).Vendor;
        }

        public void AddEntry(string mac, string vendor)
        {
            Add(new MacAndVendor() { Mac = mac, Vendor = vendor });
        }
    }

    [Serializable]
    public class MacAndVendor
    {
        public string Mac { get; set; }
        public string Vendor { get; set; }
    }

    public struct PortScanReturn
    {
        public int Port;
        public bool Success;
    }

    public struct PingReplyReturn
    {
        public IPAddress Ip;
        public PingReply PingReply;
    }

    public class BindingListInvoked : BindingList<NetDevice>
    {
        public BindingListInvoked(List<NetDevice> netDevices) : base(netDevices)
        {
        }

        private ISynchronizeInvoke _invoke;

        public BindingListInvoked(ISynchronizeInvoke invoke)
        {
            _invoke = invoke;
        }

        //public BindingListInvoked(IList<T> items) { this.DataSource = items; }

        private delegate void ListChangedDelegate(ListChangedEventArgs e);

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (_invoke != null && _invoke.InvokeRequired)
            {
                var elementAtOrDefault = this.ElementAtOrDefault(e.NewIndex);

                IAsyncResult ar = _invoke.BeginInvoke(new ListChangedDelegate(base.OnListChanged), new object[] { e });
            }
            else
            {
                base.OnListChanged(e);
            }
        }

        public IList<NetDevice> DataSource
        {
            get { return this; }
            set
            {
                if (value != null)
                {
                    this.ClearItems();
                    RaiseListChangedEvents = false;

                    foreach (NetDevice item in value)
                    {
                        this.Add(item);
                    }

                    RaiseListChangedEvents = true;
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }
    }
}
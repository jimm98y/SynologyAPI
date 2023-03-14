namespace SynologyAPI.Model
{
    public class Device
    {
        public string Band { get; set; }
        public string Connection { get; set; }
        public int? CurrentRate { get; set; }
        public string DeviceType { get; set; }
        public string HostName { get; set; }
        public string IP4 { get; set; }
        public string IP6 { get; set; }
        public bool? IsBanned { get; set; }
        public bool? IsBeamformingOn { get; set; }
        public bool? IsGuest { get; set; }
        public bool? IsHighQOS { get; set; }
        public bool? IsLowQOS { get; set; }
        public bool? IsManualDeviceType { get; set; }
        public bool? IsManualHostName { get; set; }
        public bool? IsOnline { get; set; }
        public bool? IsQOS { get; set; }
        public bool? IsWireless { get; set; }
        public string MAC { get; set; }
        public int? MaxRate { get; set; }
        public int? MeshNodeID { get; set; }
        public string RateQuality { get; set; }
        public int? SignalStrength { get; set; }
        public int? TransferRXRate { get; set; }
        public int? TransferTXRate { get; set; }
    }
}

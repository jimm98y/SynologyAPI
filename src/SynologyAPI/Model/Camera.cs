namespace SynologyAPI.Model
{   
    public class Camera
    {
        public int? AudioCodec { get; set; }
        public string Channel { get; set; }
        public bool? Enabled { get; set; }
        public string Folder { get; set; }
        public int Id { get; set; }
        public string Ip { get; set; }
        public int? LiveViewSource { get; set; }
        public string Model { get; set; }
        public string NewName { get; set; }
        public int? Port { get; set; }
        public string RecShare { get; set; }
        public int? RecShareMountType { get; set; }
        public string RecSharePath { get; set; }
        public int? RecStatus { get; set; }
        public int? RecStorageStatus { get; set; }
        public string RecVolume { get; set; }
        public int? RecordTime { get; set; }
        public int? RecordingKeepDays { get; set; }
        public string RecordingKeepSize { get; set; }
        public int? RotateOption { get; set; }
        public int? Status { get; set; }
        public int? StatusFlags { get; set; }
        public StreamInfo Stream1 { get; set; }
        public StreamInfo Stream2 { get; set; }
        public StreamInfo Stream3 { get; set; }
        public StreamInfo Stream4 { get; set; }
        public long? UpdateTime { get; set; }
        public string Vendor { get; set; }
        public int? VideoCodec { get; set; }
        public string VideoMode { get; set; }
        public string VolumeSpace { get; set; }
    }
}

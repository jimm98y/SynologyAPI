namespace SynologyAPI.Model
{
    public class LiveStream
    {
        public int Id { get; set; }
        public string MjpegHttpPath { get; set; }
        public string MulticastPath { get; set; }
        public string MxpegHttpPath { get; set; }
        public string RtspOverHttpPath { get; set; }
        public string RtspPath { get; set; }
    }
}

namespace SynologyAPI.Model
{
    public class Recording
    {
        public int? AudioCodec { get; set; }
        public int? VideoCodec { get; set; }
        public int? Width { get; set; }
        public string FilePath { get; set; }
        public int Id { get; set; }
        public string CameraName { get; set; }
        public int? CameraId { get; set; }
        public int? SizeByte { get; set; }
        public int? Height { get; set; }
    }
}

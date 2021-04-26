namespace Core.Domain
{
    public abstract class FileBase
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
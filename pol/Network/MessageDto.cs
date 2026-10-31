namespace pol.Network
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Topic { get; set; }
        public long TimestampUnix { get; set; }
        public int MessageType { get; set; }

        // Binary data converted to Base64 strings for JSON transport
        public string CiphertextBase64 { get; set; }
        public string NonceBase64 { get; set; } // Can be null for plain text
        public string SignatureBase64 { get; set; }
    }
}
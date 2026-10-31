using System;

namespace pol.Core.Models
{
    public class NetMessage
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Topic { get; set; }
        public long TimestampUnix { get; set; }

        // NEW: 0 = Encrypted, 1 = Plain Text
        public int MessageType { get; set; }

        public byte[] Ciphertext { get; set; }
        public byte[] Nonce { get; set; } // Can be null if MessageType == 1
        public byte[] Signature { get; set; }
    }
}
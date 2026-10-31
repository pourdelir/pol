using System;

namespace Pol.Core.Models
{
    public class Message
    {
        // Primary Key
        public Guid Id { get; set; }

        // Metadata (Plaintext)
        public string SenderId { get; set; } // Hash of sender public key
        public string ReceiverId { get; set; } // Hash of recipient public key
        public string Topic { get; set; }
        public long TimestampUnix { get; set; } // Unix time in milliseconds/seconds

        // Encrypted Payload
        public byte[] Ciphertext { get; set; }
        public byte[] Nonce { get; set; }

        // Signature
        public byte[] Signature { get; set; }
    }
}
using Sodium;

namespace pol.Crypto
{
    public class UserIdentity
    {
        // Identification
        public string UserId { get; set; } // Hash of the Ed25519 Public Key

        // Signing Keys (Ed25519)
        public byte[] SigningPrivateKey { get; set; }
        public byte[] SigningPublicKey { get; set; }

        // Encryption Keys (X25519)
        public byte[] EncryptionPrivateKey { get; set; }
        public byte[] EncryptionPublicKey { get; set; }
    }
}
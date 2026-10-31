using Sodium;
using System;
using System.Security.Cryptography;
using System.Text;

namespace pol.Crypto
{
    public static class CryptoService
    {
        /// <summary>
        /// Generates a new Identity (Signing + Encryption keypairs).
        /// </summary>
        public static UserIdentity GenerateIdentity()
        {
            // 1. Generate Ed25519 Signing Keypair
            var signingKeyPair = PublicKeyAuth.GenerateKeyPair();

            // 2. Generate X25519 Encryption Keypair
            var encryptionKeyPair = PublicKeyBox.GenerateKeyPair();

            // 3. Derive UserId from Signing Public Key (SHA-256 hash)
            string userId = ComputeSha256Hash(signingKeyPair.PublicKey);

            return new UserIdentity
            {
                UserId = userId,
                SigningPrivateKey = signingKeyPair.PrivateKey,
                SigningPublicKey = signingKeyPair.PublicKey,
                EncryptionPrivateKey = encryptionKeyPair.PrivateKey,
                EncryptionPublicKey = encryptionKeyPair.PublicKey
            };
        }

        /// <summary>
        /// Encrypts a message for a specific receiver using X25519.
        /// </summary>
        public static (byte[] Ciphertext, byte[] Nonce) EncryptMessage(
            string plaintext,
            byte[] receiverEncryptionPublicKey,
            byte[] senderEncryptionPrivateKey)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(plaintext);

            // Generate a random Nonce for this specific message
            byte[] nonce = PublicKeyBox.GenerateNonce();

            // Encrypt: Requires sender private key and receiver public key
            byte[] ciphertext = PublicKeyBox.Create(messageBytes, nonce, senderEncryptionPrivateKey, receiverEncryptionPublicKey);

            return (ciphertext, nonce);
        }

        /// <summary>
        /// Decrypts a message using X25519.
        /// </summary>
        public static string DecryptMessage(
            byte[] ciphertext,
            byte[] nonce,
            byte[] senderEncryptionPublicKey,
            byte[] receiverEncryptionPrivateKey)
        {
            try
            {
                // Decrypt: Requires receiver private key and sender public key
                byte[] decryptedBytes = PublicKeyBox.Open(ciphertext, nonce, receiverEncryptionPrivateKey, senderEncryptionPublicKey);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception)
            {
                return null; // Failed to decrypt (wrong keys or corrupted data)
            }
        }

        /// <summary>
        /// Signs data using Ed25519.
        /// </summary>
        public static byte[] SignData(byte[] data, byte[] signingPrivateKey)
        {
            return PublicKeyAuth.Sign(data, signingPrivateKey);
        }

        /// <summary>
        /// Verifies signature using Ed25519.
        /// </summary>
        public static bool VerifySignature(byte[] data, byte[] signature, byte[] signingPublicKey)
        {
            try
            {
                return PublicKeyAuth.Verify(signature, data, signingPublicKey);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Helper to create a readable ID from a public key
        private static string ComputeSha256Hash(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes); // Simple hex string for ID
            }
        }
    }
}
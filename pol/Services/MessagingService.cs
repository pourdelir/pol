using pol.Core.Models; // Note: 'pol' is lowercase
using pol.Crypto;      // Note: 'pol' is lowercase
using System;
using System.Text;

namespace pol.Services // Note: 'pol' is lowercase
{
    public class MessagingService
    {
        /// <summary>
        /// Creates a message object ready to be saved to the database.
        /// </summary>
        public static NetMessage CreateMessage(
            string topic,
            string body,
            string receiverPublicKeyHex,
            int messageType,
            UserIdentity senderIdentity)
        {
            // 1. Generate a unique ID for this message
            var messageId = Guid.NewGuid();

            // 2. Prepare the Receiver ID
            // We hash the receiver's public key to get their ID
            byte[] receiverPublicKeyBytes = Convert.FromHexString(receiverPublicKeyHex);
            string receiverId = CryptoService.ComputeSha256Hash(receiverPublicKeyBytes);

            // 3. Handle Content (Encryption vs Plain)
            byte[] ciphertext;
            byte[] nonce = null;

            if (messageType == 0) // Encrypted
            {
                // Encrypt body for the receiver
                var result = CryptoService.EncryptMessage(
                    body,
                    receiverPublicKeyBytes,
                    senderIdentity.EncryptionPrivateKey);

                ciphertext = result.Ciphertext;
                nonce = result.Nonce;
            }
            else // Type 1: Public / Plain Text
            {
                // Just convert string to bytes, no encryption
                ciphertext = Encoding.UTF8.GetBytes(body);
                // Nonce remains null for plain text
            }

            // 4. Sign the Payload (The Ciphertext)
            byte[] signature = CryptoService.SignData(ciphertext, senderIdentity.SigningPrivateKey);

            // 5. Construct the NetMessage Object
            return new NetMessage
            {
                Id = messageId,
                SenderId = senderIdentity.UserId,
                ReceiverId = receiverId,
                Topic = topic,
                TimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                MessageType = messageType,
                Ciphertext = ciphertext,
                Nonce = nonce,
                Signature = signature
            };
        }
        // Add this method to MessagingService.cs

        /// <summary>
        /// Reads, verifies, and decrypts a message.
        /// </summary>
        /// <param name="message">The message object from DB</param>
        /// <param name="recipientIdentity">YOUR identity (to decrypt)</param>
        /// <param name="senderSigningPublicKey">Sender's Ed25519 key (to verify signature)</param>
        /// <param name="senderEncryptionPublicKey">Sender's X25519 key (to decrypt)</param>
        public static string ReadMessage(
            NetMessage message,
            UserIdentity recipientIdentity,
            byte[] senderSigningPublicKey,
            byte[] senderEncryptionPublicKey)
        {
            // 1. Verify Signature (Ensure message isn't tampered with)
            bool isValid = CryptoService.VerifySignature(
                message.Ciphertext,
                message.Signature,
                senderSigningPublicKey);

            if (!isValid)
            {
                return "⚠️ SECURITY ALERT: Invalid Signature!";
            }

            // 2. Decrypt or Read Plain
            if (message.MessageType == 1) // Plain Text
            {
                return System.Text.Encoding.UTF8.GetString(message.Ciphertext);
            }
            else // Encrypted
            {
                string decryptedText = CryptoService.DecryptMessage(
                    message.Ciphertext,
                    message.Nonce,
                    senderEncryptionPublicKey,
                    recipientIdentity.EncryptionPrivateKey);

                if (decryptedText == null)
                    return "⚠️ Decryption Failed (Wrong Keys?)";

                return decryptedText;
            }
        }
    }
}
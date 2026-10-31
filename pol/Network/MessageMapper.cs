using pol.Core.Models;
using System;
using System.Linq;

namespace pol.Network
{
    public static class MessageMapper
    {
        public static MessageDto ToDto(NetMessage msg)
        {
            return new MessageDto
            {
                Id = msg.Id.ToString(),
                SenderId = msg.SenderId,
                ReceiverId = msg.ReceiverId,
                Topic = msg.Topic,
                TimestampUnix = msg.TimestampUnix,
                MessageType = msg.MessageType,
                CiphertextBase64 = Convert.ToBase64String(msg.Ciphertext),
                NonceBase64 = msg.Nonce != null ? Convert.ToBase64String(msg.Nonce) : null,
                SignatureBase64 = Convert.ToBase64String(msg.Signature)
            };
        }

        public static NetMessage FromDto(MessageDto dto)
        {
            return new NetMessage
            {
                Id = Guid.Parse(dto.Id),
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                Topic = dto.Topic,
                TimestampUnix = dto.TimestampUnix,
                MessageType = dto.MessageType,
                Ciphertext = Convert.FromBase64String(dto.CiphertextBase64),
                Nonce = !string.IsNullOrEmpty(dto.NonceBase64) ? Convert.FromBase64String(dto.NonceBase64) : null,
                Signature = Convert.FromBase64String(dto.SignatureBase64)
            };
        }
    }
}
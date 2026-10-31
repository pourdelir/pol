// MessageRepository.cs
using Microsoft.Data.Sqlite;
using Pol.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Pol.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DatabaseContext _context;

        public MessageRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(Message message)
        {
            var conn = _context.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO Messages (Id, SenderId, ReceiverId, Topic, TimestampUnix, Ciphertext, Nonce, Signature)
                VALUES (@id, @sender, @receiver, @topic, @ts, @cipher, @nonce, @sig);
            ";

            cmd.Parameters.AddWithValue("@id", message.Id.ToString());
            cmd.Parameters.AddWithValue("@sender", message.SenderId);
            cmd.Parameters.AddWithValue("@receiver", message.ReceiverId);
            cmd.Parameters.AddWithValue("@topic", message.Topic);
            cmd.Parameters.AddWithValue("@ts", message.TimestampUnix);
            cmd.Parameters.AddWithValue("@cipher", message.Ciphertext);
            cmd.Parameters.AddWithValue("@nonce", message.Nonce);
            cmd.Parameters.AddWithValue("@sig", message.Signature);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            var list = new List<Message>();
            var conn = _context.GetConnection();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Messages";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapFromReader(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(long timestampUnix)
        {
            var list = new List<Message>();
            var conn = _context.GetConnection();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Messages WHERE TimestampUnix > @ts ORDER BY TimestampUnix ASC";
            cmd.Parameters.AddWithValue("@ts", timestampUnix);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapFromReader(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Message>> GetMessagesForUserAsync(string userId)
        {
            var list = new List<Message>();
            var conn = _context.GetConnection();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Messages WHERE ReceiverId = @receiverId";
            cmd.Parameters.AddWithValue("@receiverId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapFromReader(reader));
            }
            return list;
        }

        private Message MapFromReader(SqliteDataReader reader)
        {
            return new Message
            {
                Id = Guid.Parse(reader.GetString(0)),
                SenderId = reader.GetString(1),
                ReceiverId = reader.GetString(2),
                Topic = reader.GetString(3),
                TimestampUnix = reader.GetInt64(4),
                Ciphertext = (byte[])reader[5],
                Nonce = (byte[])reader[6],
                Signature = (byte[])reader[7]
            };
        }
    }
}
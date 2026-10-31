using Microsoft.Data.Sqlite;
using pol.Core.Models;
using Pol.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace pol.Data.Repositories
{
    public class NetMessageRepository : INetMessageRepository
    {
        private readonly DatabaseContext _context;

        public NetMessageRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(NetMessage message)
        {
            var conn = _context.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
        INSERT OR IGNORE INTO Messages 
        (Id, SenderId, ReceiverId, Topic, TimestampUnix, MessageType, Ciphertext, Nonce, Signature)
        VALUES (@id, @sender, @receiver, @topic, @ts, @type, @cipher, @nonce, @sig);
    ";
            // Changed INSERT to INSERT OR IGNORE

            cmd.Parameters.AddWithValue("@id", message.Id.ToString());
            // ... (rest of parameters same as before) ...
            cmd.Parameters.AddWithValue("@sender", message.SenderId);
            cmd.Parameters.AddWithValue("@receiver", message.ReceiverId);
            cmd.Parameters.AddWithValue("@topic", message.Topic);
            cmd.Parameters.AddWithValue("@ts", message.TimestampUnix);
            cmd.Parameters.AddWithValue("@type", message.MessageType);
            cmd.Parameters.AddWithValue("@cipher", message.Ciphertext);
            cmd.Parameters.AddWithValue("@nonce", message.Nonce);
            cmd.Parameters.AddWithValue("@sig", message.Signature);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<NetMessage>> GetAllMessagesAsync()
        {
            var list = new List<NetMessage>();
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

        // ... Keep GetMessagesSinceAsync and GetMessagesForUserAsync ...
        // Just change the return type and MapFromReader calls.

        public async Task<IEnumerable<NetMessage>> GetMessagesSinceAsync(long timestampUnix, int limit)
        {
            var list = new List<NetMessage>();
            var conn = _context.GetConnection();

            using var cmd = conn.CreateCommand();
            // We add LIMIT to the SQL query
            cmd.CommandText = "SELECT * FROM Messages WHERE TimestampUnix > @ts ORDER BY TimestampUnix ASC LIMIT @limit";

            cmd.Parameters.AddWithValue("@ts", timestampUnix);
            cmd.Parameters.AddWithValue("@limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapFromReader(reader));
            }
            return list;
        }

        public async Task<IEnumerable<NetMessage>> GetMessagesSinceAsync(long timestampUnix)
        {
            var list = new List<NetMessage>();
            var conn = _context.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Messages WHERE TimestampUnix > @ts ORDER BY TimestampUnix ASC";
            cmd.Parameters.AddWithValue("@ts", timestampUnix);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapFromReader(reader));
            return list;
        }

        public async Task<IEnumerable<NetMessage>> GetMessagesForUserAsync(string userId)
        {
            var list = new List<NetMessage>();
            var conn = _context.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Messages WHERE ReceiverId = @receiverId";
            cmd.Parameters.AddWithValue("@receiverId", userId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(MapFromReader(reader));
            return list;
        }

        private NetMessage MapFromReader(SqliteDataReader reader)
        {
            return new NetMessage
            {
                Id = Guid.Parse(reader.GetString(0)),
                SenderId = reader.GetString(1),
                ReceiverId = reader.GetString(2),
                Topic = reader.GetString(3),
                TimestampUnix = reader.GetInt64(4),
                MessageType = reader.GetInt32(5), // Index 5 for MessageType
                Ciphertext = (byte[])reader[6],
                Nonce = (byte[])reader[7],
                Signature = (byte[])reader[8]
            };
        }
    }
}
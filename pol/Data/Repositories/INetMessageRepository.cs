using pol.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pol.Data.Repositories
{
    public interface INetMessageRepository
    {
        Task AddMessageAsync(NetMessage message);
        Task<IEnumerable<NetMessage>> GetAllMessagesAsync();
        Task<IEnumerable<NetMessage>> GetMessagesSinceAsync(long timestampUnix);
        Task<IEnumerable<NetMessage>> GetMessagesForUserAsync(string userId);
        Task<IEnumerable<NetMessage>> GetMessagesSinceAsync(long timestampUnix, int limit);
    }
}
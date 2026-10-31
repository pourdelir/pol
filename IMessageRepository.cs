// IMessageRepository.cs
using Pol.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pol.Data.Repositories
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<IEnumerable<Message>> GetMessagesSinceAsync(long timestampUnix);
        Task<IEnumerable<Message>> GetMessagesForUserAsync(string userId);
    }
}
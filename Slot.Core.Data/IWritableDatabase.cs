using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Slot.Core.Data
{
    public interface IWritableDatabase : IReadOnlyDatabase
    {
        void InsertOrUpdate<T>(T entity, params object[] id) where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        DbContext Context { get; }
    }
}

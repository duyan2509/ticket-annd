using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Infrastructure.Persistence.Mongo;

public class TicketLogRepository : ITicketLogRepository
{
    private readonly IMongoDatabase _database;

    public TicketLogRepository(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task InsertAsync(TicketLogDocument doc, CancellationToken cancellationToken = default)
    {
        var col = _database.GetCollection<TicketLogDocument>("ticket_logs");
        await col.InsertOneAsync(doc, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<TicketLogDocument>> GetByTicketAsync(Guid ticketId, int page = 1, int size = 50, CancellationToken cancellationToken = default)
    {
        var col = _database.GetCollection<TicketLogDocument>("ticket_logs");
        var filter = Builders<TicketLogDocument>.Filter.Eq(x => x.TicketId, ticketId);
        var sort = Builders<TicketLogDocument>.Sort.Descending(x => x.Timestamp);
        var skip = (Math.Max(1, page) - 1) * Math.Max(1, size);
        var cursor = await col.Find(filter).Sort(sort).Skip(skip).Limit(size).ToListAsync(cancellationToken);
        return cursor;
    }
}

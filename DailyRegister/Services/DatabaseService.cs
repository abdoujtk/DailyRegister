using SQLite;
using DailyRegister.Models;
using Contact = DailyRegister.Models.Contact;

namespace DailyRegister.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "dailyregister.db3"
        );
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database is not null)
            return _database;

        _database = new SQLiteAsyncConnection(_dbPath);

        // Create tables if they don't exist
        await _database.CreateTableAsync<Models.Contact>();
        await _database.CreateTableAsync<AppEvent>();

        return _database;
    }

    // ==================== CONTACTS ====================

    public async Task<List<Contact>> GetContactsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Contact>()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Contact?> GetContactAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Contact>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> SaveContactAsync(Contact contact)
    {
        var db = await GetDatabaseAsync();
        contact.UpdatedAt = DateTime.Now;

        if (contact.Id != 0)
            return await db.UpdateAsync(contact);
        else
        {
            contact.CreatedAt = DateTime.Now;
            return await db.InsertAsync(contact);
        }
    }

    public async Task<int> DeleteContactAsync(Contact contact)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync(contact);
    }

    // ==================== EVENTS ====================

    public async Task<List<AppEvent>> GetEventsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<AppEvent>()
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<AppEvent?> GetEventAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<AppEvent>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> SaveEventAsync(AppEvent appEvent)
    {
        var db = await GetDatabaseAsync();
        appEvent.UpdatedAt = DateTime.Now;

        if (appEvent.Id != 0)
            return await db.UpdateAsync(appEvent);
        else
        {
            appEvent.CreatedAt = DateTime.Now;
            return await db.InsertAsync(appEvent);
        }
    }

    public async Task<int> DeleteEventAsync(AppEvent appEvent)
    {
        var db = await GetDatabaseAsync();
        return await db.DeleteAsync(appEvent);
    }
}
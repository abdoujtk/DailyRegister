using SQLite;

namespace DailyRegister.Models;

public class Contact
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(255), NotNull]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
using SQLite;

namespace DailyRegister.Models;

public class AppEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(50), NotNull]
    public string Type { get; set; } = string.Empty;
    // Values: "debt", "deposit", "expense", "product_entry", "general_note"

    public int? ContactId { get; set; }
    // Nullable — not all events link to a contact

    public decimal? Amount { get; set; }
    // Nullable — general notes and product entries may not have amounts

    [NotNull]
    public string Description { get; set; } = string.Empty;
    // Required for all events (required only for expenses, but we make it required for all to keep it simple)

    [MaxLength(20)]
    public string? Status { get; set; }
    // "unpaid" or "paid" — only used for debts

    public string? StatusNote { get; set; }
    // Note when marking a debt as paid

    public DateTime EventDate { get; set; } = DateTime.Now;
    // The date the event happened

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    // When the record was created in the system

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    // Last updated

    [Ignore]
    public string? ContactName { get; set; }
}
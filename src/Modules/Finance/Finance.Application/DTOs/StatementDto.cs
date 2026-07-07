using System;
using System.Collections.Generic;

namespace Finance.Application.DTOs
{
    // Cari Hesap Ekstresi (running account statement) — a single chronological
    // line, either a debit (an invoice raising what's owed) or a credit (a
    // payment reducing it), with the running balance after that line.
    public record StatementEntryDto(
        DateTime Date,
        string Type,        // "Invoice" | "Payment"
        string Description,
        decimal Debit,
        decimal Credit,
        decimal Balance
    );

    public record StatementDto(
        List<StatementEntryDto> Entries,
        decimal TotalDebit,
        decimal TotalCredit,
        decimal Balance
    );
}

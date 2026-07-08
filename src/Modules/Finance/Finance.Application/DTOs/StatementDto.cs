using System;
using System.Collections.Generic;

namespace Finance.Application.DTOs
{

    public record StatementEntryDto(
        DateTime Date,
        string Type,        
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

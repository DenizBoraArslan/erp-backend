using MediatR;
using Common.Results;

namespace Account.Application.Features.ChartOfAccounts.Commands;

public class CreateChartOfAccountCommand : IRequest<Result<CreateChartOfAccountResponse>>
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public int Type { get; set; }
}

public class CreateChartOfAccountCommandHandler : IRequestHandler<CreateChartOfAccountCommand, Result<CreateChartOfAccountResponse>>
{
    public async Task<Result<CreateChartOfAccountResponse>> Handle(CreateChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern
        await Task.CompletedTask;
        return Result.Failure<CreateChartOfAccountResponse>("Not implemented yet");
    }
}

public class CreateChartOfAccountResponse
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
}

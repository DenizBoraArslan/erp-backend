using MediatR;
using Common.Results;
using Common.Interfaces;
using Account.Domain.Entities;

namespace Account.Application.Features.ChartOfAccounts.Commands;

public class CreateChartOfAccountCommand : IRequest<Result<CreateChartOfAccountResponse>>
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public int Type { get; set; }
}

public class CreateChartOfAccountCommandHandler : IRequestHandler<CreateChartOfAccountCommand, Result<CreateChartOfAccountResponse>>
{
    private readonly IRepository<ChartOfAccount> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChartOfAccountCommandHandler(IRepository<ChartOfAccount> accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateChartOfAccountResponse>> Handle(CreateChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var account = new ChartOfAccount
            {
                AccountCode = request.AccountCode,
                AccountName = request.AccountName,
                Type = (AccountType)request.Type,
                Balance = 0,
                IsActive = true
            };

            await _accountRepository.AddAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateChartOfAccountResponse
            {
                Id = account.Id,
                AccountCode = account.AccountCode
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<CreateChartOfAccountResponse>(ex.Message);
        }
    }
}

public class CreateChartOfAccountResponse
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
}

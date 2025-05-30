

using Service.Models;

namespace Service.Interfaces;

public interface IAccountService
{
    Task<Result> CreateAccountAsync(AccountDto dto);
    Task<Result<AccountModel>> GetAccountAsync(Guid userId);
}

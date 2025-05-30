

using Data.Contexts;
using Service.Factories;
using Service.Interfaces;
using Service.Models;

namespace Service.Services;

public class AccountService(SqlServerDbContext db) : IAccountService
{
    private readonly SqlServerDbContext _db = db;

    public async Task<Result> CreateAccountAsync(AccountDto dto)
    {
        try
        {
            var entity = AccountFactory.Create(dto);
            _db.Accounts.Add(entity);
            _db.SaveChanges();

            return new Result { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            return new Result { Success = false, ErrorMessage = ex.Message };
        }

        return new Result();
    }

    public async Task<Result<AccountModel>> GetAccountAsync(Guid userId)
    {
        try
        {
            var result = _db.Accounts.Where(x => x.Id == userId).FirstOrDefault();

            return result is null
                ? new Result<AccountModel> { Success = false, ErrorMessage = "The user does not exist" }
                : new Result<AccountModel> { Success = true, Data = AccountFactory.Create(result) };
        }
        catch (Exception ex)
        {
            return new Result<AccountModel> { Success = false, ErrorMessage=ex.Message };
        }
    }
}

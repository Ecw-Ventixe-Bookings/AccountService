

using Data.Entities;
using Service.Models;
using System.Runtime.InteropServices;

namespace Service.Factories;

public static class AccountFactory
{
    public static AccountDto Create() => new AccountDto();

    public static AccountEntity Create(AccountDto dto) =>
        new AccountEntity
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            PostalCode = dto.PostalCode,
            City = dto.City
        };

    public static AccountModel Create(AccountEntity entity) =>
        new AccountModel
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Address = entity.Address,
            PostalCode = entity.PostalCode,
            City = entity.City
        };
}

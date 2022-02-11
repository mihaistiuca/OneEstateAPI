using OneEstate.Application.Dtos;
using OneEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneEstate.Domain.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string id);

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByResetTokenAsync(string resetToken);

        Task<List<User>> GetInProgressValidationUsers();

        Task<bool> ResetPasswordAsync(string userId, string newPassword);

        Task<string> CreateAsync(UserRegisterInput input, Guid guid);

        Task<User> AuthenticateAsync(string email, string password);

        Task<bool> ConfirmAccountAsync(string guid);

        Task<bool> DeleteAsync(string id);

        Task<bool> SetResetPasswordInformationAsync(string userId, string resetToken);

        Task<bool> IsEmailUnique(string email, string id);

        Task<bool> ValidateUserChangePhoneNumber(string userId, ValidateUserChangePhoneInput input);

        Task<bool> ValidateUserChangeUserId(string userId, ValidateUserChangeIdImageIdInput input);
    }
}

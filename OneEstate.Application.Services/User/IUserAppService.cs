using OneEstate.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Application.Services
{
    public interface IUserAppService
    {
        Task<UserBasicInfoDto> GetBasicInfoById(string id);

        Task<List<UserBasicInfoDto>> GetInProgressValidationUsers();

        Task<bool> RegisterAsync(UserRegisterInput input);

        Task<UserBasicInfoDto> AuthenticateAsync(UserLoginInput input);

        Task<bool> IsEmailUnique(string email, string id);

        Task<bool> SendResetPasswordEmailAsync(ResetPasswordSendEmailInput input);

        Task<bool> ResetPasswordAsync(ResetPasswordInput input);

        Task<bool> ConfirmAccountAsync(string guid);

        Task<bool> ValidateUserChangePhoneNumber(string userId, ValidateUserChangePhoneInput input);

        Task<bool> ValidateUserChangeUserIdImage(string userId, ValidateUserChangeIdImageIdInput input);
    }
}

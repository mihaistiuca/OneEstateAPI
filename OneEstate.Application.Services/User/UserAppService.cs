using AutoMapper;
using OneEstate.Application.Dtos;
using OneEstate.Domain.Services;
using Resources.Base.Exception;
using Resources.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public UserAppService(IUserService userService, IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }

        private static readonly List<string> adminsEmails = new List<string>
        {
            "stiuca.mihai@yahoo.com",
            "cristian.cirstocea@yahoo.ro",
            "cristian.cirstocea7@gmail.com",
            "cristina.alexandru21@yahoo.com",
            "office@weveed.com",
        };

        #region GET methods 

        public async Task<UserBasicInfoDto> GetBasicInfoById(string id)
        {
            var entity = await _userService.GetByIdAsync(id);

            var dto = Mapper.Map<UserBasicInfoDto>(entity);
            dto.IsAdmin = adminsEmails.Contains(dto.Email);

            if (string.IsNullOrEmpty(dto.UserIdValidationStatus))
            {
                dto.UserIdValidationStatus = "notvalidated";
            }

            if (string.IsNullOrEmpty(dto.PhoneNumberValidationStatus))
            {
                dto.PhoneNumberValidationStatus = "notvalidated";
            }

            return dto;
        }

        public async Task<List<UserBasicInfoDto>> GetInProgressValidationUsers()
        {
            var users = await _userService.GetInProgressValidationUsers();

            if (users == null || !users.Any())
            {
                return new List<UserBasicInfoDto>();
            }

            return users.Select(a => Mapper.Map<UserBasicInfoDto>(a)).ToList();
        }

        #endregion

        #region Login and Register 

        public async Task<bool> RegisterAsync(UserRegisterInput input)
        {
            var registerGuid = Guid.NewGuid();
            var id = await _userService.CreateAsync(input, registerGuid);

            // IMPORTANT SEND EMAIL 
            var confirmationEmailSent = _emailSender.SendRegisterConfirmationEmail(registerGuid, input.Email, input.FirstName);

            if (!confirmationEmailSent)
            {
                await _userService.DeleteAsync(id);
                return false;
            }

            return true;
        }

        public async Task<UserBasicInfoDto> AuthenticateAsync(UserLoginInput input)
        {
            var entity = await _userService.AuthenticateAsync(input.Email, input.Password);
            if (entity == null)
            {
                throw new HttpStatusCodeException(401, new System.Collections.Generic.List<string> { "Adresa de email sau parola incorecte." });
            }
            if (!entity.IsActive)
            {
                throw new HttpStatusCodeException(401, new System.Collections.Generic.List<string> { "Contul tau nu a fost activat. Confirma contul pentru a te putea conecta." });
            }

            return await GetBasicInfoById(entity.Id.ToString());
        }

        public async Task<bool> ConfirmAccountAsync(string guid)
        {
            return await _userService.ConfirmAccountAsync(guid);
        }

        #endregion

        #region Reset Password

        public async Task<bool> SendResetPasswordEmailAsync(ResetPasswordSendEmailInput input)
        {
            var user = await _userService.GetByEmailAsync(input.Email);
            if (user == null)
            {
                return false;
            }

            var resetToken = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            // set on the user object the reset token and date 
            var wasResetInfoSet = await _userService.SetResetPasswordInformationAsync(user.Id.ToString(), resetToken);
            if (!wasResetInfoSet)
            {
                return false;
            }

            var emailSent = _emailSender.SendEmailToResetPassword(input.Email, resetToken, user.FirstName + " " + user.LastName);

            return emailSent;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordInput input)
        {
            // validate the reset token + email combination
            var user = await _userService.GetByResetTokenAsync(input.ResetToken);
            if (user == null)
            {
                return false;
            }

            if (!user.IsResetTokenActive)
            {
                return false;
            }

            if ((DateTime.Now - user.ResetDate).TotalHours > 2)
            {
                return false;
            }

            // if it got here, the reset token is valid, so change the password 
            var wasPasswordReset = await _userService.ResetPasswordAsync(user.Id.ToString(), input.NewPassword);

            return wasPasswordReset;
        }

        #endregion

        #region Unicity Validators 

        public async Task<bool> IsEmailUnique(string email, string id)
        {
            return await _userService.IsEmailUnique(email, id);
        }

        #endregion

        #region ChangeAccountValidation

        public async Task<bool> ValidateUserChangePhoneNumber(string userId, ValidateUserChangePhoneInput input)
        {
            var isSuccess = await _userService.ValidateUserChangePhoneNumber(userId, input);

            if (isSuccess)
            {
                var user = await _userService.GetByIdAsync(userId);

                // send email to admins that a user has completed the phone number verification 
                var wasEmailSent = _emailSender.SendUserCompletedPhoneNumberVerifEmail(userId, user.FirstName + " " + user.LastName, input.PhoneNumber);
            }

            return isSuccess;
        }

        public async Task<bool> ValidateUserChangeUserIdImage(string userId, ValidateUserChangeIdImageIdInput input)
        {
            var isSuccess = await _userService.ValidateUserChangeUserId(userId, input);

            if (isSuccess)
            {
                var user = await _userService.GetByIdAsync(userId);

                // send email to admins that a user has completed the phone number verification 
                var wasEmailSent = _emailSender.SendUserCompletedUserImageIdVerifEmail(userId, user.FirstName + " " + user.LastName, input.ImageId);
            }

            return isSuccess;
        }


        #endregion
    }
}

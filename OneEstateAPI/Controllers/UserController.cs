using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneEstate.Application.Services;
using OneEstate.Application.Dtos;
using Resources.Base.Responses;
using OneEstateAPI.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Resources.Base.SettingsModels;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using Resources.Base.AuthUtils;
using System.Linq;

namespace OneEstate.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserAppService _userAppService;
        private readonly IOptions<AppGeneralSettings> _appGeneralSettings;

        public UserController(IUserAppService userAppService, IOptions<AppGeneralSettings> appGeneralSettings)
        {
            _userAppService = userAppService;
            _appGeneralSettings = appGeneralSettings;
        }

        private static readonly List<string> adminsEmails = new List<string>
        {
            "stiuca.mihai@yahoo.com",
            "cristian.cirstocea@yahoo.ro",
            "cristian.cirstocea7@gmail.com",
            "cristina.alexandru21@yahoo.com",
            "office@weveed.com",
        };

        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> Get(string id)
        {
            var a = Request.Headers;
            var x = User.Claims;
            return new string[] { "111111111", "22222" };
        }

        #region GET methods 

        [Authorize]
        [HttpGet("getUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            var user = await _userAppService.GetBasicInfoById(id);

            return Response.Ok(new BaseResponse<UserBasicInfoDto>(user));
        }

        #endregion

        #region Login and Register 

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterInput input)
        {
            var userRegistered = await _userAppService.RegisterAsync(input);
            var response = new BaseResponse(userRegistered);
            return Response.Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginInput input)
        {
            var authenticationResult = await _userAppService.AuthenticateAsync(input);

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _appGeneralSettings.Value.Issuer,
                audience: _appGeneralSettings.Value.Audience,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds,
                claims: new List<Claim>
                {
                    new Claim(AppClaims.UserId, authenticationResult.Id)
                });

            var newTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

            authenticationResult.Token = newTokenString;
            authenticationResult.IsAdmin = adminsEmails.Contains(authenticationResult.Email);

            var response = new BaseResponse<UserBasicInfoDto>(authenticationResult);

            return Response.Ok(response);
        }

        #endregion

        #region Reset Password

        // send an email to the email address to change the password 
        [HttpPost("resetPasswordSendEmail")]
        public async Task<IActionResult> ResetPasswordSendEmail([FromBody] ResetPasswordSendEmailInput input)
        {
            var wasEmailSent = await _userAppService.SendResetPasswordEmailAsync(input);
            return Response.Ok(new BaseResponse(wasEmailSent));
        }

        // reset the password
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordInput input)
        {
            var wasPasswordReset = await _userAppService.ResetPasswordAsync(input);
            return Response.Ok(new BaseResponse(wasPasswordReset));
        }

        [HttpPost("confirmAccount")]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountInput input)
        {
            var userConfirmed = await _userAppService.ConfirmAccountAsync(input.Code);
            var response = new BaseResponse(userConfirmed);
            return Response.Ok(response);
        }

        #endregion

        #region Validate Account

        [HttpGet("adminGetAllInProgressValidationUsers")]
        public async Task<IActionResult> AdminGetAllInProgressValidationUsers()
        {
            var currentUserId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (currentUserId == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var currentUserEntity = await _userAppService.GetBasicInfoById(currentUserId);

            if (!adminsEmails.Any(a => a == currentUserEntity.Email.Trim().ToLower()))
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var users = await _userAppService.GetInProgressValidationUsers();

            return Response.Ok(new BaseResponse<List<UserBasicInfoDto>>(users));
        }

        [HttpPost("validateAccountSendPhoneNumber")]
        public async Task<IActionResult> ValidateAccountSendPhoneNumber([FromBody] ValidateUserChangePhoneInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            if (input.PhoneNumber.Length > 25)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var isSuccess = await _userAppService.ValidateUserChangePhoneNumber(id, input);

            return Response.Ok(new BaseResponse(isSuccess));
        }

        [HttpPost("validateAccountSendImageId")]
        public async Task<IActionResult> ValidateAccountSendImageId([FromBody] ValidateUserChangeIdImageIdInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.Ok(new BaseResponse(false));
            }

            if (input.ImageId.Length > 50)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var isSuccess = await _userAppService.ValidateUserChangeUserIdImage(id, input);

            return Response.Ok(new BaseResponse(isSuccess));
        }

        #endregion
    }
}

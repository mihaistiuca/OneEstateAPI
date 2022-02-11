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
using Resources.Base.Exception;
using System.Net.Http;
using OneEstateAPI.Utils;

namespace OneEstate.Controllers
{
    [Route("[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectAppService _projectAppService;
        private readonly IUserAppService _userAppService;
        private readonly IOptions<AppGeneralSettings> _appGeneralSettings;

        public ProjectController(IProjectAppService projectAppService, IOptions<AppGeneralSettings> appGeneralSettings, IUserAppService userAppService)
        {
            _projectAppService = projectAppService;
            _userAppService = userAppService;
            _appGeneralSettings = appGeneralSettings;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> Get(string id)
        {
            var a = Request.Headers;
            var x = User.Claims;
            return new string[] { "111111111", "22222" };
        }

        #region CRUD

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateProjectInput input)
        {
            var projectCreated = await _projectAppService.CreateAsync(input);
            var response = new BaseResponse<string>(projectCreated);
            return Response.Ok(response);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateProjectInput input)
        {
            var wasProjectUpdated = await _projectAppService.UpdateAsync(input.Id, input);
            var response = new BaseResponse(wasProjectUpdated);
            return Response.Ok(response);
        }

        // only performed by admin 
        [Authorize]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var currentUserId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (currentUserId == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var currentUserEntity = await _userAppService.GetBasicInfoById(currentUserId);

            if (!currentUserEntity.IsAdmin)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var result = await _projectAppService.GetAllAsync();

            return Response.Ok(new BaseResponse<List<ProjectViewDto>>(result));
        }

        // for the public main page list of projects 
        [HttpPost("getallpaginated")]
        public async Task<IActionResult> GetAllPaginated([FromBody] ProjectGetAllPaginatedInput input)
        {
            var result = await _projectAppService.GetActivePaginatedAsync(input.Skip, input.Limit);
            return Response.Ok(new BaseResponse<List<ProjectListViewDto>>(result));
        }

        // TODO IMPORTANT: Filter the ones inactive, only for admin 
        [HttpGet("getViewById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var projectDto = await _projectAppService.GetByIdAsync(id);
            var response = new BaseResponse<ProjectViewDto>(projectDto);
            return Response.Ok(response);
        }

        #endregion

        #region Statuses 

        // only performed by admin 
        [Authorize]
        [HttpPost("changestatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] ProjectChangeStatusInput input)
        {
            var currentUserId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (currentUserId == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var currentUserEntity = await _userAppService.GetBasicInfoById(currentUserId);

            if (!currentUserEntity.IsAdmin)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var result = await _projectAppService.ChangeStatusByAdmin(input.ProjectId, input.Status);
            return Response.Ok(new BaseResponse(result));
        }

        #endregion
    }
}

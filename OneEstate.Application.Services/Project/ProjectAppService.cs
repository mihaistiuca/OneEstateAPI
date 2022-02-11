using AutoMapper;
using OneEstate.Application.Dtos;
using OneEstate.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Application.Services
{
    public class ProjectAppService : IProjectAppService
    {
        private readonly IProjectService _projectService;

        public ProjectAppService(IProjectService projectService)
        {
            _projectService = projectService;
        }

        #region CRUD 

        public async Task<string> CreateAsync(CreateProjectInput input)
        {
            return await _projectService.CreateAsync(input);
        }

        public async Task<bool> UpdateAsync(string id, UpdateProjectInput input)
        {
            return await _projectService.UpdateAsync(id, input);
        }

        public async Task<ProjectViewDto> GetByIdAsync(string id)
        {
            var entity = await _projectService.GetByIdAsync(id);
            return Mapper.Map<ProjectViewDto>(entity);
        }

        // return projects for main page - for public display 
        public async Task<List<ProjectListViewDto>> GetActivePaginatedAsync(int? skip = null, int? limit = null)
        {
            var projects = await _projectService.GetActivePaginatedAsync(skip, limit);

            if (projects == null)
            {
                return new List<ProjectListViewDto>();
            }

            return projects.Select(a => Mapper.Map<ProjectListViewDto>(a)).ToList();
        }

        // return projects for main page - for public display 
        public async Task<List<ProjectViewDto>> GetAllAsync()
        {
            var projects = await _projectService.GetAllAsync();

            if (projects == null)
            {
                return new List<ProjectViewDto>();
            }

            return projects.Select(a => Mapper.Map<ProjectViewDto>(a)).ToList();
        }

        #endregion

        #region Status Change 

        public async Task<bool> ChangeStatusByAdmin(string projectId, string status)
        {
            return await _projectService.ChangeStatusByAdmin(projectId, status);
        }

        #endregion
    }
}

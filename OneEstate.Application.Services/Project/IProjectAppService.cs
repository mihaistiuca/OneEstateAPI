using OneEstate.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Application.Services
{
    public interface IProjectAppService
    {
        Task<string> CreateAsync(CreateProjectInput input);

        Task<bool> UpdateAsync(string id, UpdateProjectInput input);

        Task<ProjectViewDto> GetByIdAsync(string id);

        Task<List<ProjectListViewDto>> GetActivePaginatedAsync(int? skip = null, int? limit = null);

        Task<List<ProjectViewDto>> GetAllAsync();

        Task<bool> ChangeStatusByAdmin(string projectId, string status);
    }
}

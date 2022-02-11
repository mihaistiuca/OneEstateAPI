using OneEstate.Application.Dtos;
using OneEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Domain.Services
{
    public interface IProjectService
    {
        Task<string> CreateAsync(CreateProjectInput input);

        Task<bool> UpdateAsync(string id, UpdateProjectInput input);

        Task<Project> GetByIdAsync(string id);

        Task<List<Project>> GetActivePaginatedAsync(int? skip = 0, int? limit = 6);

        Task<List<Project>> GetAllAsync();

        Task<bool> ChangeStatusByAdmin(string projectId, string status);
    }
}

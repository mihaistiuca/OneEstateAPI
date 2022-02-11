using MongoDB.Bson;
using MongoDB.Driver;
using OneEstate.Application.Dtos;
using OneEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Domain.Services
{
    public class ProjectService : IProjectService
    {
        private IMongoCollection<Project> _projectCollection;

        const int TopCountRecent = 6;

        public ProjectService(IMongoDatabase mongoDatabase)
        {
            _projectCollection = mongoDatabase.GetCollection<Project>("project");
        }

        #region CRUD 

        public async Task<string> CreateAsync(CreateProjectInput input)
        {
            var dbProject = new Project
            {
                Name = input.Name,
                ProjectType = input.ProjectType,
                ShortDescription = input.ShortDescription,
                Description = input.Description,
                ImageIds = input.ImageIds,
                InvestmentDeadlineDate = input.InvestmentDeadlineDate,
                CompletionDate = input.CompletionDate,
                AmountGoal = input.AmountGoal,
                AmountMinGoal = input.AmountMinGoal,
                AmountInvested = 0,
                Status = ProjectStatus.Inactive,
                NumberOfInvestors = 0,
                EstimatedReturn = input.EstimatedReturn,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _projectCollection.InsertOneAsync(dbProject);

            return dbProject.Id.ToString();
        }

        public async Task<bool> UpdateAsync(string id, UpdateProjectInput input)
        {
            var filter = Builders<Project>.Filter.Eq(a => a.Id, new ObjectId(id));
            var update = Builders<Project>.Update.Set(a => a.Name, input.Name)
                                                .Set(a => a.ShortDescription, input.ShortDescription)
                                                .Set(a => a.Description, input.Description)
                                                .Set(a => a.ImageIds, input.ImageIds)
                                                .Set(a => a.InvestmentDeadlineDate, input.InvestmentDeadlineDate)
                                                .Set(a => a.CompletionDate, input.CompletionDate)
                                                .Set(a => a.AmountGoal, input.AmountGoal)
                                                .Set(a => a.AmountMinGoal, input.AmountMinGoal)
                                                .Set(a => a.EstimatedReturn, input.EstimatedReturn);

            var updateResult = await _projectCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<Project> GetByIdAsync(string id)
        {
            var filter = Builders<Project>.Filter.Eq(a => a.Id, new ObjectId(id));
            var project = await _projectCollection.Find(filter).FirstOrDefaultAsync();
            return project;
        }

        // return projects for main page - for public display 
        public async Task<List<Project>> GetActivePaginatedAsync(int? skip = 0, int? limit = TopCountRecent)
        {
            if (limit == 0)
            {
                limit = TopCountRecent;
            }

            var filter = Builders<Project>.Filter.Where(a => a.Status != ProjectStatus.Inactive);
            var sort = Builders<Project>.Sort.Ascending(a => a.Status).Descending(a => a.CreatedDate);
            var options = new FindOptions<Project>
            {
                Sort = sort,
                Skip = skip,
                Limit = limit
            };

            var projects = (await _projectCollection.FindAsync(filter, options)).ToList();
            return projects;
        }

        // for the admin page 
        public async Task<List<Project>> GetAllAsync()
        {
            var filter = Builders<Project>.Filter.Where(a => true);
            var projects = (await _projectCollection.FindAsync(filter)).ToList();
            return projects;
        }

        #endregion

        #region Status Change 

        public async Task<bool> ChangeStatusByAdmin(string projectId, string status)
        {
            var filter = Builders<Project>.Filter.Eq(a => a.Id, new ObjectId(projectId));
            var update = Builders<Project>.Update.Set(a => a.Status, status);

            var updateResult = await _projectCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        #endregion
    }
}

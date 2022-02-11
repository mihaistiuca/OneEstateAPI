using MongoDB.Bson;
using MongoDB.Driver;
using OneEstate.Application.Dtos;
using OneEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEstate.Domain.Services
{
    public class UserService : IUserService
    {
        private IMongoCollection<User> _userCollection;

        public UserService(IMongoDatabase mongoDatabase)
        {
            _userCollection = mongoDatabase.GetCollection<User>("user");
        }

        #region Get actions 

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userCollection.Find(a => true).ToListAsync();
            return users;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Email, email);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByResetTokenAsync(string resetToken)
        {
            var filter = Builders<User>.Filter.Eq(a => a.ResetToken, resetToken);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<User>> GetAllByIdsList(List<string> idsList)
        {
            var idsBsonArray = idsList.Select(a => new ObjectId(a));
            var filter = Builders<User>.Filter.In(a => a.Id, idsBsonArray.ToArray());

            var users = (await _userCollection.FindAsync(filter)).ToList();

            return users;
        }

        public async Task<List<User>> GetInProgressValidationUsers()
        {
            var filter = Builders<User>.Filter.Where(a=>a.UserIdValidationStatus == "inprogress");

            var users = (await _userCollection.FindAsync(filter)).ToList();

            return users;
        }

        #endregion

        #region Other CRUD 

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(id));
            var deleteResult = await _userCollection.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged;
        }

        #endregion

        #region Login and Register 

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            email = email.Trim().ToLower();
            var user = await _userCollection.Find(a => a.Email == email).FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        // this is for Register 
        public async Task<string> CreateAsync(UserRegisterInput input, Guid guid)
        {
            CreatePasswordHash(input.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var dbUser = new User
            {
                IsActive = false,
                Email = input.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ActivationCode = guid.ToString(),
                ActivationDate = DateTime.MinValue,
                FirstName = input.FirstName.Trim(),
                LastName = input.LastName.Trim(),
            };

            await _userCollection.InsertOneAsync(dbUser);

            return dbUser.Id.ToString();
        }

        public async Task<bool> ConfirmAccountAsync(string guid)
        {
            var user = await _userCollection.Find(a => a.ActivationCode == guid).FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(a => a.Id, user.Id);
            var update = Builders<User>.Update.Set(a => a.ActivationCode, null)
                                                .Set(a => a.ActivationDate, DateTime.Now)
                                                .Set(a => a.IsActive, true);
            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        #endregion


        #region ChangeAccountValidation

        public async Task<bool> ValidateUserChangePhoneNumber(string userId, ValidateUserChangePhoneInput input)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.PhoneNumber, input.PhoneNumber)
                                                .Set(a => a.PhoneNumberValidationStatus, "inprogress");

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> ValidateUserChangeUserId(string userId, ValidateUserChangeIdImageIdInput input)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.IdImageId, input.ImageId)
                                                .Set(a => a.UserIdValidationStatus, "inprogress");

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        #endregion

        #region Password related

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.PasswordHash, passwordHash)
                                                .Set(a => a.PasswordSalt, passwordSalt)
                                                .Set(a => a.ResetDate, default(DateTime))
                                                .Set(a => a.IsResetTokenActive, false)
                                                .Set(a => a.ResetToken, null)
                                                .Set(a => a.IsActive, true)
                                                .Set(a => a.ActivationDate, DateTime.Now);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> SetResetPasswordInformationAsync(string userId, string resetToken)
        {
            var filter = Builders<User>.Filter.Eq(a => a.Id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(a => a.ResetToken, resetToken)
                                                .Set(a => a.IsResetTokenActive, true)
                                                .Set(a => a.ResetDate, DateTime.Now);

            var updateResult = await _userCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        #endregion

        #region Utils

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        #endregion

        #region Unique validators 

        public async Task<bool> IsEmailUnique(string email, string id)
        {
            email = email.Trim().ToLower();
            var regWord = "/^(\\s*)" + email + "(\\s*)$/i";

            if (string.IsNullOrEmpty(id))
            {
                var filter = Builders<User>.Filter.Regex(a => a.Email, new BsonRegularExpression(regWord));
                var count = await _userCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
            else
            {
                var filter = Builders<User>.Filter.Ne(a => a.Id, new ObjectId(id)) & Builders<User>.Filter.Regex(a => a.Email, new BsonRegularExpression(regWord));
                var count = await _userCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
        }

        #endregion
    }
}

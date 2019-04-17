namespace ImageServe.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using ImageServe.Data.Common;
    using ImageServe.Models;
    using ImageServe.Services.Contracts;

    using AutoMapper;
    public class UserService : IUserService
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRepository<User> users;
        private readonly IMapper mapper;
        private readonly User currentUser;

        public UserService(IHttpContextAccessor httpContext, IRepository<User> users, IMapper mapper)
        {
            this.httpContextAccessor = httpContext;
            this.users = users;
            this.mapper = mapper;

            var currentUserClaims = this.GetCurrentUserClaims();
            if (!this.users.All().Any(u => u.Id == currentUserClaims.Id))
            {
                this.AddAsync().GetAwaiter().GetResult();
            }

            this.currentUser = this.users.All().SingleOrDefault(u=>u.Id == currentUserClaims.Id);

        }

        public UserService(User user,IRepository<User> users, IMapper mapper)
        {
            this.currentUser = user;
            this.users = users;
            this.mapper = mapper;

            if (!this.users.All().Any(u => u.Id == currentUser.Id))
            {
                this.AddAsync().GetAwaiter().GetResult();
            }
        }

       

        public T GetCurrent<T>()
        {            
            var userViewModel = mapper.Map<T>(this.currentUser);

            return userViewModel;
        }

        public string GetCurrentId()
        {
            return this.currentUser.Id;
        }
                
        public async Task SaveDescriptionAsync(string details)
        {
            this.currentUser.Details = details;

            await this.users.SaveChangesAsync();
            
        }

        public async Task<bool> UserExistsAsync()
        {
            var objectId = GetCurrentId();

            return await this.users.All().AnyAsync(u => u.Id == objectId);
        }


        public string GetCurrentAvatar()
        {
            var avatarName = this.currentUser.Avatar;

            return avatarName;
        }

        
        public async Task SetAvatarAsync(string name)
        {
            this.currentUser.Avatar = name;
            await this.users.SaveChangesAsync();
        }
        

        public async Task<T> SingleOrDefaultAsync<T>(Expression<Func<User,bool>> predicate)
        {
            var user = await this.users.All().SingleOrDefaultAsync(predicate);

            var userViewModel = this.mapper.Map<T>(user);

            return userViewModel;
        }


        public ICollection<string> AllFullNames()
        {
            var users =  this.users
                .All()
                .Select(u => u.GetFullName())
                .Where(u => u != currentUser.GetFullName() 
                    && !currentUser.UserFriends.Any(f=>f.Friend.GetFullName() == u))
                .ToList();

            return users;
        }


        public async Task AddFriendAsync(string friendFullName)
        {
            if (!this.AllFullNames().Contains(friendFullName))
            {
                throw new ArgumentException("Invalid name");
            }

            var targetUser = await this.SingleOrDefaultAsync<User>(u => u.GetFullName() == friendFullName);

            var friendship = new Friendship()
            {
                UserId = currentUser.Id,
                FriendId = targetUser.Id
            };

            currentUser.UserFriends.Add(friendship);

            await this.users.SaveChangesAsync();
        }   



        public async Task RemoveFriendAsync(string friendId)
        {
            if (!await this.users.All().AnyAsync(u => u.Id == friendId)
                || !this.currentUser.UserFriends.Any(f => f.FriendId == friendId))
            {
                return;
            }

            this.currentUser.UserFriends = this.currentUser.UserFriends.Where(f => f.FriendId != friendId).ToList();
            await this.users.SaveChangesAsync();
        }

        public User GetUserById(string id)
        {
            return this.users.All().FirstOrDefault(u => u.Id == id);
        }


        public string GetCurrentUserNames()
        {            
            return this.currentUser.FirstName + " " + this.currentUser.LastName;
        }

        //Private methods

        private async Task AddAsync()
        {

            await this.users.AddAsync(this.GetCurrentUserClaims());
            await this.users.SaveChangesAsync();
        }

        private User GetCurrentUserClaims()
        {
            var claimArr = httpContextAccessor.HttpContext.User.Claims.ToArray();

            var user = new User()
            {
                Id = claimArr[0].Value.ToString(),
                City = claimArr[1].Value.ToString(),
                Country = claimArr[2].Value.ToString(),
                FirstName = claimArr[3].Value.ToString(),
                LastName = claimArr[4].Value.ToString()
            };

            return user;
        }
        

    }
}


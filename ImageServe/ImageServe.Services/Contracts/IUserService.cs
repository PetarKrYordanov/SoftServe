using ImageServe.Models;
using ImageServe.WebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ImageServe.Services.Contracts
{
    public interface IUserService
    {

        string GetCurrentId();

        T GetCurrent<T>();

        Task SaveDescriptionAsync(string description);

        Task<bool> UserExistsAsync();

        string GetCurrentAvatar();

        Task SetAvatarAsync(string name);

        User GetUserById(string name);

        Task<T> SingleOrDefaultAsync<T>(Expression<Func<User, bool>> predicate);
        
        //bool IsFriend(string userId);

        ICollection<string> AllFullNames();

        Task AddFriendAsync(string friendFullName);

        Task RemoveFriendAsync(string friendId);

        string GetCurrentUserNames();
    }
}

namespace ImageServe.WebModels.ViewModels
{
    using System.Collections.Generic;

    using ImageServe.WebModels.Dtos;

    public class UserFriendlistViewModel
    {
        public string Id { get; set; }

        public  ICollection<FriendshipDto> UserFriends { get; set; }

        public string AddFriend { get; set; }
    }
}

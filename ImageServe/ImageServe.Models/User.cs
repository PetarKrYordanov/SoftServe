namespace ImageServe.Models
{
    
    using System.Collections.Generic;

    public class User 
    {
        public User()
        {
            this.Images = new List<Image>();
            this.UserFriends = new List<Friendship>();
            this.FriendUsers = new List<Friendship>();

        }

        public string Id { get; set; }

        public string FirstName { get; set; }
           
        public string LastName { get; set; }

        public string Details { get; set; }

        public string Avatar { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public virtual ICollection<Image> Images{ get; set; }
        
        public virtual ICollection<Friendship> UserFriends { get; set; }

        public virtual ICollection<Friendship> FriendUsers { get; set; }

        public virtual ICollection<ImageLike> Likes { get; set; }
        
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }

    }
}

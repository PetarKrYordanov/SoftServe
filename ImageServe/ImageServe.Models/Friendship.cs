namespace ImageServe.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Friendship
    {
        [Required]
        public string FriendId { get; set; }
        public virtual User Friend { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}

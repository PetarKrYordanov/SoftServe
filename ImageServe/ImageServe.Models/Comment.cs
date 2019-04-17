namespace ImageServe.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public  class Comment
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public string UserName { get; set; }

        

    }
}

using System;

namespace MyProject.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        public DateTime RegisterTime { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}

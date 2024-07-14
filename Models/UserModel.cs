using System.ComponentModel.DataAnnotations;

namespace FeedbackApp.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public int GitHubId { get; set; }
        public string UserName { get; set; }
        public bool isAdmin { get; set; }
        public bool isBlocked { get; set; }
    }
}

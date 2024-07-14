using System.ComponentModel.DataAnnotations;

namespace FeedbackApp.Models
{
    public class FeedbackModel
    {
        public FeedbackModel() { }

        public FeedbackModel(int _id, string _title, string _body, DateTime _createDate)
        {
            Id = _id;
            Title = _title;
            Body = _body;
            CreatedDate = _createDate;
        }

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The Feedback Title field is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The Feedback Body field is required.")]
        public string Body { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookClub.Models
{
    public class ChapterTopic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChapterTopic_Id { get; set; }

        public int Chapter_Id { get; set; }

        public int Question_Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Option_A { get; set; }
        [Required]
        public string Option_B { get; set; }
        [Required]
        public string Option_C { get; set; }
        [Required]
        public string Option_D { get; set; }
        [Required]
        public string Answer { get; set; }
        [ForeignKey("Chapter_Id")]
        public virtual Chapter Chapter { get; set; }
    }
}

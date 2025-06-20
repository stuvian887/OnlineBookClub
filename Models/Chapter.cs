﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.Models
{
    public class Chapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Chapter_Id { get; set; }
        [Required]
        public int Chapter_Index { get; set; }
        [Required]
        public string Chapter_Name { get; set; }
        [Required]
        public bool Status { get; set; }
        public int Plan_Id { get; set; }
        [ForeignKey("Plan_Id")]
        public virtual BookPlan Plan { get; set; }

        public virtual ICollection<Learn> Learn { get; set; } = new List<Learn>();
    }
}

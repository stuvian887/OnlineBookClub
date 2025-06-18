using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookClub.Models;

public partial class Learn
{
    public int Learn_Id { get; set; }
    public int Plan_Id { get; set; }
    public int? Chapter_Id { get; set; }
    public string Learn_Name { get; set; }

    public int Learn_Index { get; set; }

    public double Pass_Standard { get; set; }

    public DateTime DueTime { get; set; }

    public int Days { get; set; }

    public bool Manual_Check { get; set; }

    public virtual ICollection<Answer_Record> Answer_Record { get; set; } = new List<Answer_Record>();

    [ForeignKey("Chapter_Id")]
    public virtual Chapter Chapter { get; set; }
    public virtual BookPlan Plan { get; set; }

    public virtual ICollection<ProgressTracking> ProgressTracking { get; set; } = new List<ProgressTracking>();

    public virtual ICollection<Topic> Topic { get; set; } = new List<Topic>();
}

using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Members
{
    public int User_Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string AuthCode { get; set; }

    public bool? Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public string ProfilePictureUrl { get; set; }

    public virtual ICollection<Answer_Record> Answer_Record { get; set; } = new List<Answer_Record>();

    public virtual ICollection<BookPlan> BookPlan { get; set; } = new List<BookPlan>();

    public virtual ICollection<Notice> Notice { get; set; } = new List<Notice>();

    public virtual ICollection<PlanMembers> PlanMembers { get; set; } = new List<PlanMembers>();

    public virtual ICollection<Post> Post { get; set; } = new List<Post>();

    public virtual ICollection<ProgressTracking> ProgressTracking { get; set; } = new List<ProgressTracking>();

    public virtual ICollection<Reply> Reply { get; set; } = new List<Reply>();
}

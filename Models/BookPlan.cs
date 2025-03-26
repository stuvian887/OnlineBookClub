using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class BookPlan
{
    public int Plan_Id { get; set; }

    public string Plan_Name { get; set; } = null!;

    public string Plan_Goal { get; set; } = null!;

    public string Plan_Type { get; set; } = null!;

    public bool IsPublic { get; set; }

    public bool IsComplete { get; set; }

    public int User_Id { get; set; }

    public virtual ICollection<Book> Book { get; set; } = new List<Book>();

    public virtual ICollection<Learn> Learn { get; set; } = new List<Learn>();

    public virtual ICollection<PlanMembers> PlanMembers { get; set; } = new List<PlanMembers>();

    public virtual ICollection<Post> Post { get; set; } = new List<Post>();

    public virtual ICollection<Statistic> Statistic { get; set; } = new List<Statistic>();

    public virtual Members User { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class PlanMembers
{
    public int User_Id { get; set; }

    public int Plan_Id { get; set; }

    public string Role { get; set; }

    public DateTime JoinDate { get; set; }

    public virtual BookPlan Plan { get; set; }

    public virtual Members User { get; set; }
}

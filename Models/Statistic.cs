using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Statistic
{
    public int Statistics_Id { get; set; }

    public int Plan_Id { get; set; }

    public int CopyCount { get; set; }

    public int UserCount { get; set; }

    public int ViewTimes { get; set; }

    public virtual BookPlan Plan { get; set; } = null!;
}

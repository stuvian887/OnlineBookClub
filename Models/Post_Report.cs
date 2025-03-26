using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Post_Report
{
    public int P_Report_Id { get; set; }

    public int Post_Id { get; set; }

    public string Action { get; set; } = null!;

    public string Report_text { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}

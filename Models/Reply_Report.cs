using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Reply_Report
{
    public int R_Report_Id { get; set; }

    public int Reply_Id { get; set; }

    public string Action { get; set; }

    public string Report_text { get; set; }

    public virtual Reply Reply { get; set; }
}

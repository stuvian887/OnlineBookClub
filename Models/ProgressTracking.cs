using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class ProgressTracking
{
    public int Progress_Id { get; set; }

    public int User_Id { get; set; }

    public int Learn_Id { get; set; }

    public bool Status { get; set; }

    public DateTime? CompletionDate { get; set; }

    public virtual Learn Learn { get; set; } = null!;

    public virtual Members User { get; set; } = null!;
}

﻿using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class ProgressTracking
{
    public int Progress_Id { get; set; }

    public int User_Id { get; set; }

    public int Learn_Id { get; set; }

    public bool Status { get; set; }

    public DateTime? CompletionDate { get; set; }
    public DateTime LearnDueTime { get; set; }

    public virtual Learn Learn { get; set; }

    public virtual Members User { get; set; }
}

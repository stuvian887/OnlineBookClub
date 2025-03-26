using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Topic
{
    public int Topic_Id { get; set; }

    public int Learn_Id { get; set; }

    public int Question_Id { get; set; }

    public string Question { get; set; } = null!;

    public string Option_A { get; set; } = null!;

    public string Option_B { get; set; } = null!;

    public string Option_C { get; set; } = null!;

    public string Option_D { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public virtual Learn Learn { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Topic
{
    public int Topic_Id { get; set; }

    public int Learn_Id { get; set; }

    public int Question_Id { get; set; }

    public string Question { get; set; }

    public string Option_A { get; set; }

    public string Option_B { get; set; }

    public string Option_C { get; set; }

    public string Option_D { get; set; }

    public string Answer { get; set; }

    public virtual Learn Learn { get; set; }
}

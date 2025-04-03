using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Answer_Record
{
    public int AR_Id { get; set; }

    public int User_Id { get; set; }

    public int Learn_Id { get; set; }

    public int Topic_Id { get; set; }

    public DateTime AnswerDate { get; set; }

    public string Answer { get; set; }

    public int times { get; set; }

    public bool? Pass { get; set; }

    public virtual Learn Learn { get; set; }

    public virtual Members User { get; set; }
}

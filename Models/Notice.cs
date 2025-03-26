using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Notice
{
    public int Notice_Id { get; set; }

    public int User_Id { get; set; }

    public DateTime NoticeTime { get; set; }

    public string Message { get; set; } = null!;

    public virtual Members User { get; set; } = null!;
}

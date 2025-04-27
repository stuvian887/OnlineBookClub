using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OnlineBookClub.Models;

public partial class Reply
{
    public int Reply_Id { get; set; }

    public int Post_Id { get; set; }

    public int User_Id { get; set; }

    public string ReplyContent { get; set; }

    public DateTime ReplyTime { get; set; }

    public string ReplyImg { get; set; }

    public virtual Post Post { get; set; }

    public virtual ICollection<Reply_Report> Reply_Report { get; set; } = new List<Reply_Report>();

    public virtual Members User { get; set; }
}

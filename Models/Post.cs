using System;
using System.Collections.Generic;

namespace OnlineBookClub.Models;

public partial class Post
{
    public int Post_Id { get; set; }

    public int Plan_Id { get; set; }

    public int User_Id { get; set; }

    public string Name { get; set; }

    public string Content { get; set; }

    public DateTime CreateTime { get; set; }

    public string Img_Path { get; set; }

    public bool IsDeleted { get; set; } = false;
    public virtual BookPlan Plan { get; set; }

    public virtual ICollection<Post_Report> Post_Report { get; set; } = new List<Post_Report>();

    public virtual ICollection<Reply> Reply { get; set; } = new List<Reply>();

    public virtual Members User { get; set; }
}

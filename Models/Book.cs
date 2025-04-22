using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OnlineBookClub.Models;

public partial class Book
{
    public int Book_Id { get; set; }

    public int Plan_Id { get; set; }

    public string BookName { get; set; }

    public string Description { get; set; }

    public string Link { get; set; }

    public string bookpath { get; set; }
    [JsonIgnore]
    public virtual BookPlan Plan { get; set; }
}

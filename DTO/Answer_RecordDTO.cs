namespace OnlineBookClub.DTO
{
    public class Answer_RecordDTO
    {
        public int AR_Id { get; set; }

        public int User_Id { get; set; }

        public int Learn_Id { get; set; }

        public int Topic_Id { get; set; }

        public DateTime AnswerDate { get; set; }

        public string Answer { get; set; }

        public int times { get; set; }

        public bool? Pass { get; set; }
    }
}

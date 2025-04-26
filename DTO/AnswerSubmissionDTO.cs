namespace OnlineBookClub.DTO
{
    public class AnswerSubmissionDTO
    {
        public int Plan_Id { get; set; }
        public int Learn_Index { get; set; }
        public List<AnswerInputDTO> Answers {  get; set; }
    }
}

namespace OnlineBookClub.Models
{
    public class ForPaging
    {
        // 當前頁數
        public int NowPage { get; set; }

        // 最大頁數（由資料總筆數計算而來）
        public int MaxPage { get; set; }

        // 每頁幾筆（固定為 5）
        public int ItemNum => 10;

        public ForPaging()
        {
            NowPage = 1;
        }

        public ForPaging(int page)
        {
            NowPage = page;
        }

        // 頁數修正
        public void SetRightPage()
        {
            if (MaxPage == 0)
            {
                NowPage = 1;
            }
            else if (NowPage < 1)
            {
                NowPage = 1;
            }
            else if (NowPage > MaxPage)
            {
                NowPage = MaxPage;
            }
        }
    }
}

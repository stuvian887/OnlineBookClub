using OnlineBookClub.Models;
using System.Collections.Generic;

namespace OnlineBookClub.Repository
{
    public class MembersRepository
    {
        private readonly OnlineBookClubContext _OnlineBookClubContext;
        public MembersRepository(OnlineBookClubContext OnlineBookClubContext)
        {
            _OnlineBookClubContext = OnlineBookClubContext;
        }
        public void Add(Members NewMember)
        {
            _OnlineBookClubContext.Members.Add(NewMember);
            _OnlineBookClubContext.SaveChanges();
            
        }
        public void Delete(Members Member)
        {
            _OnlineBookClubContext.Members.Remove(Member);
            _OnlineBookClubContext.SaveChanges();
        }
        public void Update(Members Member) 
        {
            _OnlineBookClubContext.Members.Update(Member);
            _OnlineBookClubContext.SaveChanges();
        }
        




    }
}

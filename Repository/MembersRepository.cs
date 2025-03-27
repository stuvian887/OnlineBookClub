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
        public Members Add(Members NewMember)
        {
            Members result = new Members
            {
                UserName = NewMember.UserName,
                Email = NewMember.Email,
                Password = NewMember.Password,
                AuthCode = NewMember.AuthCode,
                Gender = NewMember.Gender,
                Birthday = NewMember.Birthday
            };
            _OnlineBookClubContext.Members.Add(result);
            _OnlineBookClubContext.SaveChanges();
            return result;
        }

    }
}

using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;  // 使用 EntityFramework 的非同步擴充方法

namespace OnlineBookClub.Repository
{
    public class MembersRepository
    {
        private readonly OnlineBookClubContext _OnlineBookClubContext;
        public MembersRepository(OnlineBookClubContext OnlineBookClubContext)
        {
            _OnlineBookClubContext = OnlineBookClubContext;
        }

        // 新增會員，改為非同步
        public async Task AddAsync(Members NewMember)
        {
            await _OnlineBookClubContext.Members.AddAsync(NewMember); // 使用非同步 AddAsync
            await _OnlineBookClubContext.SaveChangesAsync(); // 使用非同步 SaveChangesAsync
        }

        // 刪除會員，改為非同步
        public async Task DeleteAsync(Members Member)
        {
            _OnlineBookClubContext.Members.Remove(Member);
            await _OnlineBookClubContext.SaveChangesAsync(); // 使用非同步 SaveChangesAsync
        }

        // 更新會員，改為非同步
        public async Task UpdateAsync(Members Member)
        {
            _OnlineBookClubContext.Members.Update(Member);
            await _OnlineBookClubContext.SaveChangesAsync(); // 使用非同步 SaveChangesAsync
        }

        // 依照ID取得會員，已經是非同步
        public async Task<Members> GetByIdAsync(int userid)
        {
            return await _OnlineBookClubContext.Members.FirstOrDefaultAsync(m => m.User_Id == userid); // 使用非同步 FirstOrDefaultAsync
        }

    }
}

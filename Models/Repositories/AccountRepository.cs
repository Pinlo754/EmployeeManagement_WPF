using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repositories
{
    public class AccountRepository
    {
        private readonly EmployeeManagementContext _context;

        public AccountRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<Account?> LoginAsync(string username, string password)
        {
            // Giả sử password đã hash sẵn
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == password && a.IsActive == true);
        }
    }


}

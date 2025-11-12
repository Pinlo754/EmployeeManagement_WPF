using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Helper
{
    public static class Session
    {
        public static Account? CurrentUser { get; set; }
    }

}

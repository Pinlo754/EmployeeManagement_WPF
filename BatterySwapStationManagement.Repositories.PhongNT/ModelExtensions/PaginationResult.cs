using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Repositories.PhongNT.ModelExtensions
{
    public class PaginationResult<T> where  T :class 
    {
        public int TotalPage { get; set; }  
        public int TotalItem {get; set; }   
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public T item { get; set; }

    }
}

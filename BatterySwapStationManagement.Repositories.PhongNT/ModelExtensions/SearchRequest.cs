using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatterySwapStationManagement.Repositories.PhongNT.ModelExtensions
{
    public class SearchRequest
    {
        public int? currentPage { get; set; }   
        public int? pageSize { get; set; }   
    }
    
    public class StationSearchRequest : SearchRequest
    {

        public int? capitalSlot  { get; set; }  
        public string? address { get; set; }    
        public string? stationTypeName { get; set; }    
    }   
}

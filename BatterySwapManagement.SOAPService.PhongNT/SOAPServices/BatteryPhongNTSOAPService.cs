using BatterySwapStationManagement.SOAPService.PhongNT;
namespace BatterySwapStationManagement.SOAPService.PhongNT.SOAPServices
{
    [ServiceContract]
    public interface BatteryPhongNTSOAPService
    {
        [OperationContract]
        Task<List><BatteryPhongNT> GetAllAsync();
        [OperationContract]
        Task<BatteryPhongNT> GetByIdAsync(Guid id);

        //Mutation
        [OperationContract]
        Task

    }
    public class BatteryPhongNTSOAPService
    {
    }
}

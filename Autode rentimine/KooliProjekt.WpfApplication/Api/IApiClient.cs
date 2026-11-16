using System.Collections.Generic;
using System.Threading.Tasks;
using KooliProjekt.WpfApplication.Api;

namespace KooliProjekt.WpfApplication.Api
{
    public interface IApiClient
    {
        Task<IList<Car>> List(string numberPlate = null);
        Task<OperationResultResponse> Save(Car car);
        Task<OperationResultResponse> Delete(int id);
    }
}
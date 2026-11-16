using System.Collections.Generic;
using System.Threading.Tasks;

namespace KooliProjekt.WindowsForms.Api
{
    public interface IApiClient
    {
        Task<IList<Car>> List(string numberPlate = null);
        Task<OperationResultResponse> Save(Car car);
        Task<OperationResultResponse> Delete(int id);
    }
}

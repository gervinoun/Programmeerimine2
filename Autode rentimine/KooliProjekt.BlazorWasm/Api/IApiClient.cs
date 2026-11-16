using System.Collections.Generic;
using System.Threading.Tasks;
using KooliProjekt.BlazorWasm.Api;

namespace KooliProjekt.BlazorWasm
{
    public interface IApiClient
    {
        Task<IList<Car>> List();
        Task<OperationResultResponse<Car>> Get(int id);
        Task<OperationResultResponse<object>> Save(Car car);
    }
}
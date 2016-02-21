using System.Threading.Tasks;

namespace WebColorApplication.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
    }
}
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Fazan.Application.Common
{
    public interface IApplication
    {
        void ConfigureDi();

        Task<Result> Run();
    }
}

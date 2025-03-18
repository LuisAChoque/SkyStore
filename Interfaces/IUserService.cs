using SkyStore.Models;

namespace SkyStore.Interfaces
{
    public interface IUserService
    {
        bool RegisterUser(UserRegister request);
        string LoginUser(UserLogin request);
    }
}
using OnlineShopBack.Domain.DTOs;

namespace OnlineShopBack.Domain.Repository
{
    public interface ILoginRepository
    {
        public int Login(LoginDto value);

        public void LogOut();
    }
}

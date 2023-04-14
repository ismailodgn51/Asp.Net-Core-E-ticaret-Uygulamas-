using MainMusicStore.DataAccess.IRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.IMainRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
    }
}

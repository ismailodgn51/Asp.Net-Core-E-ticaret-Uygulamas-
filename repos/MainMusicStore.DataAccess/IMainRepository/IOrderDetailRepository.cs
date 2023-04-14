using MainMusicStore.DataAccess.IRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.IMainRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetails);
    }
}

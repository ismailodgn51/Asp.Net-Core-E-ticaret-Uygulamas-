using System;
using System.Collections.Generic;
using System.Text;

namespace MainMusicStore.DataAccess.IMainRepository
{
  public  interface IUnitOfWork : IDisposable

    {
        ICategoryRepository Category { get; } 
        ICompanyRepository Company { get; }     
        IProductRepository Product { get; }
        ICoverTypeRepository CoverType { get; }      
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }      
        IApplicationUserRepository ApplicationUser { get; }
        ISPCallRepository Sp_call { get;  }     

        void Save();
    }
}

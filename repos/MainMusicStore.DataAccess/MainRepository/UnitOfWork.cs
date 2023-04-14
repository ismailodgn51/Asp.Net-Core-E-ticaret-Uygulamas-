﻿using MainMusicStore.Data;
using MainMusicStore.DataAccess.IMainRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Company = new CompanyRepository(_db);
            Product = new ProductRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Sp_call = new SPCallRepository(_db);
        }
        
        public ICategoryRepository Category { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public ISPCallRepository Sp_call { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

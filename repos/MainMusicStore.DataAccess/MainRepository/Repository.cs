﻿using MainMusicStore.Data;
using MainMusicStore.DataAccess.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {


        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
      public  Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }        
        /// <summary>
        /// GenericAdded Entity
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, 
            IOrderedQueryable<T>> orederBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            

            if (filter !=null)
            {
                query = query.Where(filter);
            }
           
          
            if (includeProperties !=null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
        
            
            if (orederBy !=null)
            {
                return orederBy(query).ToList();
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;


            if (filter != null)
            {
                query = query.Where(filter);
            }


            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            return query.FirstOrDefault();
        }

        public void Remove(int id)
        {
            T entity = dbSet.Find(id);
            Remove(entity);
        }

        public void Remove(T entitiy)
        {
            dbSet.Remove(entitiy);
        }

        public void RemoveRange(IEnumerable<T> entitiy)
        {
            dbSet.RemoveRange(entitiy);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Repository;

namespace WebApplication2.unitofwork
{
    public class UnitofWork:DbContext,IDisposable
    {
        private HomeModel model = new HomeModel();
        private GenericRepository<webtbluser> rep;
        public GenericRepository<webtbluser> repo
        {
            get
            {
                if (this.rep == null)
                {
                    this.rep = new GenericRepository<webtbluser>(model);
                }
                return rep;
            }
        }
        private GenericRepository<slider> srep;
        public GenericRepository<slider> srepo
        {
            get
            {
                if (this.srep == null)
                {
                    this.srep = new GenericRepository<slider>(model);
                }
                return srep;
            }
        }

        private GenericRepository<pagemodel> prep;
        public GenericRepository<pagemodel> prepo
        {
            get
            {
                if (this.prep == null)
                {
                    this.prep = new GenericRepository<pagemodel>(model);
                }
                return prep;
            }
        }
        public void Save()
        {
            model.SaveChanges();
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    model.Dispose();
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
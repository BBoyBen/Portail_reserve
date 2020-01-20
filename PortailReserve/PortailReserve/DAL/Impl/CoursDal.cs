using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class CoursDal : ICoursDal
    {
        private BddContext bdd;

        public CoursDal ()
        {
            bdd = new BddContext();
        }

        public void Dispose()
        {
            bdd.Dispose();
        }
    }
}
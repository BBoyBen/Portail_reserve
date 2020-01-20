using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class AdresseDal : IAdresseDal
    {
        private BddContext bdd;

        public AdresseDal ()
        {
            bdd = new BddContext();
        }
        public void Dispose()
        {
            bdd.Dispose();
        }
    }
}
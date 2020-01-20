using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class EffectifDal : IEffectifDal
    {
        private BddContext bdd;

        public EffectifDal () 
        {
            bdd = new BddContext();    
        }
        public void Dispose()
        {
            bdd.Dispose();
        }
    }
}
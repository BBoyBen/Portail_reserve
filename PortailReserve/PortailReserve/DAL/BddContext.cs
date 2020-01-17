using Microsoft.EntityFrameworkCore;

namespace PortailReserve.DAL
{
    public class BddContext : DbContext
    {
        public BddContext ()
        {

        }

        public BddContext Create ()
        {
            return new BddContext();
        }
    }
}
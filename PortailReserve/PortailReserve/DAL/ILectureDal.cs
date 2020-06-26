using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface ILectureDal
    {
        Guid AjouterLecture(Guid util, Guid message);
        Lecture GetLectureByMessageAndByUtil(Guid message, Guid util);
    }
}

using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IMessageDal : IDisposable
    {
        Guid AjouterMessage(Message message);
        Message GetMessageById(Guid id);
        List<Message> GetAllMessage();
        List<Message> GetMessagesByEvent(Guid idEvent);
        int SupprimerMessage(Guid id);
    }
}

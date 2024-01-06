using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface BillRepository
    {
        List<Bill> FindAll();
        List<Bill> FindAllByCreatedBy(string emailCreatedBy);
        Bill FindByUUID(string billUUID);
        void Add(Bill bill);
        void Delete(Bill bill);
        int CountAll();
    }
}

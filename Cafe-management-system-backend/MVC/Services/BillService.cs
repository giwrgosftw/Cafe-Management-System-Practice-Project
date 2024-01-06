using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services
{
    public interface BillService
    {
        List<Bill> FindAllBills();
        List<Bill> FindAllBillsByCreatedBy(string emailCreatedBy);
        Bill FindBillByUUID(string billUUID);
        void AddBill(Bill bill);
        void DeleteBill(string uuid);
        int CountAllBills();
    }
}

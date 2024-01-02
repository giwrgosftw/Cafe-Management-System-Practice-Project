using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface BillProductRepository
    {
        void Add(BillProduct billProduct);
        void Delete(List<BillProduct> billProducts);
        List<BillProduct> FindAllByBillId(int billId);
    }
}

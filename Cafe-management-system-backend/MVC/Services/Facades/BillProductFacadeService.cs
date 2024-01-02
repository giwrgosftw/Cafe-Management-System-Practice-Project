using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Models.Dtos;
using Cafe_management_system_backend.MVC.Security;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.Facades
{
    public interface BillProductFacadeService
    {
        List<BillProduct> FindBillProductsByBillId(int billId);
        void DeleteBillProductAndBill(string billUuid);
        (Bill, byte[]) GenerateBillReport(PrincipalProfile principalProfile, GenerateBillReportDto generateBillReportDto);
    }
}

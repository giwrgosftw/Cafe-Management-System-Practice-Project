using Cafe_management_system_backend.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class BillProductRepositoryImpl : ConnectionRepositoryDB, BillProductRepository
    {
        /// <summary> Adds a new BillProduct to the system. </summary>
        /// <param name="billProduct"> The BillProduct object to be added. </param>
        public void Add(BillProduct billProduct)
        {
            try
            {
                db.BillProducts.Attach(billProduct);
                db.BillProducts.Add(billProduct);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"BillProductRepository:Add()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Deletes a list of BillProducts from the system. </summary>
        /// <param name="billProducts"> The list of BillProducts to be deleted. </param>
        public void Delete(List<BillProduct> billProducts)
        {
            try
            {
                // Attach and remove each BillProduct in the list
                billProducts.ForEach(billProduct => { db.BillProducts.Attach(billProduct); db.BillProducts.Remove(billProduct); });
                // Save changes after removing all BillProducts
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[BillProductRepository:Delete()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }


        /// <summary> Finds all BillProducts in the system by Bill ID. </summary>
        /// <param name="billId"> The ID of the bill associated with the BillProducts. </param>
        /// <returns> A list of BillProduct objects if found, otherwise an empty list. </returns>
        public List<BillProduct> FindAllByBillId(int billId)
        {
            try
            {
                return db.BillProducts.Where(bp => bp.billId == billId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"[BillProductRepository:FindAllByBillId()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

    }
}
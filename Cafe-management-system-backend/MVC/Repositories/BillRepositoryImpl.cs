using Cafe_management_system_backend.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class BillRepositoryImpl : ConnectionRepositoryDB, BillRepository
    {

        /// <summary> Retrieves all bills in the system. </summary>
        /// <returns> A list of all Bill objects in the system. </returns>
        public List<Bill> FindAll()
        {
            try
            {
                return db.Bills.OrderByDescending(b => b.id).ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"[BillRepository:FindAll()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Finds all bills in the system created by a specific user. </summary>
        /// <param name="emailCreatedBy"> The email of the creator. </param>
        /// <returns> A list of Bill objects if found, otherwise an empty list. </returns>
        public List<Bill> FindAllByCreatedBy(string emailCreatedBy)
        {
            try
            {
                return db.Bills.Where(b => b.createdBy == emailCreatedBy)
                               .OrderByDescending(b => b.id)
                               .ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"[BillRepository:FindAllByCreatedBy()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Finds a bill in the system by its UUID. </summary>
        /// <param name="billUUID"> The UUID of the bill to be found. </param>
        /// <returns> The Bill object if found, otherwise null. </returns>
        public Bill FindByUUID(string billUUID)
        {
            try
            {
                return db.Bills.Where(b => b.uuid == billUUID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error($"[BillRepository:FindByUUID()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Adds a new bill to the system. </summary>
        /// <param name="bill"> The Bill object to be added. </param>
        public void Add(Bill bill)
        {
            try
            {
                db.Bills.Add(bill);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"BillRepository:Add()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Deletes a bill from the system. </summary>
        /// <param name="bill"> The Bill object to be deleted. </param>
        public void Delete(Bill bill)
        {
            try
            {
                db.Bills.Remove(bill);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"BillRepository:DeleteBill()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }
    }
}
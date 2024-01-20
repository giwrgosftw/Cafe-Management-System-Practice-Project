using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using NLog;
using System.Data;
using System;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services
{
    public class BillServiceImpl : BillService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");
        private readonly BillRepository billRepository;

        public BillServiceImpl(BillRepository billRepository)
        {
            this.billRepository = billRepository;
        }

        /// <summary> Retrieves all bills in the system. </summary>
        /// <returns> A list of all Bill objects in the system. </returns>
        public List<Bill> FindAllBills()
        {
            return billRepository.FindAll();
        }

        /// <summary> Retrieves all bills in the system created by a specific user. </summary>
        /// <param name="emailCreatedBy"> The email address of the creator. </param>
        /// <returns> A list of Bill objects if found, otherwise an empty list. </returns>
        public List<Bill> FindAllBillsByCreatedBy(string emailCreatedBy)
        {
            return billRepository.FindAllByCreatedBy(emailCreatedBy);
        }

        /// <summary> Finds a bill in the system by its UUID. </summary>
        /// <param name="billUUID"> The UUID of the bill to be found. </param>
        /// <returns> The Bill object if found, otherwise null. </returns>
        public Bill FindBillByUUID(string billUUID)
        {
            Bill billDB = billRepository.FindByUUID(billUUID);
            if(billDB == null)
            {
                logger.Error($"[BillService:FindBillByUUID()] Fail: Bill with given UUID was NOT found (UUID: {billUUID})");
                throw new KeyNotFoundException($"Bill with given UUID was NOT found (UUID: {billUUID})");
            }
            return billDB; 
        }

        /// <summary> Adds a new bill to the system if it doesn't already exist. </summary>
        /// <param name="bill"> The Bill object to be added. </param>
        /// <exception cref="DuplicateNameException"> Thrown if the bill with the same UUID already exists. </exception>
        public void AddBill(Bill bill)
        {
            if (bill.createdBy == null)
            {
                logger.Error("[BillService:AddBill()] Exception: Bill creator NOT given.");
                throw new ArgumentException("Bill creator NOT given.");
            }
            Bill billDB = billRepository.FindByUUID(bill.uuid);
            if (billDB == null)
            {
                // If bill not exist (since new), add the new Bill
                bill.uuid = GenerateUUID();
                billRepository.Add(bill);
                logger.Info("[BillService:Add() Success]: Bill (UUID: {BillUUID}) was created successfully!", bill.uuid);
            }
            else
            {
                // Return an error message
                logger.Error("[BillService:AddBill()] Exception: Bill already exists (UUID: {BillUUID})", billDB.uuid);
                throw new DuplicateNameException("Bill already exists.");
            }
        }

        /// <summary> Deletes a bill from the system by its UUID. </summary>
        /// <param name="billUuid"> The UUID of the bill to be deleted. </param>
        /// <exception cref="KeyNotFoundException"> Thrown if the bill with the specified UUID is not found. </exception>
        public void DeleteBill(string billUuid)
        {
            Bill bill = FindBillByUUID(billUuid);
            billRepository.Delete(bill);
        }

        /// <summary>Counts the total number of bills.</summary>
        /// <returns>The total number of bills in the database.</returns>
        public int CountAllBills()
        {
            return billRepository.CountAll();
        }

        /// <summary> Generates a unique UUID by combining current ticks and a new Guid. </summary>
        /// <returns>A string representation of the generated unique UUID.</returns>
        private string GenerateUUID()
        {
            var ticks = DateTime.Now.Ticks; // the current date and time expressed as the number of 100-nanosecond (as Ticks - accuracy)
            var guid = Guid.NewGuid().ToString();
            var uniqueUUID = ticks.ToString() + '-' + guid;
            return uniqueUUID;
        }
    }

}
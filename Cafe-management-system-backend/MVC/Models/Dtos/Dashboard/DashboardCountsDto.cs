using System;
using System.Runtime.Serialization;

namespace Cafe_management_system_backend.MVC.Models.Dtos.Dashboard
{
    [Serializable]
    public class DashboardCountsDto : ISerializable
    {
        private int categoriesCount { get; set; }
        private int productsCount { get; set; }
        private int billsCount { get; set; }
        private int usersCount { get; set; }

        public DashboardCountsDto(int categoriesCount, int productsCount, int billsCount, int usersCount)
        {
            this.categoriesCount = categoriesCount;
            this.productsCount = productsCount;
            this.billsCount = billsCount;
            this.usersCount = usersCount;
        }

        /* NOW THE OBJECT CAN BE REPRESENTED IN JSON FORMAT PROPERLY */
        // Deserialization constructor
        protected DashboardCountsDto(SerializationInfo info, StreamingContext context)
        {
            categoriesCount = info.GetInt32(nameof(categoriesCount));
            productsCount = info.GetInt32(nameof(productsCount));
            billsCount = info.GetInt32(nameof(billsCount));
            usersCount = info.GetInt32(nameof(usersCount));
        }

        // Serialization method
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(categoriesCount), categoriesCount);
            info.AddValue(nameof(productsCount), productsCount);
            info.AddValue(nameof(billsCount), billsCount);
            info.AddValue(nameof(usersCount), usersCount);
        }
    }
}

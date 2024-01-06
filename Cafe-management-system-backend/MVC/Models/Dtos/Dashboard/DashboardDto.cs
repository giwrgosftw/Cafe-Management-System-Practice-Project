using System;

namespace Cafe_management_system_backend.MVC.Models.Dtos.Dashboard
{
    [Serializable]
    public class DashboardDto
    {
        private DashboardCountsDto dashboardCountsDto { get; set; }
        public DashboardDto() { }

        // Public getter for dashboardCountsDto
        public DashboardCountsDto GetDashboardCountsDto() => dashboardCountsDto;

        // Public setter for dashboardCountsDto
        public void SetDashboardCountsDto(DashboardCountsDto value) => dashboardCountsDto = value;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChildVaccinationManagementWebsite.Models
{
    public class MakeAppointment
    {
        public string AppointmentId { get; set; } // Thêm thuộc tính AppointmentId
        public string Email { get; set; }
        public DateTime PatientDOB { get; set; }
        public string PatientName { get; set; }
        public string Status { get; set; }
        public DateTime VaccinationDate { get; set; }
        public string VaccinationTime { get; set; }
        public string VaccineName { get; set; }
    }
}
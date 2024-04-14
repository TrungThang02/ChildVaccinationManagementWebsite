using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Cloud.Firestore;

namespace ChildVaccinationManagementWebsite.Controllers
{
    public class MakeAppointmentsController : Controller
    {
        private FirestoreDb _firestoreDb;

        public MakeAppointmentsController()
        {
            // Khởi tạo Firestore client
            string projectId = "childvaccinationmanageme-f8806"; // Thay thế bằng ID của project Firestore của bạn
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        // GET: LichTiemDaDat
        public async Task<ActionResult> LichTiemDaDat()
        {
            // Lấy danh sách các cuộc hẹn từ Firestore
            CollectionReference appointmentsRef = _firestoreDb.Collection("MakeAppointments");
            QuerySnapshot querySnapshot = await appointmentsRef.GetSnapshotAsync();

            // Tạo danh sách để lưu trữ các cuộc hẹn
            List<MakeAppointment> appointmentsList = new List<MakeAppointment>();

            // Lặp qua các mục và thêm vào danh sách
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                MakeAppointment makeappointment = new MakeAppointment
                {
                    AppointmentId = documentSnapshot.Id, // Lấy ID của document
                    Email = data["email"]?.ToString(),
                    PatientDOB = DateTime.Parse(data["patientDOB"]?.ToString()),
                    PatientName = data["patientName"]?.ToString(),
                    Status = data["status"]?.ToString(),
                    VaccinationDate = ((Timestamp)data["vaccinationDate"]).ToDateTime(),
                    VaccinationTime = data["vaccinationTime"]?.ToString(),
                    VaccineName = data["vaccineName"]?.ToString()
                };
                appointmentsList.Add(makeappointment);
            }

            // Truyền danh sách vào view để hiển thị
            return View(appointmentsList);
        }

        // Action để đánh dấu cuộc hẹn là "done"
        public async Task<ActionResult> DuyetCuocHen(string id)
        {
            // Lấy reference đến document cần sửa
            DocumentReference docRef = _firestoreDb.Collection("MakeAppointments").Document(id);

            // Update trạng thái của cuộc hẹn thành "done"
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "status", "done" }
            };
            await docRef.UpdateAsync(data);

            // Chuyển hướng trở lại trang danh sách cuộc hẹn
            return RedirectToAction("LichTiemDaDat");
        }

    }

    // Định nghĩa lớp MakeAppointment để lưu trữ dữ liệu
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

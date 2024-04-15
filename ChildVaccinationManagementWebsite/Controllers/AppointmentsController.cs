using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Cloud.Firestore;
using ChildVaccinationManagementWebsite.Models;
namespace ChildVaccinationManagementWebsite.Controllers
{
    public class AppointmentsController : Controller
    {
        private FirestoreDb _firestoreDb;

        public AppointmentsController()
        {
            // Khởi tạo Firestore client
            string projectId = "childvaccinationmanageme-f8806";
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        // GET: MakeAppointments
        public async Task<ActionResult> LichTiem()
        {
            // Lấy danh sách MakeAppointments từ Firestore
            CollectionReference appointmentsRef = _firestoreDb.Collection("appointment");
            QuerySnapshot querySnapshot = await appointmentsRef.GetSnapshotAsync();

            // Tạo danh sách để lưu trữ các mục
            List<Appointment> appointmentsList = new List<Appointment>();

            // Lặp qua các mục và thêm vào danh sách
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                Appointment appointment = new Appointment
                {
                    Id = documentSnapshot.Id, // Lấy ID của document
                    Name = data["Name"]?.ToString(),
                    Time = data["Time"]?.ToString()
                };
                appointmentsList.Add(appointment);
            }


            // Truyền danh sách vào view để hiển thị
            return View(appointmentsList);
        }

        public async Task<ActionResult> AddAppointment(string name, string time)
        {
            // Tạo một document mới trong collection "appointment"
            DocumentReference docRef = _firestoreDb.Collection("appointment").Document();
            Dictionary<string, object> data = new Dictionary<string, object>
    {
        { "Name", name },
        { "Time", time }
    };
            await docRef.SetAsync(data);

            // Chuyển hướng trở lại trang danh sách cuộc hẹn
            return RedirectToAction("LichTiem");
        }

        public async Task<ActionResult> EditAppointment(string id, string newName, string newTime)
        {
            // Lấy reference đến document cần sửa
            DocumentReference docRef = _firestoreDb.Collection("appointment").Document(id);

            // Update dữ liệu
            Dictionary<string, object> data = new Dictionary<string, object>
    {
        { "Name", newName },
        { "Time", newTime }
    };
            await docRef.SetAsync(data, SetOptions.MergeAll);

            // Chuyển hướng trở lại trang danh sách cuộc hẹn
            return RedirectToAction("LichTiem");
        }

        public async Task<ActionResult> DeleteAppointment(string id)
        {
            // Xóa document từ Firestore
            await _firestoreDb.Collection("appointment").Document(id).DeleteAsync();

            // Chuyển hướng trở lại trang danh sách cuộc hẹn
            return RedirectToAction("LichTiem");
        }
       
    }



}

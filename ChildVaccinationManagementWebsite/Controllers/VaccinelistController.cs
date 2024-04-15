using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Cloud.Firestore;
using ChildVaccinationManagementWebsite.Models;

namespace ChildVaccinationManagementWebsite.Controllers
{
    public class VaccinelistController : Controller
    {
        private FirestoreDb _firestoreDb;

        public VaccinelistController()
        {
            // Khởi tạo Firestore client
            string projectId = "childvaccinationmanageme-f8806";
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        // GET: Vaccinelist
        public async Task<ActionResult> Index()
        {
            // Lấy danh sách Vaccinelist từ Firestore
            CollectionReference vaccinesRef = _firestoreDb.Collection("vaccinelist");
            QuerySnapshot querySnapshot = await vaccinesRef.GetSnapshotAsync();

            // Tạo danh sách để lưu trữ các mục
            List<Vaccine> vaccineList = new List<Vaccine>();

            // Lặp qua các mục và thêm vào danh sách
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                Vaccine vaccine = new Vaccine
                {
                    VaccineName = data["VaccineName"]?.ToString(),
                    Info = data["Info"]?.ToString()
                };
                vaccineList.Add(vaccine);
            }

            // Truyền danh sách vào view để hiển thị
            return View(vaccineList);
        }

        [HttpPost]
        public async Task<ActionResult> AddVaccine(string vaccineName, string info)
        {
            // Tạo một document mới trong collection "vaccinelist"
            DocumentReference docRef = _firestoreDb.Collection("vaccinelist").Document();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "VaccineName", vaccineName },
                { "Info", info }
            };
            await docRef.SetAsync(data);

            // Chuyển hướng trở lại trang danh sách vaccine
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> EditVaccine(string id, string newVaccineName, string newInfo)
        {
            // Lấy reference đến document cần sửa
            DocumentReference docRef = _firestoreDb.Collection("vaccinelist").Document(id);

            // Update dữ liệu
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "VaccineName", newVaccineName },
                { "Info", newInfo }
            };
            await docRef.SetAsync(data, SetOptions.MergeAll);

            // Chuyển hướng trở lại trang danh sách vaccine
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteVaccine(string id)
        {
            // Xóa document từ Firestore
            await _firestoreDb.Collection("vaccinelist").Document(id).DeleteAsync();

            // Chuyển hướng trở lại trang danh sách vaccine
            return RedirectToAction("Index");
        }
    }
}

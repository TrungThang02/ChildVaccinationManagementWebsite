using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Cloud.Firestore;
using ChildVaccinationManagementWebsite.Models;

namespace ChildVaccinationManagementWebsite.Controllers
{
    public class MakeAppointmentController : Controller
    {

        private FirestoreDb _firestoreDb;

        public MakeAppointmentController()
        {
            // Initialize Firestore client
            string projectId = "childvaccinationmanageme-f8806"; // Replace with your Firestore project ID
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        // GET: LichTiemDaDat
        public async Task<ActionResult> LichTiemDaDat()
        {
            // Get list of appointments from Firestore
            CollectionReference appointmentsRef = _firestoreDb.Collection("MakeAppointments");
            QuerySnapshot querySnapshot = await appointmentsRef.GetSnapshotAsync();

            // Create list to store appointments
            List<MakeAppointment> appointmentsList = new List<MakeAppointment>();

            // Loop through documents and add to list
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                MakeAppointment makeappointment = new MakeAppointment
                {
                    AppointmentId = documentSnapshot.Id, // Get document ID
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

            // Pass list to view for display
            return View(appointmentsList);
        }

        // Action to mark appointment as "done"
        public async Task<ActionResult> DuyetCuocHen(string id)
        {
            // Get reference to document to update
            DocumentReference docRef = _firestoreDb.Collection("MakeAppointments").Document(id);

            // Update appointment status to "done"
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "status", "done" }
            };
            await docRef.UpdateAsync(data);

            // Redirect back to appointments list page
            return RedirectToAction("LichTiemDaDat");
        }

      
        
    }
}
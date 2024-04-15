using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Cloud.Firestore;
using Firebase.Storage;
using ChildVaccinationManagementWebsite.Models;

namespace ChildVaccinationManagementWebsite.Controllers
{
    public class NewsController : Controller
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly FirebaseStorage _firebaseStorage;

        public NewsController()
        {
            string projectId = "childvaccinationmanageme-f8806";
            _firestoreDb = FirestoreDb.Create(projectId);

            string firebaseBucket = "childvaccinationmanageme-f8806.appspot.com";
            _firebaseStorage = new FirebaseStorage(firebaseBucket);
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                QuerySnapshot querySnapshot = await _firestoreDb.Collection("news").GetSnapshotAsync();
                List<NewsItem> newsList = new List<NewsItem>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    Dictionary<string, object> data = documentSnapshot.ToDictionary();
                    string title = data.ContainsKey("title") ? data["title"]?.ToString() : "Title not found";
                    string image = data.ContainsKey("image") ? data["image"]?.ToString() : "Image URL not found";
                    string description = data.ContainsKey("description") ? data["description"]?.ToString() : "Description not found";

                    NewsItem newsItem = new NewsItem
                    {
                        Title = title,
                        Image = image,
                        Description = description
                    };
                    newsList.Add(newsItem);
                }

                return View(newsList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading news: " + ex.Message;
                return View(new List<NewsItem>());
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNews(string title, string description, HttpPostedFileBase image)
        {
            try
            {
                string imageUrl = await UploadImageToStorage(image);
                DocumentReference docRef = _firestoreDb.Collection("news").Document();
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "title", title },
                    { "description", description },
                    { "image", imageUrl }
                };
                await docRef.SetAsync(data);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error adding news: " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditNews(string id, string newTitle, string newDescription, HttpPostedFileBase newImage)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("News ID is required.");
                }

                string newImageUrl = null;
                if (newImage != null && newImage.ContentLength > 0)
                {
                    newImageUrl = await UploadImageToStorage(newImage);
                }

                DocumentReference docRef = _firestoreDb.Collection("news").Document(id);

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "title", newTitle },
                    { "description", newDescription }
                };

                if (!string.IsNullOrEmpty(newImageUrl))
                {
                    data["image"] = newImageUrl;
                }

                await docRef.SetAsync(data, SetOptions.MergeAll);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error editing news: " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteNews(string id)
        {
            try
            {
                await _firestoreDb.Collection("news").Document(id).DeleteAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error deleting news: " + ex.Message;
                return View("Error");
            }
        }

        private async Task<string> UploadImageToStorage(HttpPostedFileBase image)
        {
            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            using (var memoryStream = new MemoryStream())
            {
                await image.InputStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await _firebaseStorage.Child("news_images").Child(imageName).PutAsync(memoryStream);
            }

            return await _firebaseStorage.Child("news_images").Child(imageName).GetDownloadUrlAsync();
        }
    }
}

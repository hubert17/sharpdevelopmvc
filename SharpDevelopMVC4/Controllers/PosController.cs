using System;
using System.DrawingCore;
using System.Web;
using System.Web.Mvc;
using ZXing.ZKWeb;

namespace ASPNETWebApp45.Controllers
{
	/// <summary>
	/// Description of PosController.
	/// </summary>
	public class PosController : Controller
	{
        [HttpGet]
        public ActionResult Index()
        {
        	return RedirectToAction("Reader");
        }
		
        [HttpGet]
        public ActionResult Reader()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reader(HttpPostedFileBase file)
        {
            string filename = "error";
            try
            {
                if (file != null)
                {
                    filename = file.SaveAsImageFile();
                    var barcodeReader = new BarcodeReader()
                    {
                        AutoRotate = true,
                        TryInverted = true
                    };

                    var barcodeBitmap = (Bitmap)Bitmap.FromFile(Server.MapPath("/uploadedimages/" + filename));
                    ViewBag.ImageSize = barcodeBitmap.Size.ToString();
                    var barcodeResult = barcodeReader.Decode(barcodeBitmap);
                    ViewBag.Barcode = barcodeResult.Text.ToString();
                    ViewBag.BarcodeUrl = filename;
                }
                else
                {
                    ViewBag.Barcode = "Barkod Resmi Ekleyiniz!";
                    ViewBag.BarcodeUrl = "";
                }
            }
            catch (Exception)
            {
                ViewBag.Barcode = "Barkod Okunamadı!";
                ViewBag.BarcodeUrl = filename;
            }
            return View();
        }

	}
}
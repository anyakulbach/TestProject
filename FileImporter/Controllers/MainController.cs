using System.Web.Mvc;
using FileImporter.Services;

namespace FileImporter.Controllers
{
	public class MainController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Save(string inputWord)
		{
			var (isValid, errorMessage) = FileService.ValidateFile(Request.Files, out var file);
			if (!isValid)
				return null;

			var filePath = FileService.LoadFile_StepOne(file);

			var text = System.IO.File.ReadAllText(filePath);
			var result = FileService.ProcessText(text, inputWord);

			//TODO : save to db

			return Json(new
			{
				isSuccess = true
			});
		}


	}
}
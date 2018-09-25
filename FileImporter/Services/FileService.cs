using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FileImporter.Services
{
	public static  class FileService
	{
		public static List<string> ProcessText(string inputText, string inputWord)
		{
			var sentences = inputText.Split('.');

			var dbRes = new List<string>();

			foreach (var sentence in sentences.Where(x => x.Contains(inputWord)))
			{
				var separators = new[] { "\r\n", " ", "," };

				var words = sentence.Split(separators, StringSplitOptions.None);
				if (words.Any(word => word == inputWord))
				{
					dbRes.Add(StringHelper.Reverse(sentence.Replace("\r\n", "")));
				}
			}

			return dbRes;
		}

		public static (bool isValid, string errorMessage) ValidateFileExistence(string fileName, out string fileTempPath)
		{
			fileTempPath = string.Empty;

			if (string.IsNullOrWhiteSpace(fileName))
				return (false, "File for import has not been selected.");

			fileTempPath = Path.Combine("filePath", fileName);
			if (!File.Exists(fileTempPath))
				return (false, "File does not exist in 'Temp' directory.");

			return (true, string.Empty);
		}

		public static (bool isValid, string errorMessage) ValidateFile(HttpFileCollectionBase requestFiles, out HttpPostedFileBase fileToImport)
		{
			fileToImport = null;

			if (requestFiles.Count <= 0)
				return (false, "There are no any file selected. Please select the file.");

			fileToImport = requestFiles[0];
			if (fileToImport == null)
				return (false, "File is not selected. Please select the file.");

			//var extension = Path.GetExtension(fileToImport.FileName);
			//if (!_importSetting.AllowedExtensions.Contains(extension))
			//	return (
			//		false,
			//		$"File type '{extension}' is not allowed. Please select the file with one of the following format '{_importSetting.AllowedFileExtensions}'.");

			if (fileToImport.ContentLength <= 0)
				return (false, "Selected file is empty.");

			return (true, string.Empty);
		}

		public static string LoadFile_StepOne(HttpPostedFileBase file)
		{
			var fileName = $"{DateTime.Now:yyyyMMdd-HHmmss} -.- {file.FileName}";
			var filePath = Path.Combine("filePath", fileName);

			Save(filePath, file);

			return filePath;
		}

		public static void Save(string path, HttpPostedFileBase file)
		{
			Delete(path);

			using (var fileStream = File.Create(path))
			{
				AppendToStream(file.InputStream, fileStream, 8 * 1024);
			}
		}

		public static void AppendToStream(Stream sourceStream, Stream targetStream, int bufferSizeBytes)
		{
			if (targetStream == null) throw new ArgumentNullException(nameof(targetStream));
			if (sourceStream == null) throw new ArgumentNullException(nameof(sourceStream));

			int len;
			var buffer = new byte[bufferSizeBytes];

			while ((len = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				targetStream.Write(buffer, 0, len);
				targetStream.Flush();
			}
		}

		public static void Delete(string path)
		{
			File.Delete(path);
		}
	}

	public static class StringHelper
	{
		public static string Reverse(string s)
		{
			var charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Reflection;
using System.Xml.Linq;

namespace ArtApp {
	public interface IPictureLibrary {
		string GetPathByUrl(string url);
	}

	public partial class PictureLibrary : IPictureLibrary, IWithSerialization {
		protected Dictionary<string, string> pictures;
		protected PictureNameGenerator nameGenerator;

		public PictureLibrary() {
			pictures = new Dictionary<string, string>();
			nameGenerator = new PictureNameGenerator();
		}

		public string GetPathByUrl(string url) {
			if (!Contains(url)) {
				Download(url);
			}

			if (!IsValidPicturePath(pictures[url])) {
				Redownload(url);
			}

			return pictures[url];
		}

		protected bool Contains(string url) {
			return pictures.ContainsKey(url);
		}

		protected bool IsValidPicturePath(string url) {
			return File.Exists(url);
		}

		protected void Redownload(in string url) {
			pictures.Remove(url);
			Download(url);
		}

		protected void Download(in string url) {
			CreatePictureDirectoryIfNotFound();

			byte[] picture = new WebClient().DownloadData(url);
			string name = nameGenerator.CreatePictureName(GetImageFormatFromUrl(url));
			SaveByteArrayToFile(picture, name);

			pictures.Add(url, name);
		}

		protected void SaveByteArrayToFile(byte[] picture, string name) {
			using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.Write)) {
				fs.Write(picture, 0, picture.Length);
			}
		}

		protected string GetImageFormatFromUrl(in string url) {
			int index = url.LastIndexOf('.');
			return url.Substring(index, url.Length - index);
		}

		protected void CreatePictureDirectoryIfNotFound() {
			DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory + "\\" + nameGenerator.Path);
			if (!directory.Exists)
				directory.Create();
		}
	}
}

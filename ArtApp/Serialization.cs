﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;

namespace ArtApp {
	public interface IWithSerialization {
		XmlNode CreateXMLNode(in XmlDocument xmlDocument);
		void LoadDataFromXmlNode(XmlNode node);

		void Save(string path);
		void Save(FileStream fs);
		void Save(BinaryWriter bw);

		void Load(string path);
		void Load(FileStream fs);
		void Load(BinaryReader br);
	}

	public partial class PictureController {
		public XmlNode CreateXMLNode(in XmlDocument xmlDocument) {
			XmlNode pictureControllerNode = xmlDocument.CreateElement("PictureController");

			pictureControllerNode.AppendChild(linkHistory.CreateXMLNode(xmlDocument));
			pictureControllerNode.AppendChild(library.CreateXMLNode(xmlDocument));

			XmlNode sourceNode = xmlDocument.CreateElement("Source");
			sourceNode.InnerText = source;
			pictureControllerNode.AppendChild(sourceNode);

			XmlNode regExPatternNode = xmlDocument.CreateElement("RegExPattern");
			regExPatternNode.InnerText = regExPattern;
			pictureControllerNode.AppendChild(regExPatternNode);

			return pictureControllerNode;
		}

		public void LoadDataFromXmlNode(XmlNode node) {
			linkHistory.LoadDataFromXmlNode(node["LinkHistory"]);
			library.LoadDataFromXmlNode(node["PictureLibrary"]);
			source = node["Source"].InnerText;
			regExPattern = node["RegExPattern"].InnerText;
		}

		public void Save(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				Save(fs);
			}
		}
		public void Save(FileStream fs) {
			using (BinaryWriter bw = new BinaryWriter(fs)) {
				Save(bw);
			}
		}
		public void Save(BinaryWriter bw) {
			linkHistory.Save(bw);
			library.Save(bw);
			bw.Write(source);
		}

		public void Load(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				Load(fs);
			}
		}
		public void Load(FileStream fs) {
			using (BinaryReader br = new BinaryReader(fs)) {
				Load(br);
			}
		}
		public void Load(BinaryReader br) {
			linkHistory.Load(br);
			library.Load(br);
			source = br.ReadString();
		}
	}

	public partial class LinkHistory {
		public XmlNode CreateXMLNode(in XmlDocument xmlDocument) {
			XmlNode linkHistoryNode = xmlDocument.CreateElement("LinkHistory");

			XmlNode urlListNode = xmlDocument.CreateElement("URLList");
			foreach (string url in urlList) {
				XmlNode urlNode = xmlDocument.CreateElement("URL");
				urlNode.InnerText = url;
				urlListNode.AppendChild(urlNode);
			}
			linkHistoryNode.AppendChild(urlListNode);

			XmlNode indexNode = xmlDocument.CreateElement("Index");
			indexNode.InnerText = index.ToString();
			linkHistoryNode.AppendChild(indexNode);

			return linkHistoryNode;
		}

		public void LoadDataFromXmlNode(XmlNode node) {
			urlList.Clear();
			foreach (XmlNode urlListChild in node["URLList"].ChildNodes) {
				urlList.Add(urlListChild.InnerText);
			}

			index = Convert.ToInt32(node["Index"].InnerText);
		}

		public void Save(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				Save(fs);
			}
		}
		public void Save(FileStream fs) {
			using (BinaryWriter bw = new BinaryWriter(fs)) {
				Save(bw);
			}
		}
		public void Save(BinaryWriter bw) {
			bw.Write(urlList.Count);
			foreach (string url in urlList) {
				bw.Write(url);
			}
			bw.Write(index);
		}

		public void Load(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				Load(fs);
			}
		}
		public void Load(FileStream fs) {
			using (BinaryReader br = new BinaryReader(fs)) {
				Load(br);
			}
		}
		public void Load(BinaryReader br) {
			int urlListCount = br.ReadInt32();
			urlList.Clear();
			for (int i = 0; i < urlListCount; i++) {
				urlList.Add(br.ReadString());
			}
			index = br.ReadInt32();
		}
	}

	public partial class PictureLibrary {
		public XmlNode CreateXMLNode(in XmlDocument xmlDocument) {
			XmlNode pictureLibraryNode = xmlDocument.CreateElement("PictureLibrary");

			XmlNode picturesNode = xmlDocument.CreateElement("Pictures");
			foreach (KeyValuePair<string, string> picturePair in pictures) {
				XmlNode pictureNode = xmlDocument.CreateElement("Picture");

				XmlNode urlNode = xmlDocument.CreateElement("URL");
				urlNode.InnerText = picturePair.Key;
				pictureNode.AppendChild(urlNode);

				XmlNode pathNode = xmlDocument.CreateElement("Path");
				pathNode.InnerText = picturePair.Value;
				pictureNode.AppendChild(pathNode);

				picturesNode.AppendChild(pictureNode);
			}
			pictureLibraryNode.AppendChild(picturesNode);

			pictureLibraryNode.AppendChild(nameGenerator.CreateXMLNode(xmlDocument));

			return pictureLibraryNode;
		}

		public void LoadDataFromXmlNode(XmlNode node) {
			pictures.Clear();
			foreach (XmlNode picturesChild in node["Pictures"].ChildNodes) {
				pictures.Add(picturesChild["URL"].InnerText, picturesChild["Path"].InnerText);
			}

			nameGenerator.LoadDataFromXmlNode(node["PictureNameGenerator"]);
		}

		public void Save(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				Save(fs);
			}
		}
		public void Save(FileStream fs) {
			using (BinaryWriter bw = new BinaryWriter(fs)) {
				Save(bw);
			}
		}
		public void Save(BinaryWriter bw) {
			bw.Write(pictures.Count);
			foreach (KeyValuePair<string, string> pair in pictures) {
				bw.Write(pair.Key);
				bw.Write(pair.Value);
			}
			nameGenerator.Save(bw);
		}

		public void Load(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				Load(fs);
			}
		}
		public void Load(FileStream fs) {
			using (BinaryReader br = new BinaryReader(fs)) {
				Load(br);
			}
		}
		public void Load(BinaryReader br) {
			int picturesCount = br.ReadInt32();
			pictures.Clear();
			for (int i = 0; i < picturesCount; i++) {
				string key = br.ReadString();
				string value = br.ReadString();

				pictures.Add(key, value);
			}
			nameGenerator.Load(br);
		}
	}

	public partial class PictureNameGenerator {
		public XmlNode CreateXMLNode(in XmlDocument xmlDocument) {
			XmlNode pictureNameGeneratorNode = xmlDocument.CreateElement("PictureNameGenerator");

			XmlNode pathNode = xmlDocument.CreateElement("Path");
			pathNode.InnerText = path;
			pictureNameGeneratorNode.AppendChild(pathNode);

			XmlNode indexNode = xmlDocument.CreateElement("Index");
			indexNode.InnerText = index.ToString();
			pictureNameGeneratorNode.AppendChild(indexNode);

			return pictureNameGeneratorNode;
		}

		public void LoadDataFromXmlNode(XmlNode node) {
			path = node["Path"].InnerText;
			index = Convert.ToInt32(node["Index"].InnerText);
		}

		public void Save(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				Save(fs);
			}
		}
		public void Save(FileStream fs) {
			using (BinaryWriter bw = new BinaryWriter(fs)) {
				Save(bw);
			}
		}
		public void Save(BinaryWriter bw) {
			bw.Write(path);
			bw.Write(index);
		}

		public void Load(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				Load(fs);
			}
		}
		public void Load(FileStream fs) {
			using (BinaryReader br = new BinaryReader(fs)) {
				Load(br);
			}
		}
		public void Load(BinaryReader br) {
			path = br.ReadString();
			index = br.ReadInt32();
		}
	}
}

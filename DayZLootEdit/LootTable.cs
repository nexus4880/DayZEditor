using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace DayZLootEdit {
	internal class LootTable {
		public LootTable(String file) {
			this.FilePath = file;
		}

		public String FilePath { get; }
		public XElement XML { get; private set; }
		public ObservableCollection<LootType> Loot { get; } = new ObservableCollection<LootType>();

		public void LoadFile() {
			this.XML = XElement.Load(this.FilePath);
			this.Loot.Clear();
			foreach (XElement node in this.XML.Elements()) {
				this.Loot.Add(new LootType(node));
			}
		}

		public void SaveFile() {
			FileInfo file = new FileInfo(this.FilePath);
			if (file.Exists) {
				FileInfo backup = new FileInfo(file.FullName + ".original.xml");
				if (!backup.Exists) {
					file.CopyTo(backup.FullName);
				}
			}

			this.XML.Save(this.FilePath);
		}
	}
}
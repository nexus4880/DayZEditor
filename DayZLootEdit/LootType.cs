using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace DayZLootEdit {
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public class LootType {
		private XElement xtype;

		public LootType(XElement xnode) {
			this.xtype = xnode;
		}

		public String Name {
			get {
				return this.xtype.Attribute("name")?.Value;
			}
			set {
				this.xtype.Attribute("name").Value = value;
			}
		}

		public String Category {
			get {
				return this.xtype.Element("category")?.Attribute("name").Value;
			}
			set {
				this.xtype.Element("category")?.Attribute("name")?.SetValue(value);
			}
		}

		public Int32 Nominal {
			get {
				return GetValueInt(this.xtype, "nominal");
			}
			set {
				this.xtype.Element("nominal")?.SetValue(value.ToString());
			}
		}

		public Int32 Lifetime {
			get {
				return this.Lifetime = GetValueInt(this.xtype, "lifetime");
			}
			set {
				this.xtype.Element("lifetime")?.SetValue(value.ToString());
			}
		}

		public Int32 Restock {
			get {
				return GetValueInt(this.xtype, "restock");
			}
			set {
				this.xtype.Element("restock")?.SetValue(value.ToString());
			}
		}

		public Int32 Min {
			get {
				return GetValueInt(this.xtype, "min");
			}
			set {
				this.xtype.Element("min")?.SetValue(value.ToString());
			}
		}

		public Int32 QuantMin {
			get {
				return GetValueInt(this.xtype, "quantmin");
			}
			set {
				this.xtype.Element("quantmin")?.SetValue(value.ToString());
			}
		}

		public Int32 QuantMax {
			get {
				return GetValueInt(this.xtype, "quantmax");
			}
			set {
				this.xtype.Element("quantmax")?.SetValue(value.ToString());
			}
		}

		public Int32 Cost {
			get {
				return GetValueInt(this.xtype, "cost");
			}
			set {
				this.xtype.Element("cost")?.SetValue(value.ToString());
			}
		}

		public Boolean CountInCargo {
			get {
				return GetFlag(this.xtype, "count_in_cargo");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_cargo")?.SetValue(value ? "1" : "0");
			}
		}

		public Boolean CountInHoarder {
			get {
				return GetFlag(this.xtype, "count_in_hoarder");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_hoarder")?.SetValue(value ? "1" : "0");
			}
		}

		public Boolean CountInMap {
			get {
				return GetFlag(this.xtype, "count_in_map");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_map")?.SetValue(value ? "1" : "0");
			}
		}

		public Boolean CountInPlayer {
			get {
				return GetFlag(this.xtype, "count_in_player");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_player")?.SetValue(value ? "1" : "0");
			}
		}

		public Boolean Crafted {
			get {
				return GetFlag(this.xtype, "crafted");
			}
			set {
				this.xtype.Element("flags")?.Attribute("crafted")?.SetValue(value ? "1" : "0");
			}
		}

		public Boolean Deloot {
			get {
				return GetFlag(this.xtype, "deloot");
			}
			set {
				this.xtype.Element("flags")?.Attribute("deloot")?.SetValue(value ? "1" : "0");
			}
		}

		public String Usage {
			get {
				return String.Join(", ", this.xtype.Elements().Where(node => node.Name.LocalName.Equals("usage")).Select(node => node.Attribute("name")?.Value));
			}
			set {
				this.xtype.Elements().Where(node => node.Name.LocalName.Equals("usage")).Remove();
				foreach (String s in value.Split(',').Select(s => s.Trim())) {
					this.xtype.Add(new XElement("usage", new XAttribute("name", s)));
				}
			}
		}

		public String Value {
			get {
				return String.Join(", ", this.xtype.Elements().Where(node => node.Name.LocalName.Equals("usage")).Select(node => node.Attribute("name")?.Value));
			}
			set {
				this.xtype.Elements().Where(node => node.Name.LocalName.Equals("value")).Remove();
				foreach (String s in value.Split(',').Select(s => s.Trim())) {
					this.xtype.Add(new XElement("value", new XAttribute("name", s)));
				}
			}
		}

		private static Int32 GetValueInt(XContainer node, String name) {
			Int32.TryParse(node.Element(name)?.Value, out Int32 val);
			return val;
		}

		private static Boolean GetFlag(XContainer node, String attrib) {
			return node.Element("flags")?.Attribute(attrib)?.Value == "1";
		}

		public void SetNominal(Int32 percentage) {
			this.Nominal = (Int32)Math.Round(this.Nominal / 100.0 * percentage);
		}

		public void RemoveType() {
			this.xtype.Remove();
		}
	}
}
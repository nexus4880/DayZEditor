using System;
using System.Linq;
using System.Xml.Linq;

namespace DayZLootEdit {
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
				return this.GetValueInt(this.xtype, "nominal");
			}
			set {
				this.xtype.Element("nominal")?.SetValue(value.ToString());
			}
		}

		public Int32 Lifetime {
			get {
				return this.Lifetime = this.GetValueInt(this.xtype, "lifetime");
			}
			set {
				this.xtype.Element("lifetime")?.SetValue(value.ToString());
			}
		}

		public Int32 Restock {
			get {
				return this.GetValueInt(this.xtype, "restock");
			}
			set {
				this.xtype.Element("restock")?.SetValue(value.ToString());
			}
		}

		public Int32 Min {
			get {
				return this.GetValueInt(this.xtype, "min");
			}
			set {
				this.xtype.Element("min")?.SetValue(value.ToString());
			}
		}

		public Int32 QuantMin {
			get {
				return this.GetValueInt(this.xtype, "quantmin");
			}
			set {
				this.xtype.Element("quantmin")?.SetValue(value.ToString());
			}
		}

		public Int32 QuantMax {
			get {
				return this.GetValueInt(this.xtype, "quantmax");
			}
			set {
				this.xtype.Element("quantmax")?.SetValue(value.ToString());
			}
		}

		public Int32 Cost {
			get {
				return this.GetValueInt(this.xtype, "cost");
			}
			set {
				this.xtype.Element("cost")?.SetValue(value.ToString());
			}
		}

		public Boolean CountInCargo {
			get {
				return this.GetFlag(this.xtype, "count_in_cargo");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_cargo")?.SetValue(value.ToString());
			}
		}

		public Boolean CountInHoarder {
			get {
				return this.GetFlag(this.xtype, "count_in_hoarder");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_hoarder")?.SetValue(value.ToString());
			}
		}

		public Boolean CountInMap {
			get {
				return this.GetFlag(this.xtype, "count_in_map");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_map")?.SetValue(value.ToString());
			}
		}

		public Boolean CountInPlayer {
			get {
				return this.GetFlag(this.xtype, "count_in_player");
			}
			set {
				this.xtype.Element("flags")?.Attribute("count_in_player")?.SetValue(value.ToString());
			}
		}

		public Boolean Crafted {
			get {
				return this.GetFlag(this.xtype, "crafted");
			}
			set {
				this.xtype.Element("flags")?.Attribute("crafted")?.SetValue(value.ToString());
			}
		}

		public Boolean Deloot {
			get {
				return this.GetFlag(this.xtype, "deloot");
			}
			set {
				this.xtype.Element("flags")?.Attribute("deloot")?.SetValue(value.ToString());
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

		private Int32 GetValueInt(XElement node, String name) {
			Int32 val = 0;
			Int32.TryParse(node.Element(name)?.Value, out val);
			return val;
		}

		private Boolean GetFlag(XElement node, String attrib) {
			return (Boolean)node.Element("flags")?.Attribute(attrib)?.Value.Equals("1");
		}

		public void SetNominal(Int32 percentage) {
			this.Nominal = (Int32)Math.Round(this.Nominal / 100.0 * percentage);
		}

		public void RemoveType() {
			this.xtype.Remove();
		}

		/*
		<type name="AKM">
			<nominal>40</nominal>
			<lifetime>10800</lifetime>
			<restock>1800</restock>
			<min>20</min>
			<quantmin>-1</quantmin>
			<quantmax>-1</quantmax>
			<cost>100</cost>
			<flags count_in_cargo="1" count_in_hoarder="1" count_in_map="1" count_in_player="0" crafted="0" deloot="0"/>
			<category name="weapons"/>
			<usage name="Military"/>
		</type>
		 */
	}
}
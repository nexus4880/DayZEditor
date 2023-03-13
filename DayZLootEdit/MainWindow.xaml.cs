using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace DayZLootEdit {
	/// <summary>
	///     Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private static HashSet<String> _attachmentTypes = new HashSet<String> {
			"Hndgrd",
			"Bttstck",
			"Bayonet",
			"Compensator",
			"NVGoggles",
			"Optic",
			"Light",
			"Suppressor"
		};

		private static Func<LootTable, IEnumerable<LootType>>[] Filters = {
			FilterWeapons, FilterClothes, FilterFood, FilterExplosives, FilterContainers, FilterTools, FilterVehicleParts, FilterUnCategorized, FilterAmmo, FilterAttachments
		};

		private Int32? _filterMethod;
		private LootTable _lootTable;

		public MainWindow() {
			this.InitializeComponent();
		}

		private void LoadBtn_Click(Object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
			if (dlg.ShowDialog().Value) {
				this._lootTable = new LootTable(dlg.FileName);
				this._lootTable.LoadFile();
				this.LootList.ItemsSource = this._lootTable.Loot;
				this.LootList.IsEnabled = true;
				this.SaveBtn.IsEnabled = true;
			}
		}

		private void LootList_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
			this.LootPercBox.IsEnabled = this.LootList.SelectedItems.Count > 0;
		}

		private void PercBtn_Click(Object sender, RoutedEventArgs e) {
			if (!Int32.TryParse(this.PercBox.Text.Replace("%", ""), out Int32 percentage)) {
				return;
			}

			foreach (LootType loot in this.LootList.SelectedItems) {
				loot.SetNominal(percentage);
			}

			this.LootList.Items.Refresh();
			this.PercBox.Text = "0";
			this.UpdatePercValue();
		}

		private void PercSilder_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e) {
			this.PercBox.Text = Math.Round(this.PercSilder.Value).ToString(CultureInfo.CurrentCulture);
			this.UpdatePercValue();
		}

		private void PercBox_GotFocus(Object sender, RoutedEventArgs e) {
			this.PercBox.SelectAll();
		}

		private void PercBox_LostFocus(Object sender, RoutedEventArgs e) {
			this.UpdatePercValue();
		}

		private void PercBox_KeyUp(Object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				this.UpdatePercValue();
			}
		}

		private void UpdatePercValue() {
			if (Int32.TryParse(this.PercBox.Text.Replace("%", ""), out Int32 newval)) {
				this.PercSilder.Value = newval;
				this.PercBox.Text = $"{newval}%";
				this.PercBtn?.Focus();
			}
		}

		private void SaveBtn_Click(Object sender, RoutedEventArgs e) {
			this._lootTable.SaveFile();
		}

		private void PreviewKeyDownHandler(Object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Delete:
				case Key.Back:
					foreach (LootType loot in this.LootList.SelectedItems) {
						loot.RemoveType();
					}

					break;
			}
		}

		private void ChangeSearchFilter(Object sender, TextChangedEventArgs e) {
			if (!(sender is TextBox textBox)) {
				return;
			}

			this.LootList.ItemsSource = new ObservableCollection<LootType>(this.GetItemsFromFilter().Where(l => StrContains(l.Name, textBox.Text, StringComparison.CurrentCultureIgnoreCase)));
		}

		public static Boolean StrContains(String source, String toCheck, StringComparison comp) {
			return source?.IndexOf(toCheck, comp) >= 0;
		}

		private IEnumerable<LootType> GetItemsFromFilter() {
			if (this._lootTable == null) {
				return null;
			}

			return this._lootTable == null ? null : this._filterMethod.HasValue ? Filters[this._filterMethod.Value](this._lootTable) : this._lootTable.Loot;
		}

		private void UpdateDataGrid() {
			this.LootList.ItemsSource = new ObservableCollection<LootType>(this.GetItemsFromFilter());
		}

#region Filter Methods
		private static IEnumerable<LootType> FilterWeapons(LootTable lootTable) {
			return lootTable.Loot.Where(x => x.Category == "weapons" && !(x.Name.StartsWith("Mag_") || x.Name.StartsWith("Ammo") || _attachmentTypes.Any(weapon => x.Name.Contains(weapon))));
		}

		private static IEnumerable<LootType> FilterClothes(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "clothes");
		}

		private static IEnumerable<LootType> FilterFood(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "food");
		}

		private static IEnumerable<LootType> FilterExplosives(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "explosives");
		}

		private static IEnumerable<LootType> FilterContainers(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "containers");
		}

		private static IEnumerable<LootType> FilterTools(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "tools");
		}

		private static IEnumerable<LootType> FilterVehicleParts(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "vehiclesparts");
		}

		private static IEnumerable<LootType> FilterUnCategorized(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == null);
		}

		private static IEnumerable<LootType> FilterAmmo(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "weapons" && item.Name.StartsWith("Ammo"));
		}

		private static IEnumerable<LootType> FilterAttachments(LootTable lootTable) {
			return lootTable.Loot.Where(item => item.Category == "weapons" && _attachmentTypes.Any(x => item.Name.Contains(x) || item.Name.StartsWith("Mag")));
		}
#endregion

#region Filter Button Events
		private void WeaponsTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 0;
			this.UpdateDataGrid();
		}

		private void ClothesTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 1;
			this.UpdateDataGrid();
		}

		private void FoodTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 2;
			this.UpdateDataGrid();
		}

		private void ExplosivesTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 3;
			this.UpdateDataGrid();
		}

		private void ContainersTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 4;
			this.UpdateDataGrid();
		}

		private void ToolsTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 5;
			this.UpdateDataGrid();
		}

		private void VehiclePartsTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 6;
			this.UpdateDataGrid();
		}

		private void UnCategorizedTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 7;
			this.UpdateDataGrid();
		}

		private void AmmoTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 8;
			this.UpdateDataGrid();
		}

		private void AttachmentsTab_Click(Object sender, RoutedEventArgs e) {
			this._filterMethod = 9;
			this.UpdateDataGrid();
		}

		private void RemoveFilterTab_OnClick(Object sender, RoutedEventArgs e) {
			this._filterMethod = null;
			this.UpdateDataGrid();
		}
#endregion
	}
}
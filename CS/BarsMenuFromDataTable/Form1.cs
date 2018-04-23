using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;

namespace BarsMenuFromDataTable
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private BarItem[] CreateChildrenLinks(BarItem parentItem, DataTable childrenTable)
		{
			List<BarItem> items = new List<BarItem>();
			foreach ( DataRow row in childrenTable.Rows )
			{
				BarItem barItem = null;

				int childrenCount = childrenTable.AsEnumerable().Count(r => (int)r["ParentId"] == (int)row["Id"]);
				if ( childrenCount > 0 )
				{
					barItem = new BarSubItem();
					DataTable table = childrenTable.AsEnumerable().Where(r => (int)r["ParentId"] == (int)row["Id"]).CopyToDataTable();
					((BarSubItem)barItem).AddItems(CreateChildrenLinks(barItem, table));
				}
				else
				{
					BarItemInfo itemInfo = (BarItemInfo)row["ItemKind"];
					barItem = System.Activator.CreateInstance(itemInfo.ItemType) as BarItem;
				}

				barItem.Name = ((string)row["Caption"]).ToLower();
				barItem.Caption = (string)row["Caption"];
				barItem.Manager = barManager1;

				items.Add(barItem);
			}

			return items.ToArray();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			DataTable table = CreateTable(30);
			gridControl1.DataSource = table;

			foreach ( DataRow row in table.Select("ParentId = -1", "Id asc") )
			{
				BarItem barItem = null;

				int childrenCount = table.AsEnumerable().Count(r => (int)r["ParentId"] == (int)row["Id"]);
				if ( childrenCount > 0 )
				{
					barItem = new BarSubItem();
					DataTable childrenTable = table.AsEnumerable().Where(r => (int)r["ParentId"] == (int)row["Id"]).CopyToDataTable();
					((BarSubItem)barItem).AddItems(CreateChildrenLinks(barItem, childrenTable));
				}
				else
				{
					BarItemInfo itemInfo = (BarItemInfo)row["ItemKind"];
					barItem = System.Activator.CreateInstance(itemInfo.ItemType) as BarItem;
				}

				barItem.Name = ((string)row["Caption"]).ToLower();
				barItem.Caption = (string)row["Caption"];
				barItem.Manager = barManager1;

				bar1.AddItem(barItem);
			}
		}

		private DataTable CreateTable(int RowCount)
		{
			BarManagerPaintStyle paintStyle = barManager1.GetController().PaintStyle;
			BarItemInfoCollection itemsCollection = new BarItemInfoCollection(paintStyle);

			for ( int i = 0; i < 5; i++ )
				if ( paintStyle.ItemInfoCollection[i].ItemType != typeof(BarSubItem) )
					itemsCollection.Add(paintStyle.ItemInfoCollection[i]);

			DataTable tbl = new DataTable();
			tbl.Columns.Add("Id", typeof(int));
			tbl.Columns.Add("ParentId", typeof(int));
			tbl.Columns.Add("Caption", typeof(string));
			tbl.Columns.Add("ItemKind", typeof(BarItemInfo));

			Random rnd = new Random();
			for ( int i = 0; i < RowCount; i++ )
			{
				int parentId = rnd.Next(RowCount * -1, RowCount);
				if ( parentId == i )
					parentId--;

				tbl.Rows.Add(new object[] { i + 1, parentId <= 0 ? -1 : parentId , String.Format("Caption {0}", i),
											itemsCollection[rnd.Next(0, itemsCollection.Count - 1)] });
			}
						
			return tbl;
		}
	}
}

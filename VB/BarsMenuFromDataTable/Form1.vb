Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows.Forms
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Styles
Imports Microsoft.VisualBasic

Namespace BarsMenuFromDataTable
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Function CreateChildrenLinks(ByVal parentItem As BarItem, ByVal childrenTable As DataTable) As BarItem()
			Dim items As New List(Of BarItem)()
			For Each row As DataRow In childrenTable.Rows
				Dim barItem As BarItem = Nothing

				Dim childrenCount As Integer = childrenTable.AsEnumerable().Count(Function(r) CInt(Fix(r("ParentId"))) = CInt(Fix(row("Id"))))
				If childrenCount > 0 Then
					barItem = New BarSubItem()
					Dim table As DataTable = childrenTable.AsEnumerable().Where(Function(r) CInt(Fix(r("ParentId"))) = CInt(Fix(row("Id")))).CopyToDataTable()
					CType(barItem, BarSubItem).AddItems(CreateChildrenLinks(barItem, table))
				Else
					Dim itemInfo As BarItemInfo = CType(row("ItemKind"), BarItemInfo)
					barItem = TryCast(System.Activator.CreateInstance(itemInfo.ItemType), BarItem)
				End If

				barItem.Name = (CStr(row("Caption"))).ToLower()
				barItem.Caption = CStr(row("Caption"))
				barItem.Manager = barManager1

				items.Add(barItem)
			Next row

			Return items.ToArray()
		End Function

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			Dim table As DataTable = CreateTable(30)
			gridControl1.DataSource = table

			For Each row As DataRow In table.Select("ParentId = -1", "Id asc")
				Dim barItem As BarItem = Nothing

				Dim childrenCount As Integer = table.AsEnumerable().Count(Function(r) CInt(Fix(r("ParentId"))) = CInt(Fix(row("Id"))))
				If childrenCount > 0 Then
					barItem = New BarSubItem()
					Dim childrenTable As DataTable = table.AsEnumerable().Where(Function(r) CInt(Fix(r("ParentId"))) = CInt(Fix(row("Id")))).CopyToDataTable()
					CType(barItem, BarSubItem).AddItems(CreateChildrenLinks(barItem, childrenTable))
				Else
					Dim itemInfo As BarItemInfo = CType(row("ItemKind"), BarItemInfo)
					barItem = TryCast(System.Activator.CreateInstance(itemInfo.ItemType), BarItem)
				End If

				barItem.Name = (CStr(row("Caption"))).ToLower()
				barItem.Caption = CStr(row("Caption"))
				barItem.Manager = barManager1

				bar1.AddItem(barItem)
			Next row
		End Sub

		Private Function CreateTable(ByVal RowCount As Integer) As DataTable
			Dim paintStyle As BarManagerPaintStyle = barManager1.GetController().PaintStyle
			Dim itemsCollection As New BarItemInfoCollection(paintStyle)

			For i As Integer = 0 To 4
				If paintStyle.ItemInfoCollection(i).ItemType IsNot GetType(BarSubItem) Then
					itemsCollection.Add(paintStyle.ItemInfoCollection(i))
				End If
			Next i

			Dim tbl As New DataTable()
			tbl.Columns.Add("Id", GetType(Integer))
			tbl.Columns.Add("ParentId", GetType(Integer))
			tbl.Columns.Add("Caption", GetType(String))
			tbl.Columns.Add("ItemKind", GetType(BarItemInfo))

			Dim rnd As New Random()
			For i As Integer = 0 To RowCount - 1
				Dim parentId As Integer = rnd.Next(RowCount * -1, RowCount)
				If parentId = i Then
					parentId -= 1
				End If

				tbl.Rows.Add(New Object() { i + 1,If(parentId <= 0, -1, parentId), String.Format("Caption {0}", i), itemsCollection(rnd.Next(0, itemsCollection.Count - 1)) })
			Next i

			Return tbl
		End Function
	End Class
End Namespace

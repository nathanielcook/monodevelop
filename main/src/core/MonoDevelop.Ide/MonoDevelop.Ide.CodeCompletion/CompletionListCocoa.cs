//
// CompletionListCocoa.cs
//
// Author:
//       iain <iain@falsevictories.com>
//
// Copyright (c) 2018 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#if MAC

using System;
using System.Collections.Generic;
using AppKit;
using CoreGraphics;
using Foundation;
using Microsoft.Build.Exceptions;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Mac;

namespace MonoDevelop.Ide.CodeCompletion
{
	public class CompletionListCocoa : NSTableView
	{
		public CompletionListCocoa ()
		{
			HeaderView = null;
			BackgroundColor = Styles.CodeCompletion.BackgroundColor.ToNSColor ();

			var column = new NSTableColumn ("");
			column.Width = 400;
			AddColumn (column);

			Delegate = new CompletionListDelegate ();
			DataSource = new CompletionListDataSource ();
			(Delegate as CompletionListDelegate).FilteredItems = filteredItems;
			(DataSource as CompletionListDataSource).FilteredItems = filteredItems;
		}

		IListDataProvider provider;
		internal IListDataProvider DataProvider {
			get => provider;
			set {
				provider = value;
				(Delegate as CompletionListDelegate).Provider = value;
				(DataSource as CompletionListDataSource).Provider = value;

				ReloadData ();
			}
		}

		List<CategorizedCompletionItems> categories = new List<CategorizedCompletionItems> ();
		List<int> filteredItems = new List<int> ();
		internal void ShowFilteredItems (CompletionListFilterResult filterResult)
		{
			filteredItems = filterResult.FilteredItems;
			if (filterResult.CategorizedItems == null) {
				categories.Clear ();
			} else {
				categories = filterResult.CategorizedItems;
			}

			(Delegate as CompletionListDelegate).FilteredItems = filteredItems;
			(DataSource as CompletionListDataSource).FilteredItems = filteredItems;
			ReloadData ();
		}

		internal void ResetState ()
		{
			categories.Clear ();
			filteredItems.Clear ();
		}

		// FIXME: Optimise.
		int RowFromItemIdx (int index)
		{
			int i = 0;
			foreach (var idx in filteredItems) {
				i++;
				if (index == idx) {
					return i;
				}
			}

			return -1;
		}

		internal int SelectedIndex {
			get {
				if (SelectedRow < 0) {
					return -1;
				}

				return filteredItems[(int)SelectedRow];
			}

			set {
				if (value < 0) {
					SelectRow (-1, false);
					return;
				}

				var row = RowFromItemIdx (value);
				SelectRow (row, false);

				ScrollRowToVisible (row);
			}
		}
	}

	class CompletionCellView : NSTableCellView
	{
		public CompletionCellView ()
		{
			// FIXME: Can we use CreateLabel?
			TextField = NSTextField.CreateLabel ("");
			TextField.Font = NSFont.UserFixedPitchFontOfSize (12);

			TextField.TranslatesAutoresizingMaskIntoConstraints = false;
			ImageView = new NSImageView ();
			ImageView.TranslatesAutoresizingMaskIntoConstraints = false;

			AddSubview (TextField);
			AddSubview (ImageView);

			var viewsDict = new NSDictionary ("imageView", ImageView, "textField", TextField);
			var constraints = NSLayoutConstraint.FromVisualFormat ("|-6-[imageView(==16)]-4-[textField]-6-|", NSLayoutFormatOptions.AlignAllCenterY, null, viewsDict);
			AddConstraints (constraints);

			constraints = NSLayoutConstraint.FromVisualFormat ("V:|[imageView(==16)]|", NSLayoutFormatOptions.None, null, viewsDict);
			AddConstraints (constraints);

		}
	}
	class CompletionListDelegate : NSTableViewDelegate
	{
		internal List<int> FilteredItems { get; set; }
		internal IListDataProvider Provider { get; set; }
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			if (Provider == null || FilteredItems == null) {
				return null;
			}

			var filteredRow = FilteredItems [(int)row];
			var r = new CompletionCellView ();

			string markup = Provider.HasMarkup (filteredRow) ? (Provider.GetMarkup (filteredRow) ?? "&lt;null&gt;") : GLib.Markup.EscapeText (Provider.GetText (filteredRow) ?? "<null>");
			string description = Provider.GetDescription (filteredRow, tableView.SelectedRow == row);

			if (!string.IsNullOrEmpty (description)) {
				markup = markup + " " + description;
			}

			r.TextField.AttributedStringValue = AttibutedStringUtils.NSAttributedStringFromMarkup (markup);
			Console.WriteLine ($"Markup: {markup} - {r.TextField.AttributedStringValue}");
			r.ImageView.Image = Provider.GetIcon (filteredRow).ToNSImage ();
			return r;
		}
	}

	class CompletionListDataSource : NSTableViewDataSource
	{
		internal List<int> FilteredItems { get; set; }
		internal IListDataProvider Provider { get; set; }
		public override nint GetRowCount (NSTableView tableView)
		{
			return FilteredItems != null ? FilteredItems.Count : 0;
		}
	}
}

#endif
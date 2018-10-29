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
using AppKit;
using CoreGraphics;
using Foundation;

namespace MonoDevelop.Ide.CodeCompletion
{
	public class CompletionListCocoa : NSTableView
	{
		public CompletionListCocoa ()
		{
			var column = new NSTableColumn ("Suggestion");
			column.Width = 400;
			AddColumn (column);

			Delegate = new CompletionListDelegate ();
			DataSource = new CompletionListDataSource ();
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			NSColor.Red.Set ();
			NSBezierPath.FillRect (dirtyRect);
			base.DrawRect (dirtyRect);
		}
	}

	class CompletionCellView : NSTableCellView
	{
		public CompletionCellView ()
		{
			TextField = NSTextField.CreateLabel ("");
			TextField.TranslatesAutoresizingMaskIntoConstraints = false;
			ImageView = new NSImageView ();
			ImageView.TranslatesAutoresizingMaskIntoConstraints = false;

			AddSubview (TextField);
			AddSubview (ImageView);

			var viewsDict = new NSDictionary ("imageView", ImageView, "textField", TextField);
			var constraints = NSLayoutConstraint.FromVisualFormat ("|[imageView(==16)][textField]|", NSLayoutFormatOptions.AlignAllCenterY, null, viewsDict);
			AddConstraints (constraints);

			constraints = NSLayoutConstraint.FromVisualFormat ("V:|[imageView(==16)]|", NSLayoutFormatOptions.None, null, viewsDict);
			AddConstraints (constraints);

		}
	}
	class CompletionListDelegate : NSTableViewDelegate
	{
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var r = new CompletionCellView ();
			r.TextField.StringValue = "test";

			return r;
		}
	}

	class CompletionListDataSource : NSTableViewDataSource
	{
		public override nint GetRowCount (NSTableView tableView)
		{
			return 3;
		}
	}
}

#endif
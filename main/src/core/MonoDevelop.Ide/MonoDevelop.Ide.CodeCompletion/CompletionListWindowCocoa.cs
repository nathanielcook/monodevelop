//
// CompletionListWindowCocoa.cs
//
// Author:
//       iain <iaholmes@microsoft.com>
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
using Gtk;
using Xwt;
using CoreGraphics;

namespace MonoDevelop.Ide.CodeCompletion
{
	internal class CompletionListWindowCocoa : NSWindow, ICompletionView
	{
		CompletionListViewController viewController;
		ICompletionViewEventSink eventSink;

		public CompletionListWindowCocoa ()
		{
			viewController = new CompletionListViewController ();
			ContentViewController = viewController;

			StyleMask = NSWindowStyle.Borderless | NSWindowStyle.FullSizeContentView;
			TitleVisibility = NSWindowTitleVisibility.Hidden;
			TitlebarAppearsTransparent = true;
		}

		public int SelectedItemIndex {
			get => -1;
			set { return; }
		}
		public bool InCategoryMode { get => true; set { return; } }
		public bool SelectionEnabled { get => true; set { return; } }

		public bool Visible => false;

		public Rectangle Allocation => new Rectangle (Frame.X, Frame.Y, Frame.Width, Frame.Height);

		public int X => (int)Frame.X;

		public int Y => (int)Frame.Y;

		public Gtk.Window TransientFor {
			get => null;
			set {
				return;
			}
		}

		public void Destroy ()
		{
		}

		public void Hide ()
		{
			OrderOut (this);
		}

		public void HideLoadingMessage ()
		{
		}

		public void Initialize (IListDataProvider provider, ICompletionViewEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public void MoveCursor (int relative)
		{
		}

		public void PageDown ()
		{
		}

		public void PageUp ()
		{
		}

		public void Reposition (int triggerX, int triggerY, int triggerHeight, bool force)
		{
		}

		public bool RepositionDeclarationViewWindow (TooltipInformationWindow declarationviewwindow, int selectedItem)
		{
			return true;
		}

		public void RepositionWindow (Rectangle? r)
		{
			if (!r.HasValue) {
				return;
			}

			SetFrameTopLeftPoint (new CGPoint (r.Value.X, r.Value.Y));
		}

		public void ResetSizes ()
		{
		}

		public void ResetState ()
		{
		}

		public void Show ()
		{
			OrderFront (this);
		}

		public void ShowFilteredItems (CompletionListFilterResult filterResult)
		{
		}

		public void ShowLoadingMessage ()
		{
		}

		public void ShowPreviewCompletionEntry ()
		{
		}

		class CompletionListViewController : NSViewController
		{
			NSTableView listView;

			public override void LoadView ()
			{
				View = new NSView (new CGRect (0, 0, 400, 500));

				listView = new NSTableView ();
				listView.TranslatesAutoresizingMaskIntoConstraints = false;

				View.AddSubview (listView);

				listView.TopAnchor.ConstraintEqualToAnchor (View.TopAnchor);
				listView.BottomAnchor.ConstraintEqualToAnchor (View.BottomAnchor);
				listView.LeadingAnchor.ConstraintEqualToAnchor (View.LeadingAnchor);
				listView.TrailingAnchor.ConstraintEqualToAnchor (View.TrailingAnchor);
				listView.WidthAnchor.ConstraintEqualToConstant (400);
				listView.HeightAnchor.ConstraintEqualToConstant (500);
			}
		}
	}
}

#endif
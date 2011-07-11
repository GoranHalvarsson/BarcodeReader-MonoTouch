using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Text;
using System.Drawing;
using com.google.zxing;



namespace BarcodeTesting.Controllers
{
	
	
	public class BarcodeViewController : DialogViewController
	{
		
		
		
		private static CameraViewController CameraPicker;
		private DateTime _lastPicked = DateTime.Now;
		private string _currentBarCode = null;
		private string _currentBarCodeType = null;
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabbarController">
		/// A <see cref="UITabBarController"/>
		/// </param>
		public BarcodeViewController (bool push):base(null,push)
		{
			
		}
		
		
		public override void DidReceiveMemoryWarning ()
		{
			System.GC.Collect();	
			
		}
		
		private void GetRootData()
		{
			//Just some image
			var image = UIImage.FromFile("Images/zxing-icon-128.png");
			var imageView = new UIImageView (new RectangleF(0,0,300, 250));
			imageView.Image = image;
			imageView.Layer.BorderWidth = 1;
			imageView.ClipsToBounds = true;
			imageView.Layer.CornerRadius = 30;
			imageView.Image.Dispose();
			
			this.Root = new RootElement ("Barcode scanner") {
				new Section()
				{
					
					new UIViewElement(null, imageView, true){},
					new UIViewElement(null, new UIView(new RectangleF(0,0,0,20)), true),
					new UIViewElement(null, CreateGlassButtonView(), true){}
					
				}
				
			};
			
			
			
		}
		
		private UIView CreateGlassButtonView()
		{
			var view = new UIView();
			view.BackgroundColor = UIColor.White;
			
			var button = new MonoTouch.Dialog.GlassButton(new RectangleF(0,0, 300, 60))
			{
				NormalColor = UIColor.FromRGB(216, 59, 84),
          		HighlightedColor = UIColor.Blue,
				DisabledColor = UIColor.White,
				Font = UIFont.BoldSystemFontOfSize(19)
				
			};
			
			
			button.SetTitle("Scan barcode",UIControlState.Normal);
			button.TouchUpInside += delegate(object sender, EventArgs e) {
				ScanClicked(sender, e);
			};
			
			return button;
		}
			
		
		public override void ViewDidLoad ()
		{
			GetRootData();
			
			base.ViewDidLoad ();
		}

	
	
		
		private void ScanClicked (object sender, EventArgs e)
		{
			
			try {
				
				if(CameraPicker==null)
				{
					CameraPicker = new CameraViewController();
				}
					
				CameraPicker.BarCodeEvent += Handle_pickerBarCodeEvent; 
				
				// start up
				CameraPicker.Initialize();
				this.PresentModalViewController(CameraPicker, true);
				
			} catch (Exception ex) {
				Console.WriteLine("Error in ScanClicked: {0}",ex.Message);
				
				AppDelegate.MainAppDelegate.ShowPopUp("Error", ex.Message.ToString(),10, false, AppDelegate.MainAppDelegate.MainView); 
			}
		}

		void Handle_pickerBarCodeEvent (BarCodeEventArgs evs)
		{
			CameraPicker.DismissViewController();
					
			//Need this, just in case if it's called more than once...
			TimeSpan span = DateTime.Now.Subtract ( _lastPicked );
			if(span.TotalSeconds > 2)
			{
				
				_currentBarCode = evs.BarcodeResult.Text;
				_currentBarCodeType = evs.BarcodeResult.BarcodeFormat.Name;

				//Show the result
				AppDelegate.MainAppDelegate.ShowPopUp("Barcode scanned", String.Format("Barcode type:{0}   Barcode:{1}",_currentBarCodeType, _currentBarCode), 10, false, AppDelegate.MainAppDelegate.MainView); 
				_lastPicked = DateTime.Now;
				
			} else {
				return;
			}
			
			if(CameraPicker.OverlayView == null)
			{
				return;
			}
			
			if (CameraPicker.OverlayView.WorkerTimer != null)
            {
                CameraPicker.OverlayView.WorkerTimer.Invalidate();
                CameraPicker.OverlayView.WorkerTimer.Dispose();
                CameraPicker.OverlayView.WorkerTimer = null;
            }
			
			CameraPicker.OverlayView.Dispose();
			CameraPicker.OverlayView = null;
			CameraPicker.BarCodeEvent -= Handle_pickerBarCodeEvent;
		}
		
		
		
		
	}
}




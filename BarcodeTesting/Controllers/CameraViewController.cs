using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using com.google.zxing;

namespace BarcodeTesting.Controllers
{
    
	public class BarCodeEventArgs : EventArgs
    {
		
		public BarCodeEventArgs(Result result)
		{
		   	BarcodeResult = result;
		}
		
		public Result BarcodeResult  {
			get;
			set;
		}
		
	
		
    }

	// Delegate declaration.
    public delegate void BarCodeEventHandler(BarCodeEventArgs e);
	
	
	public class CameraViewController : UIImagePickerController
    {
        public CameraOverLayView OverlayView;
       	public event BarCodeEventHandler BarCodeEvent;

        public CameraViewController()
            : base()
        {
            Initialize();
        }
       	
		#region Public methods
		public void Initialize()
        {
            
		    if (IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
                SourceType = UIImagePickerControllerSourceType.Camera;
            else
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			

			
            ShowsCameraControls = false;
            AllowsEditing = true;
			
			
			OverlayView = new CameraOverLayView(this);
			CameraOverlayView = OverlayView;
			
        }
       	
		
        public void BarCodeScanned(Result result)
        {
          	if (OverlayView != null)
                OverlayView.StopWorker();
			
			if(_imageView!=null)
			{
				_imageView.RemoveFromSuperview();
			}
			
			if(result!=null)
			{
					BarCodeEventArgs eventArgs = new BarCodeEventArgs(result);
					BarCodeEventHandler handler  = BarCodeEvent;
					if (handler != null)
					{
						// Invokes the delegates.
						handler(eventArgs);
						BarCodeEvent = null;
					}
				
		
			}
			
		}
		 #endregion
		
        public virtual void DismissViewController()
        {
          	if (OverlayView != null)
                OverlayView.StopWorker();
		
			if(_imageView!=null)
			{
				_imageView.RemoveFromSuperview();
			}
			
			DismissModalViewControllerAnimated(true);
			
			
	    }
       
		
		
		
		#region Override methods
        public override void LoadView()
        {
            base.LoadView();

			InvokeOnMainThread(delegate {
	            // Setting tool bar
	            var toolBar = new UIToolbar(new RectangleF(0, 480 - 54, 320, 54));
	            toolBar.Items = new UIBarButtonItem[] {
					new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Done, 
					                    delegate {
											
											DismissViewController();
									
										}),
	                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
	            };
	            Add(toolBar);
			});	
			
		}
		
		private UIImageView _imageView;
		
		public override void ViewWillAppear (bool animated)
		{
			
			base.ViewWillAppear (animated);
			
				_imageView = new UIImageView(new RectangleF(0, 0, 320, 480 - 54));
				_imageView.BackgroundColor = UIColor.Gray;
				_imageView.Image = UIImage.FromBundle("Images/BarCodeOverlay.png");
				Add(_imageView);
				_imageView.Image.Dispose();
				
				//The iris - we don't want that one
				NSNotificationCenter.DefaultCenter.AddObserver(new NSString("PLCameraViewIrisAnimationDidEndNotification"), (notification) => {   
			        
					if(this.View != null)
					{
						_imageView.RemoveFromSuperview();
						
						this.OverlayView.StartWorker();
					}
				});  
			
			
		}
		

		
			
	
		
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
			
			if (OverlayView != null)
                OverlayView.StopWorker();
			
        }
        #endregion

       
		
			
    }
}


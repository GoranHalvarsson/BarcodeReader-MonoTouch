using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using com.google.zxing;
using com.google.zxing.common;
using System.Collections;
using MonoTouch.AudioToolbox;
using BarcodeTesting.Utilities;

namespace BarcodeTesting.Controllers
{
    
	public class ResultCallBack : ResultPointCallback
	{
		CameraOverLayView _view;
		public ResultCallBack(CameraOverLayView view)
		{
			_view = view;
		}
		
		
		#region ResultPointCallback implementation
		public void foundPossibleResultPoint (ResultPoint point)
		{
			if(point!=null)
			{
				_view.SetArrows(true, true);
			} else {
				_view.SetArrows(false, true);
			}
		}
		#endregion	
		
	}
	
	public class CameraOverLayView : UIView
    {
        #region Variables
        public NSTimer WorkerTimer;
		
     	private CameraViewController _parentViewController;
		
		private UIImageView _mainView;
		private UIImageView _greenTopArrow;
		private UIImageView _greenBottomArrow;
		private UIImageView _whiteTopArrow;
		private UIImageView _whiteBottomArrow;
		private UILabel _textCue;
		private UIImageView _otherView1, _otherView2, _focusView1, _focusView2;
		private bool _isGreen;
		private Hashtable hints;
		
		private static com.google.zxing.oned.MultiFormatOneDReader _multiFormatOneDReader = null;
        private static RectangleF picFrame = RectangleF.Empty;
		private static UIImage _theScreenImage = null;
	  #endregion

		
        public CameraOverLayView(CameraViewController parentController) : base()
        {
            Initialize();
			_parentViewController = parentController;
        }
		
		#region Private methods
		private void Initialize()
        {            
           	
				Frame = new RectangleF(0, 0, 320, (480) - (54));
			    Opaque = false;
	            BackgroundColor = UIColor.Clear;
	
				_mainView = new UIImageView(Frame);  
				_mainView.Image = UIImage.FromBundle("Images/BarCodeOverlay.png");
				Add(_mainView);
				
				
				_textCue = new UILabel()
				{
					Frame = new RectangleF(0,102,320,21),
					Text = "Adjust barcode according to arrows",
					Font = UIFont.SystemFontOfSize(13),
					TextAlignment = UITextAlignment.Center,
					TextColor = UIColor.White,
					Opaque = false,
					BackgroundColor = UIColor.FromRGBA(0,0,0,0),
				};
				
	            _greenBottomArrow = new UIImageView(UIImage.FromBundle("Images/green_down_arrow.png"));
				_greenBottomArrow.Frame = new RectangleF(0,129,320,19);
				_greenTopArrow = new UIImageView(UIImage.FromBundle("Images/green_up_arrow.png"));
				_greenTopArrow.Frame = new RectangleF(0,302,320,19);
				
				_whiteBottomArrow = new UIImageView(UIImage.FromBundle("Images/white_down_arrow.png"));
				_whiteBottomArrow.Frame = new RectangleF(0,129,320,19);
				_whiteTopArrow = new UIImageView(UIImage.FromBundle("Images/white_up_arrow.png"));
				_whiteTopArrow.Frame = new RectangleF(0,302,320,19);
				
				
				AddSubview(_textCue);
				AddSubview(_greenBottomArrow);
				AddSubview(_greenTopArrow);
				AddSubview(_whiteBottomArrow);
				AddSubview(_whiteTopArrow);
				
				_greenBottomArrow.Image.Dispose();
				_greenTopArrow.Image.Dispose();
				_textCue.Dispose();
				_whiteBottomArrow.Image.Dispose();
				_whiteTopArrow.Image.Dispose();
				_mainView.Image.Dispose();
				
				
				// initalise flags
				_isGreen = false;
				SetArrows(false, false);
			

        
		}

		private void Worker()
        {
                float fY1;
                float fY2;

                float scale;
                
                // calculate picFrame just once
                if (picFrame == RectangleF.Empty)
                {

                    // check if device has retina display, if so scale factor by 2
                    if (UIScreen.MainScreen.RespondsToSelector(new MonoTouch.ObjCRuntime.Selector(@"scale")) &&
                        UIScreen.MainScreen.Scale == 2)
                        scale = 2f;
                    else
                        scale = 1f;

                    // check if the device is an ipad or an iphone
                    if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                    {
                        fY1 = 146f / UIScreen.MainScreen.Bounds.Height * scale;     
                        fY2 = 157f / UIScreen.MainScreen.Bounds.Height * scale;
                    }
                    else
                    {
                        // ipad - constants probably need to be modified if running at native screen res

                        fY1 = 146f / UIScreen.MainScreen.Bounds.Height * scale;     
                        fY2 = 157f / UIScreen.MainScreen.Bounds.Height * scale;
                    }

                    picFrame = new RectangleF(0, UIScreen.MainScreen.Bounds.Height * fY1, UIScreen.MainScreen.Bounds.Width * scale, UIScreen.MainScreen.Bounds.Height * fY2);
                }
				
				if(hints==null)
				{
					var list = new ArrayList();
					list.Add(com.google.zxing.BarcodeFormat.EAN_8);
					list.Add(com.google.zxing.BarcodeFormat.EAN_13);
					
					hints = new Hashtable();
					hints.Add(com.google.zxing.DecodeHintType.POSSIBLE_FORMATS, list);
					hints.Add(com.google.zxing.DecodeHintType.NEED_RESULT_POINT_CALLBACK, new ResultCallBack(this));
				}
				
				if(_multiFormatOneDReader == null)
				{
					_multiFormatOneDReader = new com.google.zxing.oned.MultiFormatOneDReader(hints);
				}
				
				
				// Capturing screen image            
	            using (var screenImage = CGImage.ScreenImage.WithImageInRect(picFrame))
	            {
					_theScreenImage = UIImage.FromImage(screenImage);
					Bitmap srcbitmap = new System.Drawing.Bitmap(_theScreenImage);
					LuminanceSource source = null;
					BinaryBitmap bitmap = null;
					try {
						source = new RGBLuminanceSource(srcbitmap, screenImage.Width, screenImage.Height);
		              	bitmap = new BinaryBitmap(new HybridBinarizer(source));
						
						com.google.zxing.common.BitArray row = new com.google.zxing.common.BitArray(screenImage.Width);
						int middle = screenImage.Height >> 1;
						int rowStep = System.Math.Max(1, screenImage.Height >> (4));
						
						for (int x = 0; x < 9; x++)
						{
							
							// Scanning from the middle out. Determine which row we're looking at next:
							int rowStepsAboveOrBelow = (x + 1) >> 1;
							bool isAbove = (x & 0x01) == 0; // i.e. is x even?
							int rowNumber = middle + rowStep * (isAbove?rowStepsAboveOrBelow:- rowStepsAboveOrBelow);
							if (rowNumber < 0 || rowNumber >= screenImage.Height)
							{
								// Oops, if we run off the top or bottom, stop
								break;
							}
							
							// Estimate black point for this row and load it:
							try
							{
								row = bitmap.getBlackRow(rowNumber, row);
								
								var resultb = _multiFormatOneDReader.decodeRow(rowNumber, row, hints);
								if(resultb.Text!=null)
								{
									BeepOrVibrate();
									_parentViewController.BarCodeScanned(resultb);
										
								
									break;
								}
								else {
									continue;
								}
								
							}
							catch (ReaderException re)
							{
								continue;
							}
					
						}
						
						
						
	//					var result = _barcodeReader.decodeWithState(bitmap);
	//					
	//					if(result.Text!=null)
	//					{
	//						_multiFormatOneDReader = null;
	//						BeepOrVibrate();
	//						_parentViewController.BarCodeScanned(result);
	//					}
						
					} catch (Exception ex) {
						Console.WriteLine(ex.Message);
					}
					finally {
						if(bitmap!=null)
							bitmap = null;
					
						 if(source!=null)
							source = null;
						
		                if(srcbitmap!=null)
							srcbitmap = null;
					
					}	
					
	      
	            }
			
        }
		
		private void BeepOrVibrate()
		{
			SystemSound.FromFile("Sounds/beep.wav").PlayAlertSound();
		}
		#endregion

        #region Public methods
		public void SetArrows(bool inRange, bool animated)
		{
			
			
			// already showing green bars
			if (inRange && _isGreen) return;
			
			// update flag
			_isGreen = inRange;
			
			if (_isGreen)
			{
				_textCue.Text =  "Hold still for scanning";
				_otherView1 = _whiteTopArrow; 
				_otherView2 = _whiteBottomArrow;
				_focusView1 = _greenTopArrow; 
				_focusView2 = _greenBottomArrow;
			}
			else
			{
				_textCue.Text =  "Adjust barcode according to arrows";
				_focusView1 = _whiteTopArrow; 
				_focusView2 = _whiteBottomArrow;
				_otherView1 = _greenTopArrow; 
				_otherView2 = _greenBottomArrow;
			}
			
			if (animated)
			{
				UIView.BeginAnimations("");
				UIView.SetAnimationDuration(0.15f);
				UIView.SetAnimationBeginsFromCurrentState(true);
			}
			
			_focusView1.Alpha = 1;
			_focusView2.Alpha = 1;
			
			_otherView1.Alpha = 0;
			_otherView2.Alpha = 0;
			
			if (animated) UIView.CommitAnimations();
		}
		
		public void StartWorker()
        {
           
			if(WorkerTimer!=null)
			{
				return;
			}
			
			
			 WorkerTimer = NSTimer.CreateRepeatingTimer(new TimeSpan(1L), delegate { 
				Worker(); 
			});
			NSRunLoop.Current.AddTimer(WorkerTimer, NSRunLoopMode.Default);
			
			
            
        }
        
		public void StopWorker()
        {
            // starting timer
            if (WorkerTimer != null)
            {
                WorkerTimer.Invalidate();
                WorkerTimer.Dispose();
                WorkerTimer = null;
				NSRunLoop.Current.Dispose();
            }
			
			//Just in case
			_multiFormatOneDReader = null;
			hints = null;
        }
        #endregion
		
		
        #region Override methods       
        protected override void Dispose(bool disposing)
        {
            StopWorker();
			_greenBottomArrow.Dispose();
			_greenTopArrow.Dispose();
			_textCue.Dispose();
			_whiteBottomArrow.Dispose();
			_whiteTopArrow.Dispose();
			_mainView.Dispose();
			 _otherView1.Dispose();
			_otherView2.Dispose();
			_focusView1.Dispose();
			_focusView2.Dispose();
			
			
            base.Dispose(disposing);
        }
        #endregion
		
    }
}

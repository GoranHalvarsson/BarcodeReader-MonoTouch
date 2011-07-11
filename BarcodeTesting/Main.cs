
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BarcodeTesting.Controllers;
using BarcodeTesting.Utilities;


namespace BarcodeTesting
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		
		private TabBarController TabBar; 
		public UIView MainView;
		public LoadingHUDView LoadingView;
		public static AppDelegate MainAppDelegate;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			window.MakeKeyAndVisible ();
			MainAppDelegate = this;
			CreatePhoneGui();
			
			return true;
			           
		}

		private void CreatePhoneGui()
		{
			
			// Create the tabs
			TabBar = new TabBarController();
			MainView = TabBar.View;
			// Add the navigation controller as a subview
            window.AddSubview(MainView);
		
		}
		
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
		
		public override void ReceiveMemoryWarning(UIApplication application)
        {
            System.GC.Collect();
        }
		
		
		public void ShowPopUp(string header, string detail, int showInSeconds, bool loading, UIView currentView)
		{
			LoadingView = new LoadingHUDView (header, detail);
			
			currentView.AddSubview(AppDelegate.MainAppDelegate.LoadingView);
			
			
			if(loading)
			{
				LoadingView.StartAnimating();
			} else {
				LoadingView.Show ();
			}
			
			var timerCancel = NSTimer.CreateScheduledTimer(new TimeSpan(0, 0, showInSeconds),delegate
			{
						
				if(AppDelegate.MainAppDelegate.LoadingView == null)
					return;
							
				AppDelegate.MainAppDelegate.LoadingView.Close();
				AppDelegate.MainAppDelegate.LoadingView = null;
						
			});
			
			LoadingView.CancelEvent += delegate {
				//Hide the view
				AppDelegate.MainAppDelegate.LoadingView.Close();
				AppDelegate.MainAppDelegate.LoadingView = null;
				
				if(timerCancel != null)
				{
					timerCancel.Invalidate();
					timerCancel = null;
				}
				
			};
		}
		
		
	}
	
	
	
		
	

}


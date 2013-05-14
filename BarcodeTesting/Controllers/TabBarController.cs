using System;
using MonoTouch.UIKit;

namespace BarcodeTesting.Controllers
{
	public class TabBarController : UITabBarController
	{
		public MonoTouch.UIKit.UINavigationController 
		NavStartController;
		
		public TabBarController()
		{
			CreateTabs();
			this.SelectedViewController = NavStartController;
		}
		
		private void CreateTabs()
		{
			
			var svc = new BarcodeViewController(false);
			NavStartController = new MonoTouch.UIKit.UINavigationController();
			NavStartController.PushViewController(svc, false);
			NavStartController.NavigationBar.TintColor = UIColor.Black;
			NavStartController.TabBarItem = new UITabBarItem("Barcode", UIImage.FromBundle("Images/icon_barcode.png"), 0);
		
			this.ViewControllers = new UIViewController[]{NavStartController};
			
			MoreNavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			
		}
		
	}
}


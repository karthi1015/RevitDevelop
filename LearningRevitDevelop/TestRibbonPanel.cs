using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Media.Imaging;

namespace LearningRevitDevelop
{
    public class TestRibbonPanel : IExternalApplication
    {
        static string addinPath = typeof(TestRibbonPanel).Assembly.Location;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("HelloFunny");
            RibbonPanel panelOne = application.CreateRibbonPanel("HelloFuuny", "HelloRevit");
            PushButtonData pushButtonData = new PushButtonData("Funny", "HelloFunny", addinPath, "LearningRevitDevelop.HelloRevit");

            Uri uriImage = new Uri(@"E:\HelloFunny\image\Heart.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            panelOne.AddItem(pushButtonData);
            return Result.Succeeded;
        }
    }
}

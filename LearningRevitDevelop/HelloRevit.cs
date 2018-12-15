using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class HelloRevit : IExternalCommand
    {   /// <summary>
        /// 
        /// </summary>
        /// <param name="commandData">传递给外部应用程序的对象，其中包含与命令相关的数据，如应用程序对象和活动视图。</param>
        /// <param name="message">可以由外部应用程序设置的消息，如果外部命令返回失败或取消，将显示该消息。</param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application app = commandData.Application.Application;
            Document activeDoc = commandData.Application.ActiveUIDocument.Document;

            TaskDialog mainDialog = new TaskDialog("Hello Reivt");
            mainDialog.MainInstruction = "Hello Revit";
            mainDialog.MainContent = "这个例子可以演示一个外部命令是怎样加载到Revit用户界面的"
                + "它使用Revit任务对话框与用户交流信息\n"
                + "点击下面链接获取更多帮助";
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "关于Revit版本信息");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "关于此文档的信息");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "关于开发此Command的作者");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "关于软件日志");

            //设置常见按钮和默认按钮，如果不需要设置按钮，显示框不会展现按钮
            mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
            mainDialog.DefaultButton = TaskDialogResult.Close;


            mainDialog.FooterText = "<a href=\"http://www.google.com \">"
                + "Click here for the Revit API Developer Center</a>";

            TaskDialogResult tResult = mainDialog.Show();

            if (TaskDialogResult.CommandLink1 == tResult)
            {
                TaskDialog dialog_CommandLink1 = new TaskDialog("Revit Build Information");
                dialog_CommandLink1.MainInstruction = "Revit Version Name is：" + "\t" + app.VersionName + "\n"
                    + "Revit Version Number is:" + "\t" + app.VersionNumber + "\n"
                    + "Revit Version Build is:" + "\t" + app.VersionBuild
                    + "Revit Username is" + "\t" + app.Username;
                dialog_CommandLink1.Show();

            }
            else if (TaskDialogResult.CommandLink2 == tResult)
            {
                TaskDialog.Show("Active Document Information", "Active document:" + activeDoc.Title + "\n"
                    + "Active view name:" + activeDoc.ActiveView.Name);
            }
            else if (TaskDialogResult.CommandLink3 == tResult)
            {
                TaskDialog.Show("Hello Baby", "This is just a test command XDDD");
            }
            else
            {
                TaskDialog.Show("日志", app.RecordingJournalFilename);
            }
            return Result.Succeeded;
        }

    }

}

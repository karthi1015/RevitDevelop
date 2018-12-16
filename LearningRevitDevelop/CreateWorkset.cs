using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HelloFunnyTools
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CreateWorket : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<string> newWorkList = new List<string> { "1", "2", "3", "4", "5" };
            if (!doc.IsWorkshared)
            {
                message = "该项目不是中心文件。";
                return Result.Failed;
            }

            foreach (string newworksetName in newWorkList)
            {
                bool result = (WorksetTable.IsWorksetNameUnique(doc, newworksetName));
                if (result == false)
                {
                    message = "有重复的工作集名称。";
                    return Result.Failed;
                }
                if (result == true)
                {
                    CreateWorksets(doc, newworksetName);
                }
            }
            TaskDialog.Show("Revit", "工作集创建成功");
            return Result.Succeeded;
        }

        /// <summary>
        /// 创建工作集
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="worksetName">输入工作集名称</param>
        /// <returns></returns>
        public void CreateWorksets(Document doc, string worksetName)
        {
            using (Transaction trans = new Transaction(doc, "创建工作集"))
            {
                trans.Start();
                Workset.Create(doc, worksetName);
                trans.Commit();
            }
        }
    }
}

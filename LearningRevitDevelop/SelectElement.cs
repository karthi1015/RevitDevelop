using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Collections;

namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SelectElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;  //获取当前文档
                Selection selection = uiDoc.Selection; //获取当前文档的选择集
                ICollection<ElementId> selectedIds = uiDoc.Selection.GetElementIds();

                if (selectedIds.Count == 0)
                {
                    TaskDialog.Show("抱歉", "你还没有选择任何图元");
                }
                else
                {
                    string info = "选中的图元ID如下";
                    foreach (ElementId id in selectedIds)
                    {
                        info += "\n\t" + id.IntegerValue;
                    }
                    TaskDialog.Show("revit", info);
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;

        }
    }
}

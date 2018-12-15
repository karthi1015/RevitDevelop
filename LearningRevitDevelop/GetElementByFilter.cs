using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections;
using System.IO;

namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class GetElementByFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                FilteredElementCollector filteredElementCollecter = new FilteredElementCollector(doc);
                ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
                ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(FamilyInstance));
                LogicalAndFilter logicAndFilter = new LogicalAndFilter(elementCategoryFilter, elementClassFilter);
                filteredElementCollecter.WherePasses(logicAndFilter);
                IList<Element> elementS = filteredElementCollecter.ToElements();

                if (elementS.Count == 0)
                {
                    TaskDialog.Show("Revit", "抱歉，没有符合要求的图元");
                }
                else
                {
                    string info = "符合条件的图元如下";

                    foreach (Element elem in elementS)
                    {
                        info += "\n\t" + elem.Name;

                    }
                    File.WriteAllText(@"D:\doors.txt", info + @"\n共计" + elementS.Count + "扇门。");
                    TaskDialog.Show("Revit", "写入成功");
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Revit", e.Message);
            }
            return Result.Succeeded;
        }
    }
}

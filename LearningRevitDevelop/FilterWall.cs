using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections;

namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FilterWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector filteredElements = new FilteredElementCollector(doc); //创建一个收集器
            ElementFilter wallCategory = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            ElementFilter wallClass = new ElementClassFilter(typeof(Wall));
            LogicalAndFilter wallFitered = new LogicalAndFilter(wallCategory, wallClass);
            filteredElements.WherePasses(wallFitered);

            IList<Wall> walls = new List<Wall>();
            foreach (Wall wal in filteredElements)
            {
                var functionParameter = wal.WallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM);
                if ((functionParameter != null) && (functionParameter.StorageType == StorageType.Integer))
                {
                    IList<ElementId> walls_ids = new List<ElementId>();
                    if (functionParameter.AsInteger() == (int)WallFunction.Exterior)
                    {
                        walls.Add(wal);
                        foreach (Wall wa in walls)
                        {
                            walls_ids.Add(wa.Id);
                        }

                    }
                    uidoc.Selection.SetElementIds(walls_ids);  //将图元设置为选中状态
                }
            }
            return Result.Succeeded;
        }
    }
}

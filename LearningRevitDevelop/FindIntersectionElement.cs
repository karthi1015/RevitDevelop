using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FindIntersectionElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            // 找到与选择图元相交的solid
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "选择要检查与其他图元相交的图元");
            Element element = doc.GetElement(reference);
            GeometryElement geomElement = element.get_Geometry(new Options());
            Solid solid = null;
            foreach (GeometryObject geomObj in geomElement)
            {
                solid = geomObj as Solid;
                if (solid != null) break;
            }

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilyInstance));
            collector.WherePasses(new ElementIntersectsSolidFilter(solid)); // 应用相交过滤器

            TaskDialog.Show("Revit", collector.Count() + " 个图元与相中的图元相交(" + element.Category.Name + " id:" + element.Id.ToString() + ")");

            return Result.Succeeded;
        }
    }
}

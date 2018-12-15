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
    class SelectionFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ISelectionFilter wallFilter = new WallSelecitonFilter(doc);
            IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, wallFilter, "请选择墙");
            ICollection<ElementId> wallIds = new List<ElementId>();
            foreach (Reference re in references)
            {
                wallIds.Add(re.ElementId);
            }
            uidoc.Selection.SetElementIds(wallIds);
            return Result.Succeeded;
        }

        public class WallSelecitonFilter : ISelectionFilter
        {
            Document document = null;
            public WallSelecitonFilter(Document doc)
            {
                doc = document;
            }
            public bool AllowElement(Element elem)
            {
                if (elem is Wall && elem.Name == "常规 - 200mm")
                {
                    return true;
                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {

                return false;
            }
        }
    }
}

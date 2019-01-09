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
    class SayGoodBye : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector sf = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming)
                .OfClass(typeof(FamilyInstance));
            FilteredElementCollector cl = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns)
                .OfClass(typeof(FamilyInstance));
            TaskDialog.Show("亲爱的", "到目前为止，今天你已经绘制了" + sf.Count().ToString() + "道梁" + "\n\t" + cl.Count().ToString() +
  "根柱子" + "\n\t" + "早点下班回家休息吧~么么哒" + "\n\t别猝死在办公室噢~QAQ");
            return Result.Succeeded;
        }
    }
}

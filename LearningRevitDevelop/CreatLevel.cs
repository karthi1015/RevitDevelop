using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace LearningRevitDevelop
{   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CreatLevel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            double elevation = 3;
            Transaction trans = new Transaction(doc);
            trans.Start("creatlevel");
            Level level = Level.Create(doc, elevation);
            trans.Commit();

            var classFilter = new ElementClassFilter(typeof(ViewFamilyType));
            FilteredElementCollector filterElements = new FilteredElementCollector(doc);
            filterElements.WherePasses(classFilter);
            foreach (ViewFamilyType viewFamilyType in filterElements)
            {
                if ((viewFamilyType.ViewFamily == ViewFamily.FloorPlan) || (viewFamilyType.ViewFamily == ViewFamily.CeilingPlan))
                {

                    trans.Start("creat view of type" + viewFamilyType.ViewFamily);
                    ViewPlan view = ViewPlan.Create(doc, viewFamilyType.Id, level.Id);
                    trans.Commit();
                }
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections;
using System.IO;
namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class GetAllElementParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            ElementFilter door_Category = new ElementCategoryFilter(BuiltInCategory.OST_Doors);//获取门类别
            ElementFilter door_FamilyInstance = new ElementClassFilter(typeof(FamilyInstance));//获取族实例
            LogicalAndFilter doorFamilyInstance = new LogicalAndFilter(door_Category, door_FamilyInstance);
            FilteredElementCollector doorFiltered = new FilteredElementCollector(doc).WherePasses(doorFamilyInstance);
            IList<Element> door_elements = doorFiltered.ToElements();
            

            ParameterSet paras = null;

            foreach (Element elems in door_elements)
            {
                paras = elems.Parameters;
            }
            string paraName = "";

            foreach (Parameter para in paras)
            {
                paraName += "\n\t" + para.Definition.Name;
            }
            File.WriteAllText(@"D:\door.txt", paraName);


            return Result.Succeeded;

        }
    }
}

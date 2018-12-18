using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
namespace HelloFunnyTools
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CreateWorket : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Workset workset = null;
            List<Element> elementList = new List<Element>();
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
                    return Result.Succeeded;
                }
                if (result == true)
                {
                    workset = CreateWorksets(doc, newworksetName);
                }
            }
            TaskDialog.Show("Revit", "工作集创建成功");
            elementList = FilterElement(doc);
            AddElementToWorkset(doc, workset, elementList);

            return Result.Succeeded;
        }

        /// <summary>
        /// 创建工作集
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="worksetName">输入工作集名称</param>
        /// <returns></returns>
        public Workset CreateWorksets(Document doc, string worksetName)
        {
            Workset newWorksets = null;
            using (Transaction trans = new Transaction(doc, "创建工作集"))
            {
                trans.Start();
                newWorksets = Workset.Create(doc, worksetName);
                trans.Commit();
            }
            return newWorksets;
        }

        public void AddElementToWorkset(Document doc, Workset workset, List<Element> element)
        {
            if (workset != null)
            {
                int worksetId = workset.Id.IntegerValue;
                using (Transaction trans = new Transaction(doc, "将图元添加到指定工作集"))
                {
                    trans.Start();
                    foreach (Element ele in element)
                    {
                        Parameter wspara = ele.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                        if (wspara != null)
                        {
                            wspara.Set(worksetId);
                        }
                    }
                    trans.Commit();
                }
            }
            if (workset == null)
            {
                TaskDialog.Show("Revit", "工作集为空");
            }
        }

        public List<Element> FilterElement(Document doc)
        {
            FilteredElementCollector doorCollector = new FilteredElementCollector(doc);
            doorCollector.OfCategory(BuiltInCategory.OST_Doors).OfClass(typeof(FamilyInstance));
            List<Element> doorList = new List<Element>();
            foreach (Element door in doorCollector)
            {
                doorList.Add(door);
            }
            return doorList;
        }
    }
}

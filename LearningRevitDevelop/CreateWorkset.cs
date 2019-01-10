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
            // 让用户输入工作集名称 添加至list 批量生成工作集
            List<string> newWorkList = new List<string> { "(01-1F-建筑-墙)", "02-1F-结构-墙", "03-1F-建筑-门", "04-1F-建筑-窗", "05-1F-建筑-柱" };
            if (!doc.IsWorkshared)
            {
                message = "该项目不是中心文件。";
                return Result.Failed;
            }

            foreach (string newworksetName in newWorkList)
            {
                workset = CreateWorksets(doc, newworksetName);
            }

            TaskDialog.Show("Revit", "工作集创建成功");
            //BuiltInCategory builtInCategory = BuiltInCategory.OST_Doors;
            //Type type = typeof(FamilyInstance);
            //string levelName = "Level 1";
            //Level level = GetLevel(doc, levelName);
            //elementList = FilterElement(doc, builtInCategory, type, level);
            //AddElementToWorkset(doc, workset, elementList);


            return Result.Succeeded;
        }

        /// <summary>
        /// 创建工作集
        /// </summary>
        /// <param name="doc">当前文档</param>
        /// <param name="worksetName">输入工作集名称</param>
        /// <returns>新的工作集</returns>
        public Workset CreateWorksets(Document doc, string worksetName)
        {
            Workset newWorksets = null;
            if (WorksetTable.IsWorksetNameUnique(doc, worksetName))
            {
                using (Transaction trans = new Transaction(doc, "创建工作集"))
                {
                    trans.Start();
                    newWorksets = Workset.Create(doc, worksetName);
                    trans.Commit();
                }
            }
            //判定是否有重名的工作集
            else
            {
                FilteredWorksetCollector worksetCollector = new FilteredWorksetCollector(doc);
                IList<Workset> worksetsList = worksetCollector.OfKind(WorksetKind.UserWorkset).ToWorksets();
                foreach (Workset workset in worksetsList)
                {
                    if (workset.Name.Contains(worksetName))
                    {
                        return workset;
                    }
                }
            }
            return newWorksets;
        }
        /// <summary>
        /// 添加图元到指定工作集
        /// </summary>
        /// <param name="doc">当前项目</param>
        /// <param name="workset">要添加图元的工作集</param>
        /// <param name="element">要操作的图元</param>
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


        /// <summary>
        /// 过滤要添加的图元
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>过滤的图元列表</returns>
        public List<Element> FilterElement(Document doc, BuiltInCategory builtInCategory, Type type, Level level)
        {

            FilteredElementCollector elementCollector = new FilteredElementCollector(doc);
            elementCollector.OfCategory(builtInCategory).OfClass(type);
            List<Element> elementList = new List<Element>();
            if (elementCollector.Count() != 0)
            {
                foreach (Element ele in elementCollector)
                {
                    if (ele.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Definition.Name == level.Name)
                    {
                        elementList.Add(ele);
                    }
                }
            }
            else
            {

            }
            return elementList;
        }
    }
}

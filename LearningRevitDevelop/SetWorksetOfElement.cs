using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace LearningRevitDevelop
{
    [Transaction(TransactionMode.Manual)]
    class SetWorksetOfElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //让用户选择要标高，类别
            ElementClassFilter classFilter = new ElementClassFilter(typeof(FamilyInstance));
            ElementClassFilter hostObjectFilter = new ElementClassFilter(typeof(HostObject));
            Categories categories = doc.Settings.Categories;
            FilteredElementCollector allElement = new FilteredElementCollector(doc);
            FilteredElementCollector hostObjetElement = new FilteredElementCollector(doc);
            foreach (Category ca in categories)
            {

                if (ca.CategoryType == CategoryType.Model)
                {
                    ElementCategoryFilter categoryFilter = new ElementCategoryFilter(ca.Id);
                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(categoryFilter, classFilter);
                    LogicalOrFilter host_object = new LogicalOrFilter(hostObjectFilter, categoryFilter);
                    hostObjetElement.WherePasses(hostObjectFilter);
                    allElement.WherePasses(logicalOrFilter).UnionWith(hostObjetElement);
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (Element ele in allElement)
            {
                sb.Append(ele.GetType().Name + "\n\t");
            }
            TaskDialog.Show("Revit", sb.ToString());

            // TaskDialog.Show("Revit", allElement.Count().ToString());

            //List<Element> levelsList = GetAllLevels(doc);

            return Result.Succeeded;
        }
        /// <summary>
        /// 将图元添加进工作集
        /// </summary>
        /// <param name="doc">当前文档</param>
        /// <param name="workset">要添加的工作集</param>
        /// <param name="element">要添加的图元</param>
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
        /// 获取当前文档的所有标高
        /// </summary>
        /// <param name="doc">当前文档</param>
        /// <returns>标高列表</returns>
        public List<Element> GetAllLevels(Document doc)
        {      //用父类的返回值
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc);
            ICollection<Element> levelList = levelCollector.OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).ToElements();
            List<Element> levelsList = levelList.ToList();
            return levelsList;
        }

        // public void GetElementOfLevel(Document doc,List)
    }
}
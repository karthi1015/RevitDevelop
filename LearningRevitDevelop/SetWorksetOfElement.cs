using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
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

            List<Element> levelsList = GetAllLevels(doc);
            List<Level> lelList = levelsList.ConvertAll(x => x as Level);



            // TaskDialog.Show("Revit", elsList.Count().ToString());
            List<Element> elsList = GetAllElements(doc);


            StringBuilder sb = new StringBuilder();
            Categories categories = doc.Settings.Categories;
            List<FamilySymbol> famsysList = new List<FamilySymbol>();
            foreach (Element el in elsList)
            {
                famsysList.Add(el as FamilySymbol);
            }

            TaskDialog.Show("Revit", sb.ToString());





            return Result.Succeeded;
        }
        /// <summary>
        /// 获取当前项目中的所有图元
        /// </summary>
        /// <param name="doc">当前项目</param>
        /// <returns>图元列表</returns>
        public List<Element> GetAllElements(Document doc)
        {
            List<Element> elementsList = new List<Element>();
            ElementClassFilter fmistFilter = new ElementClassFilter(typeof(FamilyInstance));//获取族实例类型的图元
            ElementClassFilter hostObjectFilter = new ElementClassFilter(typeof(HostObject));//获取宿主类型的图元
            Categories categories = doc.Settings.Categories;
            FilteredElementCollector allElement = new FilteredElementCollector(doc);
            FilteredElementCollector hostObjetElement = new FilteredElementCollector(doc);
            foreach (Category ca in categories)
            {

                if (ca.CategoryType == CategoryType.Model)
                {
                    ElementCategoryFilter categoryFilter = new ElementCategoryFilter(ca.Id);
                    LogicalOrFilter logicalOrFilter = new LogicalOrFilter(categoryFilter, fmistFilter);
                    LogicalOrFilter host_object = new LogicalOrFilter(hostObjectFilter, categoryFilter);
                    hostObjetElement.WherePasses(hostObjectFilter);
                    allElement.WherePasses(logicalOrFilter).UnionWith(hostObjetElement);
                }
            }
            elementsList = allElement.ToList();
            return elementsList;
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
        /// <summary>
        /// 获取某标高上的模型图元
        /// </summary>
        /// <param name="doc">当前文档</param>
        /// <param name="level">该标高</param>
        /// <returns></returns>
        public List<Element> GetElementOfLevel(Document doc, Level level)
        {
            ElementLevelFilter levelFilter = new ElementLevelFilter(level.Id, false);
            FilteredElementCollector elements = new FilteredElementCollector(doc).WherePasses(levelFilter);
            List<Element> elsList = new List<Element>();
            if (elements.ToList().Count != 0)
            {
                foreach (Element el in elements)
                {
                    if (el.Category.CategoryType == CategoryType.Model)
                    {
                        elsList.Add(el);
                    }

                }
            }
            return elsList;
        }
    }


}
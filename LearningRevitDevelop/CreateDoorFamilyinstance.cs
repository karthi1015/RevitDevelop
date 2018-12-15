using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

namespace LearningRevitDevelop
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CreateDoorFamilyinstance : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            ElementFilter elementCatgeoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);//过滤内置门类别
            ElementFilter elementFamilyinstanceFilter = new ElementClassFilter(typeof(FamilySymbol));//过滤族类型
            LogicalAndFilter doorsFamilyinstances = new LogicalAndFilter(elementCatgeoryFilter, elementFamilyinstanceFilter);
            FilteredElementCollector doors = new FilteredElementCollector(doc).WherePasses(doorsFamilyinstances);
            
            string doorTypeName = "750 x 2000mm";
            FamilySymbol doorType = null;
            bool familyFound = false;

            foreach (FamilySymbol door in doors)
            {
                if (door.Name == doorTypeName)
                {
                    doorType = door;
                    familyFound = true;
                    break;
                }
            }

            if (!familyFound) //如果没有找到族文件 加载一个
            {
                string familyPath = "";
                Family family;
                bool loadSucceed = doc.LoadFamily(familyPath, out family); //是否记载成功，成功后传出族
                if (loadSucceed)
                {
                    foreach (ElementId doorTypeID in family.GetValidTypes())
                    {
                        doorType = doc.GetElement(doorTypeID) as FamilySymbol;
                        if (doorType != null)
                        {
                            if (doorType.Name == doorTypeName)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    TaskDialog.Show("族加载失败", "不能找到族文件");
                }
            }


            if (doorType != null)
            {
                ElementFilter wallFilter = new ElementClassFilter(typeof(Wall));
                FilteredElementCollector filteredElementCollecter = new FilteredElementCollector(doc);
                filteredElementCollecter.WherePasses(wallFilter);
                Wall wall = null;
                Line line = null;

                foreach (Wall element in filteredElementCollecter)
                {
                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (locationCurve != null)
                    {
                        line = locationCurve.Curve as Line;
                        if (line != null)
                        {
                            wall = element;
                        }
                    }
                }

                if (wall != null)
                {
                    XYZ midPoint = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;
                    Level wallLevel = doc.GetElement(wall.LevelId) as Level;
                    using (Transaction trans = new Transaction(doc))
                    {
                        trans.Start("CreateDoor");
                        FamilyInstance door = doc.Create.NewFamilyInstance(midPoint, doorType, wall, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        TaskDialog.Show("创建成功", "图元的ID为" + door.Id);
                        trans.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("图元不存在", "没有找到符合条件的墙");
                }
            }
            else
            {
                TaskDialog.Show("族类型不存在", "没有找到族类型" + doorTypeName + "");
            }

            return Result.Succeeded;
        }
    }
}
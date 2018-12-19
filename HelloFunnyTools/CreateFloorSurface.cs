using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using CreateFloor;

namespace HelloFunnyTools
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CreateFloorSurface : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //过滤房间
            List<Element> roomList = RoomList(doc);
            if (!(roomList.Count > 0))
            {
                message = "项目中没有房间类型";
                return Result.Failed;
            }
            //过滤楼板
            List<Element> floorList = FloorList(doc);
            if (!(floorList.Count > 0))
            {
                message = "项目中没有楼板类型";
                return Result.Failed;
            }
            //弹出对话框，让用户选择添加楼板面层的房间和楼板面层的类型
            List<string> createSetting = ShowDialog(roomList, floorList);
            if (!(createSetting.Count > 0))
            {
                return Result.Cancelled;
            }
            //获取房间轮廓
            List<Element> roomsList = new List<Element>();
            List<CurveArray> curveArrayList = RoomBoundaryList(roomList, createSetting, out roomsList);
            if (!(curveArrayList.Count > 0))
            {
                message = "项目中没有有效的房间轮廓";
                return Result.Failed;
            }


            //创建楼板面层
            FloorType ft = floorList.Where(x => x.Name == createSetting[2]).First() as FloorType;
            bool result = CreateSurface(doc, ft, curveArrayList, roomsList);
            if (result == false)
            {
                message = "创建楼板失败";
                return Result.Failed;
            }
            else
            {
                TaskDialog.Show("Revit", "楼板创建成功");
            }
            return Result.Succeeded;
        }
        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public List<Element> RoomList(Document doc)
        {
            FilteredElementCollector roomCollcter = new FilteredElementCollector(doc);
            roomCollcter.OfCategory(BuiltInCategory.OST_Rooms).OfClass(typeof(SpatialElement));
            List<Element> roomList = roomCollcter.ToList();
            return roomList;
        }
        /// <summary>
        /// 获取楼板
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private List<Element> FloorList(Document doc)
        {
            FilteredElementCollector floorCollcter = new FilteredElementCollector(doc);
            floorCollcter.OfCategory(BuiltInCategory.OST_Floors).OfClass(typeof(FloorType));
            List<Element> floorList = floorCollcter.ToList();
            return floorList;
        }
        /// <summary>
        /// 获取房间边界线
        /// </summary>
        /// <param name="roomList"></param>
        /// <returns></returns>
        private List<CurveArray> RoomBoundaryList(List<Element> roomList, List<string> createSetting, out List<Element> roomsToCreate)
        {
            //获取指定房间
            List<Element> roomsList = new List<Element>();
            string paraName = createSetting[0];
            string paraValue = createSetting[1];
            if (paraValue != "全部生成")
            {
                foreach (Element ele in roomList)
                {
                    ParameterMap paraMap = ele.ParametersMap;
                    foreach (Parameter para in paraMap)
                    {
                        if (para.Definition.Name == paraName)
                        {
                            if (para.HasValue)
                            {
                                string value;
                                if (para.StorageType == StorageType.String)
                                {
                                    value = para.AsString();
                                }
                                else
                                {
                                    value = para.AsValueString();
                                }
                                if (paraValue == value)
                                {
                                    roomsList.Add(ele);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                roomsList = roomList;
            }

            List<CurveArray> curveArraysList = new List<CurveArray>();
            roomsToCreate = new List<Element>();
            foreach (Element element in roomsList)
            {
                Room room = element as Room;
                // 存储房间最大轮廓
                CurveArray curveArray = new CurveArray();
                //用于判定最大房间轮廓
                List<CurveLoop> curveLoopList = new List<CurveLoop>();
                //获得房间边界
                IList<IList<BoundarySegment>> roomBoundaryListList = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                //获取所有边界
                if (roomBoundaryListList != null)
                {
                    foreach (IList<BoundarySegment> roomBoundaryList in roomBoundaryListList)
                    {
                        CurveLoop curveLoop = new CurveLoop();
                        foreach (BoundarySegment roomBoundary in roomBoundaryList)
                        {
                            curveLoop.Append(roomBoundary.GetCurve());
                        }
                        curveLoopList.Add(curveLoop);
                    }
                }
                if (curveLoopList.Count == 0)
                {
                    continue;
                }
                //获取房间边界的拉伸体体积
                List<double> volumn = new List<double>();
                try
                {
                    foreach (CurveLoop curveLoop in curveLoopList)
                    {
                        IList<CurveLoop> clList = new List<CurveLoop>();
                        clList.Add(curveLoop);
                        Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(clList, XYZ.BasisZ, 1);
                        volumn.Add(solid.Volume);
                    }
                }
                catch
                {
                    continue;
                }

                //获取体积最大值的房间边界
                CurveLoop largeLoop = curveLoopList.ElementAt(volumn.IndexOf(volumn.Max()));
                foreach (Curve curve in largeLoop)
                {
                    curveArray.Append(curve);
                }
                curveArraysList.Add(curveArray);
                roomsToCreate.Add(element);
            }

            return curveArraysList;
        }
        /// <summary>
        /// 创建面层
        /// </summary>
        /// <param name="doc">当前项目对象</param>
        /// <param name="floorType">楼板面层类型</param>
        /// <param name="roomBoundaryList">房间边界线List</param>
        /// <returns></returns>
        private bool CreateSurface(Document doc, FloorType floorType, List<CurveArray> roomBoundaryList, List<Element> roomsList)
        {
            double thick = floorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
            using (Transaction trans = new Transaction(doc, "生成楼板面层"))
            {
                trans.Start();
                for (int i = 0; i < roomBoundaryList.Count; i++)
                {
                    Floor floor = doc.Create.NewFloor(roomBoundaryList[i], floorType, doc.GetElement(roomsList[i].Id) as Level, false);
                    floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(thick);
                    floor.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(roomsList[i].get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
                    roomsList[i].get_Parameter(BuiltInParameter.ROOM_FINISH_FLOOR).Set(floor.Name);
                }
                trans.Commit();
            }
            return true;
        }

        private List<string> ShowDialog(List<Element> roomlist, List<Element> floorTypeList)
        {
            List<string> value = new List<string>();

            List<Room> roomslist = roomlist.ConvertAll(x => x as Room); //将一个对象转换成另外一种对象
            List<string> parameterName = new List<string>();
            ParameterMap paraMap = roomlist.First().ParametersMap;
            foreach (Parameter para in paraMap)
            {
                parameterName.Add(para.Definition.Name);
            }

            List<string> floorTypesList = floorTypeList.ConvertAll(x => x.Name);

            MainWindow mainWindow = new CreateFloor.MainWindow(parameterName, roomslist, floorTypesList);

            if (mainWindow.ShowDialog() == true)
            {
                value.Add(mainWindow.parameterNameCombo.SelectedItem.ToString());
                value.Add(mainWindow.parameterValueCombo.SelectedItem.ToString());
                value.Add(mainWindow.floorTypeCombo.SelectedItem.ToString());
                return value;
            }
            return value;
        }
    }
}

using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace CreateFloor
{

    public partial class MainWindow : Window
    {
        List<Room> roomList;
        public MainWindow(List<string> parameterNameList, List<Room> roomList, List<string> floortypeList)
        {
            InitializeComponent();
            this.roomList = roomList;
            parameterNameCombo.ItemsSource = parameterNameList;
            floorTypeCombo.ItemsSource = floortypeList;

        }

        private void ParameterNameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string paraName = parameterNameCombo.SelectedItem.ToString();
            List<string> parameterValueList = new List<string>();
            parameterValueList.Add("全部生成");
            foreach (Room room in roomList)
            {
                ParameterMap paraMap = room.ParametersMap;
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
                            if (!(parameterValueList.Contains(value)))
                            {
                                parameterValueList.Add(value);
                            }
                        }
                    }
                }
            }
            parameterValueCombo.ItemsSource = parameterValueList;
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if ((parameterNameCombo.SelectedItem != null) && (parameterValueCombo.SelectedItem != null) && (floorTypeCombo.SelectedItem != null))
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parameterNameCombo.SelectedIndex = 0;
            parameterValueCombo.SelectedIndex = 0;
            floorTypeCombo.SelectedIndex = 0;
        }
    }
}

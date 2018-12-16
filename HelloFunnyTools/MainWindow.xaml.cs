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
            foreach (Room room in roomList)
            {
                ParameterMap paraMap = room.ParametersMap;
                foreach (Parameter para in paraMap)
                {
                    if (para.Definition.Name == paraName)
                    {

                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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

namespace gui_form
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string test_text;

        public MainWindow()
        {
            List<GridItem> gitems = new List<GridItem>();
        List<string> jlist = new List<string>();
        List<string> alist = new List<string>();
        int count = 0;
        public MainWindow()
        {
            InitializeComponent();
            List<String> joints = new List<String>{ "Head","Left Ankle", "Left Elbow", "Left Foot", "Left Hand", "Left Hip", "Left Knee", "Left Shoulder","Left Wrist", "Neck", "Pelvis", "Right Ankle" , "Right Elbow" , "Right Foot" , "Right Hand","Right Hip", "Right Knee", "Right Shoulder","Right Wrist", "Stomach"};
            List<String> actions = new List<String> { "Volume", "Pan" };
            ComboBox1.ItemsSource = joints;
            ComboBox2.ItemsSource = actions;
            DGrid.ItemsSource = gitems;
        }

      
        //Get the value chosen in the Joints Dropdown box
        /* private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("" + ComboBox1.SelectedValue);
         * 
         * //xaml file
         * //Window x: 
         * //Loaded="Window_Loaded_1">
        }*/

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("Joint: " + ComboBox1.SelectedValue + "      Action: " + ComboBox2.SelectedValue);
            GridItem itemz = new GridItem{ JointID = " "+ComboBox1.SelectedValue,ActionID=" "+ComboBox2.SelectedValue };
            gitems.Add(itemz);
            DGrid.Items.Refresh();
            /*string[,] values = new string[,]
            {
                {" "+ComboBox1.SelectedValue, ""+ComboBox2.SelectedValue}
            };
            MessageBox.Show("" + values[0,0]);*/
            //List<string> jlist = new List<string>();
            jlist.Add("" + ComboBox1.SelectedValue);
            alist.Add("" + ComboBox2.SelectedValue);
            count++;   
        }

        private void ComboBox1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //Label1.Content = ComboBox1.SelectedValue;
        }

        private void DGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void onChooseData(object sender, RoutedEventArgs e)
        {
            /*string chosenvalues = null;
            MessageBox.Show("Joint: " + ComboBox1.SelectedValue + "    Action: " + ComboBox2.SelectedValue);
            for (int i = 0; i < jlist.Count; i++) // Loop through List with for
            {
                //MessageBox.Show("" + jlist[i]);
                chosenvalues = "" + chosenvalues+ jlist[i] + " : " + alist[i]+ " ; ";
                               
            }
            MessageBox.Show("" + chosenvalues);*/
            chosenString();
            
        }

        private string chosenString()
        {
            string chosenvalues = null;
            MessageBox.Show("Joint: " + ComboBox1.SelectedValue + "    Action: " + ComboBox2.SelectedValue);
            for (int i = 0; i < jlist.Count; i++) // Loop through List with for
            {
                //MessageBox.Show("" + jlist[i]);
                chosenvalues = "" + chosenvalues+ jlist[i] + " : " + alist[i]+ " ; ";
                               
            }
            MessageBox.Show("" + chosenvalues);
            return chosenvalues;
        }

        //    InitializeComponent();
        //}

        private void txtTest_TextChanged(object sender, TextChangedEventArgs e)
        {
            test_text = txtTest.Text;
        }
        
    }
}

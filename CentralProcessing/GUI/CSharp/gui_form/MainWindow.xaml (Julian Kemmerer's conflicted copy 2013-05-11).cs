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
        //Global setting string
        //Updated when used clicks send settings
        //Constantly being requested from OF
        string global_settings_string;

        //Bool to send the close form event
        //String will contain 1 or zero ...everything is a string
        string do_close = "0";

        /*
        List<GridItem> gitems = new List<GridItem>();
        List<string> jlist = new List<string>();
        List<string> alist = new List<string>();
        int count = 0;
        */

        public MainWindow()
        {
            InitializeComponent();
            /*
            List<String> joints = new List<String> { "Head", "Left Ankle", "Left Elbow", "Left Foot", "Left Hand", "Left Hip", "Left Knee", "Left Shoulder", "Left Wrist", "Neck", "Pelvis", "Right Ankle", "Right Elbow", "Right Foot", "Right Hand", "Right Hip", "Right Knee", "Right Shoulder", "Right Wrist", "Stomach" };
            List<String> actions = new List<String> { "Volume", "Pan" };
            ComboBox1.ItemsSource = joints;
            ComboBox2.ItemsSource = actions;
            DGrid.ItemsSource = gitems;
             */
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

        /*
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("Joint: " + ComboBox1.SelectedValue + "      Action: " + ComboBox2.SelectedValue);
            GridItem itemz = new GridItem { JointID = " " + ComboBox1.SelectedValue, ActionID = " " + ComboBox2.SelectedValue };
            gitems.Add(itemz);
            DGrid.Items.Refresh();
            jlist.Add("" + ComboBox1.SelectedValue);
            alist.Add("" + ComboBox2.SelectedValue);
            count++;
        }
         * */
        /*
        private void ComboBox1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //Label1.Content = ComboBox1.SelectedValue;
        }

        private void onChooseData(object sender, RoutedEventArgs e)
        {
            chosenString();
        }

        private string chosenString()
        {
            string chosenvalues = null;
            MessageBox.Show("Joint: " + ComboBox1.SelectedValue + "    Action: " + ComboBox2.SelectedValue);
            for (int i = 0; i < jlist.Count; i++) // Loop through List with for
            {
                //MessageBox.Show("" + jlist[i]);
                chosenvalues = "" + chosenvalues + jlist[i] + " : " + alist[i] + " ; ";

            }
            MessageBox.Show("" + chosenvalues);
            return chosenvalues;
        }
       


       //     InitializeComponent();
       // }

        private void txtTest_TextChanged(object sender, TextChangedEventArgs e)
        {
        //    test_text = txtTest.Text;
        }
         * */

        //Get the setting information from the form
        public string getSettingsString()
        {
            //Clear and return the settings
            string tmp = global_settings_string;
            global_settings_string = "";
            return tmp;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            do_close = "1";
            //Send only this setting
            //do_close
            global_settings_string += "do_close:" + do_close + "\n";
        }

        private void btnSendSettings_Click(object sender, RoutedEventArgs e)
        {
            //Go through every control on the form and give information
            //Use format
            // "settingName:settingvalue\n"

            //Get chirp info
            global_settings_string += chirpSettings();

            //Send flag saying that the form is closing...
            global_settings_string += "do_close:" + do_close + "\n";

            //Get skeleton and effect info
            global_settings_string += skeletonSettings();
        }

        string skeletonSettings()
        {
            string rv = "";
            //Skeleton index
            //Must be before mappings in string
            rv += "SkeletonIndex:" + txtSkeletonIndex.Text + "\n";
            rv += "KinectIndex:" + txtKinectIndex.Text + "\n";

            //Hand right
            rv += "HandRightMap:" + cmbHandRightMap.Text + "\n";
            return rv;
        }

        string chirpSettings()
        {
            string rv = "";
            rv+= flChirpSettings();
            rv+= frChirpSettings();
            rv+= slChirpSettings();
            rv+= srChirpSettings();
            return rv;
        }

        string flChirpSettings()
        {
            string rv = "";
            rv += "flStartFreq:" + flStartFreq.Text + "\n";
            rv += "flEndFreq:" + flEndFreq.Text + "\n";
            rv += "flDur:" + flDur.Text + "\n";
            rv += "flTime:" + flTime.Text + "\n";
            rv += "flPer:" + flPer.Text + "\n";
            return rv;
        }
        string frChirpSettings()
        {
            string rv = "";
            rv += "frStartFreq:" + frStartFreq.Text + "\n";
            rv += "frEndFreq:" + frEndFreq.Text + "\n";
            rv += "frDur:" + frDur.Text + "\n";
            rv += "frTime:" + frTime.Text + "\n";
            rv += "frPer:" + frPer.Text + "\n";
            return rv;
        }
        string slChirpSettings()
        {
            string rv = "";
            rv += "slStartFreq:" + slStartFreq.Text + "\n";
            rv += "slEndFreq:" + slEndFreq.Text + "\n";
            rv += "slDur:" + slDur.Text + "\n";
            rv += "slTime:" + slTime.Text + "\n";
            rv += "slPer:" + slPer.Text + "\n";
            return rv;
        }
        string srChirpSettings()
        {
            string rv = "";
            rv += "srStartFreq:" + srStartFreq.Text + "\n";
            rv += "srEndFreq:" + srEndFreq.Text + "\n";
            rv += "srDur:" + srDur.Text + "\n";
            rv += "srTime:" + srTime.Text + "\n";
            rv += "srPer:" + srPer.Text + "\n";
            return rv;
        }


        private void sldVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Send only this setting
            
            //Volume
            global_settings_string += "GlobalVolume:" + sldVol.Value / 100.0 + "\n";

        }
        
    }
}

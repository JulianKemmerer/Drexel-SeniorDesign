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
            List<String> stems = new List<String> { "Vocals", "Guitar","Drums","Bass" };
            cmbStem.ItemsSource = stems;
            /*
            List<String> joints = new List<String> { "Head", "Left Ankle", "Left Elbow", "Left Foot", "Left Hand", "Left Hip", "Left Knee", "Left Shoulder", "Left Wrist", "Neck", "Pelvis", "Right Ankle", "Right Elbow", "Right Foot", "Right Hand", "Right Hip", "Right Knee", "Right Shoulder", "Right Wrist", "Stomach" };
            List<String> actions = new List<String> { "Volume", "Pan" };
            ComboBox1.ItemsSource = joints;
            ComboBox2.ItemsSource = actions;
            DGrid.ItemsSource = gitems;
             */
        }

        List<ComboBox> cmbs = new List<ComboBox>(); //Have list be global
        //For ease of programming map index to names
        Dictionary<ComboBox, String> cmbToName = new Dictionary<ComboBox, string>();
        private void Window_Loaded(object sender, EventArgs e)
        {
            //Build combo box entries
            //First create list of comboboxes to fill
            //And populate map
            cmbs.Add(cmbHandLeftMap); cmbToName.Add(cmbHandLeftMap, "HandLeft");
            cmbs.Add(cmbHandRightMap); cmbToName.Add(cmbHandRightMap, "HandRight");
            cmbs.Add(cmbWristLeftMap); cmbToName.Add(cmbWristLeftMap, "WristLeft");
            cmbs.Add(cmbWristRightMap); cmbToName.Add(cmbWristRightMap, "WristRight");
            cmbs.Add(cmbElbowLeftMap); cmbToName.Add(cmbElbowLeftMap, "ElbowLeft");
            cmbs.Add(cmbElbowRightMap); cmbToName.Add(cmbElbowRightMap, "ElbowRight");
            cmbs.Add(cmbShoulderLeftMap); cmbToName.Add(cmbShoulderLeftMap, "ShoulderLeft");
            cmbs.Add(cmbShoulderRightMap); cmbToName.Add(cmbShoulderRightMap, "ShoulderRight");
            //cmbs.Add(cmbStem); cmbToName.Add(cmbStem, "Stem");
            fillComboBoxes(cmbs);
        }

        void fillComboBoxes(List<ComboBox> l)
        {
            foreach(ComboBox c in l)
            {
                fillComboBox(c);
            }
        }
        string track_volume_option_string = "Music Track Volume...";
        string track_pan_option_string = "Music Track Pan...";

        void fillComboBox(ComboBox c)
        {
            //These are the items that a joint can be mapped to
            //Throw in a global if you want to us that text elsewhere see above declarations ^
            //If you add things here you must also add them in string_consts.cpp in C++
            c.Items.Add("Global Pan");
            c.Items.Add("Global Volume");
            
            //These don't count as items to add to string_const.cpp
         //   c.Items.Add(track_volume_option_string);
         //   c.Items.Add(track_pan_option_string);
        }

        

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

            //Get combo box mappings - must come after skeleton and kinect index
            rv += jointMappings();
            
            return rv;
        }

        string jointMappings()
        {
            string rv = "";
            //Loop through dict of cmb box and name to construct settings
            foreach (KeyValuePair<ComboBox,String> pair in cmbToName)
            {
                //Check that there is a selection
                if (pair.Key.SelectedIndex >= 0)
                {
                    //Build map string
                    //Old way: rv += "HandRightMap:" + cmbHandRightMap.Text + "\n";
                    rv += pair.Value + "Map:" + pair.Key.Text+ "\n";
                }
            }
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

        private void slVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Send only this setting
            //Surround Left Volume
            global_settings_string += "SLVolume:" + slVol.Value / 100.0 + "\n";
        }

        private void srVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Send only this setting
            //Surround Left Volume
            global_settings_string += "SRVolume:" + slVol.Value / 100.0 + "\n";
        }

        private void flVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Send only this setting
            //Surround Left Volume
            global_settings_string += "FLVolume:" + slVol.Value / 100.0 + "\n";
        }

        private void frVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Send only this setting
            //Surround Left Volume
            global_settings_string += "FRVolume:" + slVol.Value / 100.0 + "\n";
        }

        void cmbSelectionEvent(ComboBox sender, int selected_index, String selected_text)
        {
            if ((selected_text == track_volume_option_string) || (selected_text == track_pan_option_string))
            {
                //Ask them for the file to play
                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.ShowDialog();
                if (dlg.FileName == "") return;
                string filename = dlg.FileName;
                //Add this as a new option for all comboboxes
                //Loop through dict of cmb box and name to construct settings
                foreach (KeyValuePair<ComboBox, String> pair in cmbToName)
                {
                    string prefix = "";
                    if(selected_text == track_volume_option_string)
                    {
                        prefix = "Track Volume";
                    }
                    else if (selected_text == track_pan_option_string)
                    {
                        prefix = "Track Pan";
                    }
                    //Hey, the | char is a fun one...
                    pair.Key.Items.Add(prefix + "|" + filename);
                }
                //Make this the selected item
                int last_item = sender.Items.Count - 1;
                sender.SelectedIndex = last_item;
            }
        }

        private void cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Do same function for all mappings
            cmbSelectionEvent((ComboBox)sender, ((ComboBox)sender).SelectedIndex, ((String)((ComboBox)sender).SelectedItem));
        }
             
    }
}

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
using System.Xaml;
using gui_form;

/*
//After build, open developer command prompt as admin and run:
//Julian's system:
//cd "C:\Dropbox\Drexel\Classes\Junior Year\Senior Design\CentralProcessing\GUI\CSharp\gui_dll\bin\Debug" && RegAsm.exe gui_dll.dll /tlb:gui_dll.tlb /codebase && RegAsm.exe gui_form.exe /tlb:gui_form.tlb /codebase
//Matt's System:
//cd "C:\Users\Zimmerman\Dropbox\Senior Design\CentralProcessing\GUI\CSharp\gui_dll\bin\Debug" && RegAsm.exe gui_dll.dll /tlb:gui_dll.tlb /codebase && RegAsm.exe gui_form.exe /tlb:gui_form.tlb /codebase

 */


namespace gui_dll
{
    public interface IGUI_DLL
    {
        void start_form();
        string getSettingsString();
    }

    public class gui_class : IGUI_DLL
    {
        MainWindow the_window;
        App the_app;
        public gui_class()
        {

        }

        public void start_form()
        {
            the_app = new App();
            the_window = new MainWindow();
            the_window.ShowDialog();
        }
        public string getSettingsString()
        {
            return the_window.getSettingsString();
        }
    }
}

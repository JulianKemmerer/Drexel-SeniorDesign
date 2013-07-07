#include "ofMain.h"

//COM intefaces
//To use: Open openFrameworks / libs / poco / include / Poco / UnWindows.h
//Comment out line: #undef FormatMessage
#include <comdef.h> 
#include <WinBase.h>

//Import the gui dll
//After build open developer command prompt and run:
//Julian's system:
//cd "C:\Dropbox\Drexel\Classes\Junior Year\Senior Design\CentralProcessing\GUI\CSharp\gui_dll\bin\Debug" && RegAsm.exe gui_dll.dll /tlb:gui_dll.tlb /codebase && RegAsm.exe gui_form.exe /tlb:gui_form.tlb /codebase
#import "..\..\..\..\..\..\CSharp\gui_dll\bin\Debug\gui_dll.tlb" raw_interfaces_only
#import "..\..\..\..\..\..\CSharp\gui_dll\bin\Debug\gui_form.tlb" raw_interfaces_only

using namespace gui_dll;

class GUIForm: public ofThread
{
	//Pointer representing the C# class
	IGUI_DLLPtr pGUI_DLL;

public:

	void GUIForm::init_gui_thread()
	{
		//Initialize the COM interface
		// Make a smart pointer to the interface in DLL
		HRESULT hr = CoInitialize(NULL);
		pGUI_DLL = IGUI_DLLPtr(__uuidof(gui_class));
	}

	void GUIForm::start_form()
	{
		//Form runs in own thread, run the threaded function
		startThread(true,false);
	}

	//This is there the form is started
	void GUIForm::threadedFunction()
	{
		HRESULT hr = CoInitialize(NULL);
		hr = pGUI_DLL->start_form();
		//This function does not end
	}

	//Function to return a string of gui info
	string GUIForm::getInfo()
	{
		BSTR result;
		HRESULT hr = CoInitialize(NULL);


		hr = pGUI_DLL->getSettingsString(&result);
		//Make sure non null string returns
		if(result != NULL)
		{
			_bstr_t bstrResult(result, true); // passing true means you should not call SysFreeString
			return LPCSTR(bstrResult);
		}
		return "";
	}

	//Release the COM interface sometime?
	//CoUninitialize();
};

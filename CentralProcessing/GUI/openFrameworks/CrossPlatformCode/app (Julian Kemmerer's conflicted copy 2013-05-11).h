#pragma once

#include "ofMain.h"
#include <string>
#include <iostream>
#pragma comment(lib, "ws2_32.lib")
#define OF_ADDON_USING_OFXOSC
#include "ofxOsc.h"
#define OF_SOUNDSTREAM_PORTAUDIO
#include "ofPASoundStream.h"
#include "portaudio.h"
#include "gui_form.h"
#include "skeleton.h"
#include "sndfile.h"
#include "sndfile.hh"
#include "audioeffect.h"


/*
//Import the gui dll
//After build, open developer command prompt and run:
//Julian's system:
//cd "C:\Dropbox\Drexel\Classes\Junior Year\Senior Design\CentralProcessing\GUI\CSharp\gui_dll\bin\Debug" && RegAsm.exe gui_dll.dll /tlb:gui_dll.tlb /codebase && RegAsm.exe gui_form.exe /tlb:gui_form.tlb /codebase
//Matt's System:
//cd "C:\Users\Zimmerman\Dropbox\Senior Design\CentralProcessing\GUI\CSharp\gui_dll\bin\Debug" && RegAsm.exe gui_dll.dll /tlb:gui_dll.tlb /codebase && RegAsm.exe gui_form.exe /tlb:gui_form.tlb /codebase
*/


class app : public ofBaseApp{
	public:
		//Functions setup by OF
		//Run once at start
		void setup();
		//Update and draw run one after
		//until the end of the program
		void update();
		void draw();
		//General events
		void keyPressed(int key);
		void keyReleased(int key);
		void mouseMoved(int x, int y);
		void mouseDragged(int x, int y, int button);
		void mousePressed(int x, int y, int button);
		void mouseReleased(int x, int y, int button);
		void windowResized(int w, int h);
		void dragEvent(ofDragInfo dragInfo);
		void gotMessage(ofMessage msg);

		//Extra closing event to clean up various things
		//Built into OF but not shown
		//This event comes from the Window X button in top corner or escape button
		void exit();
		void soft_exit();

		//Sound stuff
		//Function that asks for a sound buffer to be filled
		void audioRequested(float * output, int bufferSize, int nChannels);
		ofPASoundStream pa_soundstream;
		//ofSoundStream soundstream;
		//Init sound
		void init_sound();
		int findNOutputChannelIndex(int numChannels);
		void clean_sound();
		//Sound parameters
		int numOutputChannels;
		int numInputChannels;
		int global_sampleRate;
		int global_buffsize;
		int numWindows;
		float pan;
		float global_volume;
		//Channel specific params (later put in class with write method per speaker??)
		float fr_vol; //Front right vol
		float fl_vol; 
		float sr_vol;
		float sl_vol;
		//Global list of audio effects
		vector<AudioEffect *> audioEffects;
		void init_audio_effects();

		//Wave read stuff
		SndfileHandle sndfilehandle;
		string wav_filename;
		void test_wav_read();

		//OSC Stuff
		ofxOscReceiver osc_receiver;
		int osc_recv_port;
		void init_osc_recv();
		void process_osc_messages();
		void process_message(ofxOscMessage * msg);
		string SKELETON_METHOD;
		//Kepe track of bundle count to not accept late bundles
		long latest_bundle_index;

		//Stuff for dealing with kinect skeletons
		Skeleton skeletonOSCMessageToSkeleton(ofxOscMessage * msg);
		vector<Skeleton> skeletons;
		void addOrUpdateSkeletonsList(Skeleton * s);
		//Draw the list of skeletons on screen
		void drawSkeletons(ofRectangle boundingbox);
		void drawSkeleton_doEffect(Skeleton * s, ofRectangle boundingbox);
		void drawBone_doEffect(Bone * b, ofPoint multiplier, ofPoint constant, Skeleton * s);
		void drawJoint_doEffect(Joint* j, ofPoint multiplier, ofPoint constant,Bone * b, Skeleton * s);
		int current_gui_selected_skeleton;
		int current_gui_selected_kinect;
		void applyJointMapping(int skeletonIndex, int kinectIndex, string JointType, string mapping);
		void doSkeletonEffect(Skeleton * s);
		void doBoneEffect(Bone * b, ofPoint multiplier, ofPoint constant, Skeleton * s);
		void doJointEffect(Joint* j, ofPoint multiplier, ofPoint constant,Bone * b, Skeleton * s);
		void drawJoint(Joint* j, ofPoint multiplier, ofPoint constant);
		int numSkeletonOSCArgs;

		//Audio effects section?
		void doPanEffect(float panX, float panY);

		//Split functions stolen from stack overflow
		std::vector<std::string> & split(const std::string &s, char delim, std::vector<std::string> &elems);
		std::vector<std::string> split(const std::string &s, char delim);

		//Object representing the C# gui
		GUIForm gui;
		//Allocate a string to hold the current GUI settings
		string current_GUI_settings;
		void applySettings(string settings_string);
		void applySettingString(string setting);
		void applySetting(string name, string value);
		char setting_delim;
		char setting_arg_delim;
		//Possible messages from the GUI
		//Take form <param>:<value>\n
		static const string Setting_Do_Close;
		static const string Setting_SkeletonIndex;
		static const string Setting_HandRightMap;
		static const string Setting_GlobalVolume;
		//Setting checkers
		bool generalSettings(string name, string value);
		bool chirpSettings(string name, string value);
		vector<Chirp *> getChirpsFromString(string str);
		void init_add_and_start_chirp(float dur_ms,float freq_start,float freq_end,float rep_period, float volume, float start_offset_ms, string speaker_id);
		//Global list of chirps
		vector<Chirp*> chirps;
		//Skeleton settings from gui
		bool skeletonSettings(string name, string value);

};

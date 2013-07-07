#include "app.h"

void app::soft_exit()
{
	//Call regular exit function
	//exit();
	//Then kill program
	//std kill also forces of exit call ... do not call from here
	std::exit(0);
}

void app::exit()
{
	//Called when OF closes
	//Do sound clean up here
	clean_sound();
}

void app::clean_sound()
{
	pa_soundstream.stop();
	pa_soundstream.close();

	//Loop through all audio effects and close
	int num_effects = audioEffects.size();
	for(int i = 0; i < num_effects; i++)
	{
		audioEffects[i]->close();
	}
}

void app::test_wav_read()
{
	wav_filename = "..\\..\\..\\..\\..\\..\\..\\WAV Files\\BornToRun.wav";
	sndfilehandle = SndfileHandle(wav_filename.c_str());
	//Print info
	cout << "Sample rate: " << sndfilehandle.samplerate() << endl;
	cout << "Channels: " << sndfilehandle.channels() << endl;
	cout << "Error: " << sndfilehandle.error() << endl;
	cout << "Frames: " << sndfilehandle.frames() << endl;
}

void app::init_osc_recv()
{
	int port = 5103;
	osc_receiver.setup(port);
	SKELETON_METHOD = "/kinect/skeleton";
	latest_bundle_index = 0;
	char setting_delim = '\n';
	char setting_arg_delim = ':';
	//Reset skeleton index
	current_gui_selected_skeleton = -1;
	numSkeletonOSCArgs = 19;
}

void app::process_osc_messages()
{
	//Allocate a new message
	//Will be overridden each iteration of the while loop
	ofxOscMessage * msg = new ofxOscMessage();
	while(osc_receiver.hasWaitingMessages())
	{
		//Get the message
		osc_receiver.getNextMessage(msg);
		//Process the message
		process_message(msg);
	}
	//Free the allocation
	free(msg);
}

void app::process_message(ofxOscMessage * msg)
{
	//Get bundle index
	long bundle_index = msg->getArgAsInt64(0);
	//If this on time (not late)
	if(bundle_index >= latest_bundle_index)
	{
		//Keep track of most recent packet
		latest_bundle_index = bundle_index;

		//Strip information from the msg
		//If this message is a kinect skeleton message
		//"Address" instead of "method"? ...Alright
		if(msg->getAddress().find(SKELETON_METHOD) != -1)
		{
			//Get the skeleton object
			Skeleton s = skeletonOSCMessageToSkeleton(msg);
			//Add or update the skeleton in the list
			addOrUpdateSkeletonsList(&s);
		}
		else
		{
			cout << "Unknown OSC Method: " << msg->getAddress() << endl;
		}
	}
	else
	{
		cout << "Dropped OSC bundle: " << bundle_index << endl;
	}
}

Skeleton app::skeletonOSCMessageToSkeleton(ofxOscMessage * msg)
{
	//If this message is not formatted correctly
	if(msg->getNumArgs() !=numSkeletonOSCArgs)
	{
		cout << "Skeleton OSC message does not have " << numSkeletonOSCArgs << " args as required... - has " <<  msg->getNumArgs() << endl;
	}

	//This is tied directly to the order of information within the packet
	Skeleton s;
	// 0 - bundle index, not used here
	// 1 - skeleton index
	s.idx = msg->getArgAsInt32(1);
	// 2-HandLeftx
	// 3-HandLefty
	Joint handleft = Joint(ofPoint(msg->getArgAsFloat(2),msg->getArgAsFloat(3)), Joint::Type_HandLeft);
	// 4-WristLeftx
	// 5-WristLefty
	Joint wristleft = Joint(ofPoint(msg->getArgAsFloat(4),msg->getArgAsFloat(5)), Joint::Type_WristLeft);
	// 6-ElbowLeftx
	// 7-ElbowLefty
	Joint elbowleft = Joint(ofPoint(msg->getArgAsFloat(6),msg->getArgAsFloat(7)), Joint::Type_ElbowLeft);
	// 8-ElbowRightx
	// 9-ElbowRighty
	Joint elbowright = Joint(ofPoint(msg->getArgAsFloat(8),msg->getArgAsFloat(9)), Joint::Type_ElbowRight);
	// 10-WristRightx
	// 11-WristRight7
	Joint wristright = Joint(ofPoint(msg->getArgAsFloat(10),msg->getArgAsFloat(11)), Joint::Type_WristRight);
	// 12-HandRightx
	// 13-HandRighty
	Joint handright = Joint(ofPoint(msg->getArgAsFloat(12),msg->getArgAsFloat(13)), Joint::Type_HandRight);
	// 14-ShoulderLeftx
	// 15-ShoulderLefty
	Joint shoulderleft = Joint(ofPoint(msg->getArgAsFloat(14),msg->getArgAsFloat(15)), Joint::Type_ShoulderLeft);
	// 16-ShoulderRightx
	// 17-ShoulderRight7
	Joint shoulderright = Joint(ofPoint(msg->getArgAsFloat(16),msg->getArgAsFloat(17)), Joint::Type_ShoulderRight);
	// 18 - kinect id
	s.src_kinect = msg->getArgAsInt32(18);

	//Now construct the list of bones
	//Hand->wrist
	s.bones.push_back(Bone(handleft,wristleft));
	s.bones.push_back(Bone(handright,wristright));
	//Wrist->Elbow
	s.bones.push_back(Bone(wristleft,elbowleft));
	s.bones.push_back(Bone(wristright,elbowright));
	//Elbow->Shoulder
	s.bones.push_back(Bone(elbowleft,shoulderleft));
	s.bones.push_back(Bone(elbowright,shoulderright));
	//Shoulders
	s.bones.push_back(Bone(shoulderleft,shoulderright));

	return s;
}

void app::addOrUpdateSkeletonsList(Skeleton * s)
{
	//Loop through list of skeletons
	int num_skeletons = skeletons.size();
	for(int i = 0; i < num_skeletons; ++i)
	{
		//if index and src kinect match
		if((skeletons[i].idx == s->idx) && (skeletons[i].src_kinect == s->src_kinect) )
		{
			skeletons[i] = *s;
			return;
		}
	}

	//not found
	skeletons.push_back(*s);
}

void app::drawSkeletons(ofRectangle boundingbox)
{
	//Drawing many skeletons within this bounding box
	//Split up the bounding box
	//First find out how many skeletons we are dealing with
	int num_skeletons = skeletons.size();
	if(num_skeletons == 0) return;

	//1 takes up the whole screen
	if(num_skeletons ==1)
	{
		drawSkeleton_doEffect(&(skeletons[0]),boundingbox);
	}
	//2 are side by side
	else if(num_skeletons ==2)
	{
		//Split the bounding box in half by x direction
		ofRectangle boundingbox_left = boundingbox;
		ofRectangle boundingbox_right = boundingbox;
		boundingbox_left.setWidth(boundingbox_left.getWidth()/2.0);
		boundingbox_right.setWidth(boundingbox_right.getWidth()/2.0);
		//Move the "right" box to the right by a widths length
		boundingbox_right.setX(boundingbox_right.getX() + boundingbox_right.getWidth());
		drawSkeleton_doEffect(&(skeletons[0]), boundingbox_left);
		drawSkeleton_doEffect(&(skeletons[1]), boundingbox_right);
	}
	else
	{
		cout << "Unimplemented number of skeletons to draw..." << num_skeletons << endl;
	}
	//3 ... TODO
}

void app::drawSkeleton_doEffect(Skeleton * s, ofRectangle boundingbox)
{
	//Draw a single skeleton in the bounding box
	//All points in the skeleton are with a 0,0 point at the center
	//And a range of -1 to 1 on both the x and y axis
	//Need to know multiplier and constant
	float multiplierX = boundingbox.getWidth()/2.0;
	float constantX = boundingbox.x + (boundingbox.getWidth()/2.0);
	float multiplierY = -1.0 * boundingbox.getHeight()/2.0; //Negative since 0 is at top of window
	float constantY = boundingbox.y + (boundingbox.getHeight()/2.0);
	ofPoint m = ofPoint(multiplierX, multiplierY);
	ofPoint c = ofPoint(constantX, constantY);

	//Loop through each bone and draw them
	int num_bones = s->bones.size();
	for(int i = 0; i < num_bones; ++i)
	{
		drawBone_doEffect(&(s->bones[i]),m,c,s);
	}

	//Do this level's effect as seperate function
	doSkeletonEffect(s);
}

void app::doSkeletonEffect(Skeleton * s)
{
	//Nothing really to do here yet?
}

void app::drawBone_doEffect(Bone * b, ofPoint multiplier, ofPoint constant, Skeleton * s)
{
	//Draw the line representing the bone (scaled)
	//Loop through the joints two at a time
	int num_joints = b->num_joints;
	for(int i = 0; i < num_joints; i+=2)
	{
		//Draw bones as blue for now
		ofSetColor(ofColor::blue);
		ofLine(ofPoint(b->joints[i].point.x*multiplier.x + constant.x,b->joints[i].point.y*multiplier.y + constant.y),
			   ofPoint(b->joints[i+1].point.x*multiplier.x + constant.x,b->joints[i+1].point.y*multiplier.y + constant.y)
			   );
	}

	//Then draw all joints
	for(int i = 0; i < num_joints; ++i)
	{
		drawJoint_doEffect(&(b->joints[i]),multiplier,constant,b,s);
	}

	//Do this level's effect as seperate function
	doBoneEffect(b, multiplier, constant,s);
}

void app::doBoneEffect(Bone * b, ofPoint multiplier, ofPoint constant, Skeleton * s)
{
	//Nothing to do here yet...
}

void app::drawJoint_doEffect(Joint* j, ofPoint multiplier, ofPoint constant, Bone * b, Skeleton * s)
{
	//Draw
	drawJoint(j, multiplier, constant);
	
	//Do this level's effect as seperate function
	doJointEffect(j, multiplier, constant,b,s);
}

void app::drawJoint(Joint* j, ofPoint multiplier, ofPoint constant)
{
	ofPoint realpt = ofPoint(j->point.x*multiplier.x + constant.x,j->point.y*multiplier.y + constant.y);
	//Draw text near joint
	if(j->mapping != "")
	{
		ofDrawBitmapString("	Mapping: " + j->mapping,realpt);
	}

	//Draw joints as red for now
	ofSetColor(ofColor::red);
	float joint_radius = 5;
	ofCircle(realpt,joint_radius);
}

void app::doJointEffect(Joint* j, ofPoint multiplier, ofPoint constant,Bone * b, Skeleton * s)
{
	//Look at the mapping this join has
	if(j->mapping == Joint::Mapping_Pan)
	{
		//Do pan effect
		doPanEffect(j->point.x,j->point.y);
	}
}

void app::doPanEffect(float panX, float panY)
{
	//Expecting panX and Y to be -1 to 1
	if(panX > 1)
	{
		panX = 1;
	}
	if(panY > 1)
	{
		panY = 1;
	}
	if(panX < -1)
	{
		panX = -1;
	}
	if(panY < -1)
	{
		panY = -1;
	}

	//First reset all the pan levels
	fr_vol = 1;
	fl_vol = 1;
	sl_vol = 1;
	sr_vol = 1;

	//Calculate the speaker volumes
	//Front to back pan is Y value
	//-1 is back, 1 is front
	//Shift y value up 1 to make range 0 to 2
	panY = panY + 1;
	//Then half to make range 0  to 1
	panY = panY / 2.0;
	//Front gets * panY, back get * (panY-1)
	fr_vol *= panY;
	fl_vol *= panY;
	sr_vol *= (panY-1);
	sl_vol *= (panY-1);

	//Do similar thing for left right with panX
	panX = panX + 1;
	panX = panX / 2.0;
	fr_vol *= panX;
	sr_vol *= panX;
	fl_vol *= (panX-1);
	sl_vol *= (panX-1);
}

void app::init_sound()
{
	//Do sound setup
	numOutputChannels = 8;
	numInputChannels = 0;
	global_sampleRate = 44100;
	global_buffsize = 2048;
	numWindows = 1;
	//Setup the sound stream
	//pa_soundstream.listDevices();
	int outputIndex = findNOutputChannelIndex(numOutputChannels);
	if(outputIndex == -1)
	{
		cout << "Could not find " << numOutputChannels << " channel output device." << endl;
		soft_exit();
	}
	pa_soundstream.setDeviceID(outputIndex);
	pa_soundstream.setup(this,numOutputChannels,numInputChannels,global_sampleRate,global_buffsize,numWindows);
	//Volume init
	global_volume = 0.0;
	//Volumes
	fr_vol = 1; //Front right vol
	fl_vol = 1; 
	sr_vol = 1;
	sl_vol = 1;

	//Init audio effects
	init_audio_effects();
}

void app::init_audio_effects()
{
	//Add a test effect to the global list
	SingleWAVFileAudioPlayer * swfap = new SingleWAVFileAudioPlayer();\
	swfap->filename = "..\\..\\..\\..\\..\\..\\..\\WAV Files\\BornToRun.wav";
	swfap->buffsize = global_buffsize;
	swfap->samplerate = global_sampleRate;
	swfap->fl_enabled = true;
	swfap->fr_enabled = true;
	swfap->sl_enabled = true;
	swfap->sr_enabled = true;
	swfap->volume = 1;
	swfap->init();
	//Add to global list
	audioEffects.push_back(swfap);

	init_add_and_start_chirp(500,440,880,2000,1,0,"fl");
}

void app::init_add_and_start_chirp(float dur_ms,float freq_start,float freq_end,float rep_period, float volume, float start_offset_ms, string speaker_id)
{
	Chirp * chirp = new Chirp();
	chirp->buffsize = global_buffsize;
	chirp->samplerate = global_sampleRate;
	chirp->duration_in_ms = dur_ms;
	chirp->freqStart = freq_start;
	chirp->freqEnd = freq_end;
	chirp->repeatPeriodMS = rep_period;
	chirp->volume = volume;
	chirp->init();
	chirp->fl_enabled = false;
	chirp->fr_enabled = false;
	chirp->sl_enabled = false;
	chirp->sr_enabled = false;
	//Enable correct channels
	if(speaker_id == "fl") chirp->fl_enabled = true;
	if(speaker_id == "fr") chirp->fr_enabled = true;
	if(speaker_id == "sl") chirp->sl_enabled = true;
	if(speaker_id == "sr") chirp->sr_enabled = true;
	audioEffects.push_back(chirp);
	//Add to global list
	chirps.push_back(chirp);
	chirp->startChirping();
}

int app::findNOutputChannelIndex(int numChannels)
{
	//Based off of the list devices code
	int numDevices;
	numDevices = Pa_GetDeviceCount();
	if( numDevices < 0 )
	{
    	ofLog(OF_LOG_ERROR,"PortAudio error: %s\n",Pa_GetErrorText( numDevices ));
    	return -1;
	}
	const   PaDeviceInfo *deviceInfo;

	for( int i=0; i<numDevices; i++ )
	{
		deviceInfo = Pa_GetDeviceInfo( i );
		if(deviceInfo->maxOutputChannels == numChannels)
		{
			return i;
		}
	}
	return -1;
}

void app::setup()
{
	//Init random number generator
	srand((unsigned int)time((time_t *)NULL));

	//GUI setup
	//First init
	gui.init_gui_thread();
	//Then start
	gui.start_form();

	//Init OSC
	init_osc_recv();

	//Get the most recent GUI info
	current_GUI_settings = gui.getInfo();
	//Apply the most recent settings
	applySettings(current_GUI_settings);

	//Init sound
	init_sound();
}

//Here is where you write into the audio buffer
void app::audioRequested(float * output, int bufferSize, int nChannels)
{
	//Collect list of pointers for each channel
	vector<float *> fl;
	vector<float *> fr;
	vector<float *> sl;
	vector<float *> sr;
	for (int i = 0; i < bufferSize; i++)
	{
		fl.push_back(&(output[nChannels*i]));
		fr.push_back(&(output[nChannels*i +1]));
		if(nChannels > 2)
		{
			sl.push_back(&(output[nChannels*i +2]));
			sr.push_back(&(output[nChannels*i +3]));
			//if(nChannels > 4)
			//Then do other channels here
		}
	}

	//Then call the appropriate buffer function for each audio effect
	int num_effects = audioEffects.size();
	for(int i = 0; i < num_effects; i++)
	{
		audioEffects[i]->flBufferRequested(&(fl));
		audioEffects[i]->frBufferRequested(&(fr));
		if(nChannels > 2)
		{
			audioEffects[i]->slBufferRequested(&(sl));
			audioEffects[i]->srBufferRequested(&(sr));
			//if(nChannels > 4)
			//Then do other channels here
		}
	}

	//Scale all audio by the global volume
	for (int i = 0; i < bufferSize*nChannels; i++)
	{
		output[i] *= global_volume;
	}
}

//--------------------------------------------------------------
void app::update()
{
	//Get the most recent GUI info
	current_GUI_settings = gui.getInfo();
	//Apply the most recent settings
	applySettings(current_GUI_settings);

	//Check for and process new osc messages
	process_osc_messages();
}

void app::applySettings(string settings_string)
{
	//Apply the settings received
	//Split on new line
	vector<string> settings = split(settings_string,'\n');
	//Parse each setting
	int num_settings = settings.size();
	for(int i = 0; i < num_settings; ++i)
	{
		applySettingString(settings[i]);
	}
}

bool app::generalSettings(string name, string value)
{
	//do_close variable
	if(name == app::Setting_Do_Close)
	{
		if(value == "1")
		{
			soft_exit();
		}
	}
	//Global volume
	else if(name == app::Setting_GlobalVolume)
	{
		global_volume = atof(value.c_str());
	}
	else
	{
		return false;
	}
	return true;
}

bool app::skeletonSettings(string name, string value)
{
	//Skeleton index
	if(name == app::Setting_SkeletonIndex)
	{
		current_gui_selected_skeleton = atoi(value.c_str());
	}
	else if(name == "KinectIndex:")
	{
		current_gui_selected_kinect = atoi(value.c_str());
	}
	//Right hand map
	else if(name == app::Setting_HandRightMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_HandRight,value);
	}
	else
	{
		return false;
	}
	return true;
}

void app::applySetting(string name, string value)
{
	//All setting checkers are functions
	//Return true if setting was set
	//Best practice to end setting checker with 
	//else
	//{
	//  	return false;
	//}
	//return true;

	if(generalSettings(name,value)) return;

	if(chirpSettings(name,value)) return;

	//Add more settings above here as else ifs
	else
	{
		cout << "Unrecognized setting from GUI form: " << name << ":" << value << endl;
	}
}

bool app::chirpSettings(string name, string value)
{
	float val = atof(value.c_str());
	vector<Chirp *> chirpys = getChirpsFromString(name);
	int num_chirps = chirpys.size();

	if(name.find("StartFreq") >= 0)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->freqStart = val;
		}
	}
	else if(name.find("EndFreq") >= 0)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->freqEnd = val;
		}
	}
	else if(name.find("Dur") >= 0)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->duration_in_ms = val;
		}
	}
	else if(name.find("Time") >= 0)
	{
		//AHH TODO!
	}
	else if(name.find("Per") >= 0)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->repeatPeriodMS = val;
		}
	}
	else
	{
		return false;
	}
	return true;
}

vector<Chirp *> app::getChirpsFromString(string str)
{
	//Get first two chars
	string id = str.substr(0,2);
	//Loop through all chirps, make list of chirps with that channel enabled
	vector<Chirp*> fl_chirps;
	vector<Chirp*> fr_chirps;
	vector<Chirp*> sr_chirps;
	vector<Chirp*> sl_chirps;
	int num_chirps = chirps.size();
	for(int i = 0; i <  num_chirps; i++)
	{
		if(chirps[i]->fl_enabled)
		{
			fl_chirps.push_back(chirps[i]);
		}
		else if(chirps[i]->fr_enabled)
		{
			fr_chirps.push_back(chirps[i]);
		}
		else if(chirps[i]->sr_enabled)
		{
			sr_chirps.push_back(chirps[i]);
		}
		else if(chirps[i]->sl_enabled)
		{
			sl_chirps.push_back(chirps[i]);
		}
	}

	if(id=="fl")
	{
		return fl_chirps;
	}
	else if(id=="fr")
	{
		return fr_chirps;
	}
	else if(id=="sr")
	{
		return sr_chirps;
	}
	else if(id=="sl")
	{
		return sl_chirps;
	}
	else
	{
		return vector<Chirp*>();
	}
}

void app::applySettingString(string setting)
{
	//Split on the ':' character
	vector<string> args = split(setting,':');

	int num_args = args.size();
	if(num_args < 2) return;

	applySetting(args[0],args[1]);
}

//Apply a mapping into a joint
void app::applyJointMapping(int skeletonIndex, int kinectIndex, string JointType, string mapping)
{
	//FIX ME TODO
	----
	//Make sure skeleton exists
	int num_skeletons = skeletons.size();
	if(num_skeletons==0) return;
	if( (skeletonIndex <= (num_skeletons-1)) && (skeletonIndex >= 0)  )
	{
		int num_bones = skeletons[skeletonIndex].bones.size();
		for(int i  = 0; i < num_bones; i++)
		{
			int num_joints = skeletons[skeletonIndex].bones[i].num_joints;
			for(int j = 0; j < num_joints; j++)
			{
				if(skeletons[skeletonIndex].bones[i].joints[j].type == JointType)
				{
					skeletons[skeletonIndex].bones[i].joints[j].mapping = mapping;
				}
			}
		}
		//TODO find better way than looping through bones, map joint to possible bones
	}
}

//--------------------------------------------------------------
void app::draw()
{
	//Draw the skeletons on screen
	//For now use the whole screen
	drawSkeletons(ofRectangle(0,0,(float)ofGetWindowWidth(),(float)ofGetWindowHeight()));
}

//--------------------------------------------------------------
void app::keyPressed(int key){

}

//--------------------------------------------------------------
void app::keyReleased(int key){

}

//--------------------------------------------------------------
void app::mouseMoved(int x, int y){

}

//--------------------------------------------------------------
void app::mouseDragged(int x, int y, int button){

}

//--------------------------------------------------------------
void app::mousePressed(int x, int y, int button){

}

//--------------------------------------------------------------
void app::mouseReleased(int x, int y, int button){

}

//--------------------------------------------------------------
void app::windowResized(int w, int h){

}

//--------------------------------------------------------------
void app::gotMessage(ofMessage msg){

}

//--------------------------------------------------------------
void app::dragEvent(ofDragInfo dragInfo){ 

}

//Splitting functions
std::vector<std::string> & app::split(const std::string &s, char delim, std::vector<std::string> &elems) {
    std::stringstream ss(s);
    std::string item;
    while(std::getline(ss, item, delim)) {
        elems.push_back(item);
    }
    return elems;
}
std::vector<std::string> app::split(const std::string &s, char delim) {
    std::vector<std::string> elems;
    return split(s, delim, elems);
}
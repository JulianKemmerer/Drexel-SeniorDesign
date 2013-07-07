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
	//if(bundle_index >= latest_bundle_index)
	//{
		//Keep track of most recent packet
		latest_bundle_index = bundle_index;

		//Strip information from the msg
		//If this message is a kinect skeleton message
		//"Address" instead of "method"? ...Alright
		if(msg->getAddress().find(SKELETON_METHOD) != string::npos)
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
	//}
	//else
	//{
		//cout << "Dropped OSC bundle: " << bundle_index << endl;
	//}
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

Skeleton * app::findSkeleton(int index, int src_kinect)
{
	//Loop through list of skeletons
	int num_skeletons = skeletons.size();
	for(int i = 0; i < num_skeletons; ++i)
	{
		//if index and src kinect match
		if((skeletons[i].idx == index) && (skeletons[i].src_kinect == src_kinect) )
		{
			return &(skeletons[i]);
		}
	}
	return NULL;
}

void app::special_skeleton_copy(Skeleton * to, Skeleton * from)
{
	//Copies only joint position information
	//Save mapping and audio effect!!!!!!!!!!!!!

	//Audio effects for skeleton...
	SingleWAVFileAudioPlayer * swfap = from->swfap;
	AudioEffect * ae = from->ae;
	AudioInput * ai = from->ai;

	//From is new (bad), to is old (good)

	//Loop through all joints in skeleton from
	Skeleton * s = from;
	int num_bones = s->bones.size();
	for(int k  = 0; k < num_bones; k++)
	{
		int num_joints = s->bones[k].num_joints;
		for(int j = 0; j < num_joints; j++)
		{
				from->bones[k].joints[j].mapping = to->bones[k].joints[j].mapping;
		}
	}
	from->swfap = to->swfap;
	from->ae = to->ae;
	from->ai = to->ai;

	//Do the copy
	*to = *from;
}

void app::addOrUpdateSkeletonsList(Skeleton * s)
{
	Skeleton * found = findSkeleton(s->idx,s->src_kinect);
	if(found)
	{
		special_skeleton_copy(found,s);
	}
	else
	{
		//not found
		//ONly accept index 0
		if(s->idx==0)
		{
			skeletons.push_back(*s);
		}
	}
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
	//3 ... TODO
	else
	{
		cout << "Unimplemented number of skeletons to draw..." << num_skeletons << endl;
		//Delete the third...
		for(int i=(num_skeletons-1); i>=2; i--)
		{
			skeletons.erase(skeletons.begin() + i);
		}
	}
	
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
	drawJoint(j, multiplier, constant,b,s);
	
	//Do this level's effect as seperate function
	doJointEffect(j, multiplier, constant,b,s);
}

void app::drawJoint(Joint* j, ofPoint multiplier, ofPoint constant, Bone * b, Skeleton * s)
{
	ofPoint realpt = ofPoint(j->point.x*multiplier.x + constant.x,j->point.y*multiplier.y + constant.y);

	//Draw joints as red for now
	ofSetColor(ofColor::red);
	float joint_radius = 5;
	ofCircle(realpt,joint_radius);

	drawJointText(j, multiplier,constant,realpt,b,s);
}

void app::drawJointText(Joint* j, ofPoint multiplier, ofPoint constant, ofPoint realpt, Bone * b, Skeleton * s)
{
	string to_draw = "";
	to_draw += "	Mapping: " + j->mapping + "\n";
	to_draw += "	Point: " + ofToString(j->point.x) + ", " + ofToString(j->point.y) + "\n";
	
	//If audio effect exists
	if(s->ae)
	{
		//Give information based on mapping
		if(j->mapping.find(Joint::Mapping_Pan) != string::npos)
		{
			to_draw += "	Front Left Pan: " + ofToString(s->ae->fl_pan) + "\n";
			to_draw += "	Front Right Pan: " + ofToString(s->ae->fr_pan) + "\n";
			to_draw += "	Surround Left Pan: " + ofToString(s->ae->sl_pan) + "\n";
			to_draw += "	Surround Right Pan: " + ofToString(s->ae->sr_pan) + "\n";
		}
		else if(j->mapping.find(Joint::Mapping_Vol) != string::npos)
		{
			to_draw += "	Front Left Volume: " + ofToString(s->ae->fl_vol) + "\n";
			to_draw += "	Front Right Volume: " + ofToString(s->ae->fr_vol) + "\n";
			to_draw += "	Surround Left Volume: " + ofToString(s->ae->sl_vol) + "\n";
			to_draw += "	Surround Right Volumen: " + ofToString(s->ae->sr_vol) + "\n";
		}
	}


	//Draw text near joint
	if(j->mapping != "")
	{
		ofDrawBitmapString(to_draw ,realpt);
	}

}

void app::doJointEffect(Joint* j, ofPoint multiplier, ofPoint constant,Bone * b, Skeleton * s)
{
	//cout << "Mapping: " << j->mapping << endl;
	//Look at the mapping this joint has
	if(j->mapping == Joint::Mapping_Global_Pan)
	{
		//Do pan effect
		doPanEffect(j->point.x,j->point.y);
	}
	//DO VOLUME EFFECT! TODO
	//Track volume and pan are only prefixes in the whole mapping
	//Check for those
	else if((j->mapping.find(Joint::Mapping_Pan) != string::npos) ||(j->mapping.find(Joint::Mapping_Vol) != string::npos) )
	{
		if(s->ae)
		{
			//Get file name
			string filename = getFileNameFromTrackMappingString(j->mapping,Joint::Mapping_Track_Vol_Prefix);
			//Use that to find all files associated with this 
			vector<SingleWAVFileAudioPlayer *> wavFilePlayers = findWAVFilePlayersFromTrackAndJoint(j,filename);
			//Do either volume of pan for these files
			if(j->mapping == "")
			{
				//Disabled mapping
				s->ae->setPan(0,0);
				s->ae->setVol(0);
			}
			else if(j->mapping.find(Joint::Mapping_Pan) != string::npos)
			{
				//DO pan
				float panX = getPanXFromJoint(j);
				float panY = getPanYFromJoint(j);
				//ONly to this skeletons track
				s->ae->setPan(panX,panY);

				return;
				int num_tracks  = wavFilePlayers.size();
				for(int i = 0; i < num_tracks; i++)
				{
				
				}
			}
			else if(j->mapping.find(Joint::Mapping_Vol) != string::npos)
			{
				//Do volume
				float vol = getVolFromJoint(j);

				s->ae->setVol(vol);	
			}
		}
	}
	else
	{
	}
}
float app::getVolFromJoint(Joint * j)
{
	//Realistically values go from -.5 + .5 for y
	//Vol is 0 to 1
	float vol = (j->point.y + 0.5);
	//cout << vol << endl;
	return vol;
}

float app::getPanXFromJoint(Joint * j)
{
	//Realistically values go from -1 to 1 for x
	return j->point.x*2;
}
float app::getPanYFromJoint(Joint * j)
{
	//Realistically values go from -.5 + .5 for y
	//Pan is -1 to 1
	return j->point.y*2;
}

string app::getFileNameFromTrackMappingString(string mapping, string prefix)
{
	//replace the prefix and the seperating char
	string to_find = prefix +"|";
	replaceAll(mapping, to_find,"");
	return mapping;
}

//Stack overflow stolen...
void app::replaceAll(std::string& str, const std::string& from, const std::string& to) {
    if(from.empty())
        return;
    size_t start_pos = 0;
    while((start_pos = str.find(from, start_pos)) != std::string::npos) {
        str.replace(start_pos, from.length(), to);
        start_pos += to.length(); // In case 'to' contains 'from', like replacing 'x' with 'yx'
    }
}

vector<SingleWAVFileAudioPlayer *> app::findWAVFilePlayersFromTrackAndJoint(Joint * j, string filename)
{
	vector<SingleWAVFileAudioPlayer *> rv;
	//Loop through all players for this joint and find those that match
	int num_players = j->wavFilePlayers.size();
	for(int i = 0; i < num_players; i++)
	{
		if( j->wavFilePlayers[i]->filename == filename )
		{
			rv.push_back(j->wavFilePlayers[i]);
		}
	}
	return rv;
}

void app::doPanEffect(float panX, float panY)
{
	//MOdifying the global volume levels
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
	numOutputChannels = 4;
	numInputChannels = 2;
	global_sampleRate = 44100;
	global_buffsize = 1024;
	numWindows = 1;

	//Input buffers per channel
	lin_buffer = new float[global_buffsize];
	rin_buffer = new float[global_buffsize];

	//Setup the sound stream
	pa_soundstream.listDevices();
	string to_find = "1800";
	int outputIndex = findOutputIndex("1800",numOutputChannels,numInputChannels);
	if(outputIndex == -1)
	{
		cout << "Could not find matching sound device." << endl;
		cout << "	Keyword: " << to_find << endl;
		cout <<	"	Min output channels: " << numOutputChannels << endl;
		cout <<	"	Min input channels: " << numInputChannels << endl;
		//Fuck it, Wait
		while(1)
		{
		}
	}
	pa_soundstream.setDeviceID(outputIndex);
	pa_soundstream.setup(this,numOutputChannels,numInputChannels,global_sampleRate,global_buffsize,numWindows);

	//Using sound
	cout << endl << endl << "==== Sound Device ====" << endl;
	const   PaDeviceInfo *deviceInfo;
	deviceInfo = Pa_GetDeviceInfo( outputIndex );
	cout << outputIndex << ": " << deviceInfo->name << endl;
	cout << "Low input latency: " << deviceInfo->defaultLowInputLatency << endl;
	cout << "Low output latency: " << deviceInfo->defaultLowOutputLatency << endl;
	cout << "api: " << deviceInfo->hostApi << endl;
	cout << "max in channels" << deviceInfo->maxInputChannels << endl;
	cout << "max out channels" << deviceInfo->maxOutputChannels << endl;
	cout << "default sample rate:" << deviceInfo->defaultSampleRate << endl;
	cout << "input channels: " << numInputChannels << endl;
	cout << "input channels: " << numOutputChannels << endl;
	cout << "sample rate: " << global_sampleRate << endl;
	cout << "buffer size: " << global_buffsize << endl;	

	//Volume init
	global_volume = 0.0;
	//Volumes
	fr_vol = 1; //Front right vol
	fl_vol = 1; 
	sr_vol = 1;
	sl_vol = 1;

	//Init asound input
	init_sound_input();
}


void app::init_sound_input()
{
	//Add two objects for sound input
	AudioInput * lin = &leftAudioInput;
	AudioInput * rin = &rightAudioInput;
	lin->channel="l";
	rin->channel="r";
	lin->lin_buffer = lin_buffer;
	rin->rin_buffer = rin_buffer;
	rin->buffsize = global_buffsize;
	lin->buffsize = global_buffsize;
	rin->samplerate = global_sampleRate;
	lin->samplerate = global_sampleRate;
	rin->setVol(1);
	lin->setVol(1);
	audioEffects.push_back(lin);
	audioEffects.push_back(rin);
	audioInputs.push_back(lin);
	audioInputs.push_back(rin);
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
		string name = string(deviceInfo->name);
		
		if(name.find("ASIO4") != string::npos)
		{
			return i;
		}
		
		/*
		if(deviceInfo->maxOutputChannels == numChannels)
		{
			return i;
		}
		*/
		
	}
	return -1;
}

int app::findOutputIndex(string to_find,int out, int in)
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
		string name = string(deviceInfo->name);
		
		if((name.find(to_find) != string::npos) &&(deviceInfo->maxInputChannels>=in) &&(deviceInfo->maxOutputChannels>=out))
		{
			return i;
		}
	}
	return -1;
}

SingleWAVFileAudioPlayer * app::init_add_start_wavfileplayer(string filename)
{
	SingleWAVFileAudioPlayer * swfap = new SingleWAVFileAudioPlayer();
	swfap->filename = filename;
	swfap->buffsize = global_buffsize;
	swfap->samplerate = global_sampleRate;
	swfap->fl_enabled = true;
	swfap->fr_enabled = true;
	swfap->sl_enabled = true;
	swfap->sr_enabled = true;
	swfap->setVol(1);
	swfap->init();
	//Add to global lists
	audioEffects.push_back(swfap);
	wavFilePlayers.push_back(swfap);
	return swfap;
}

void app::init_add_chirp(float dur_ms,float freq_start,float freq_end,float rep_period, float volume, float start_offset_ms, string speaker_id)
{
	Chirp * chirp = new Chirp();
	chirp->buffsize = global_buffsize;
	chirp->samplerate = global_sampleRate;
	chirp->duration_in_ms = dur_ms;
	chirp->freqStart = freq_start;
	chirp->freqEnd = freq_end;
	chirp->repeatPeriodMS = rep_period;
	chirp->setVol(volume);
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
	//chirp->startChirping();
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

void app::audioReceived(float * input, int bufferSize, int nChannels)
{		
	//Loop through incoming audio and split channels into buffers
	for (int i = 0; i < bufferSize; i++)
	{
		lin_buffer[i] = input[nChannels*i];
		rin_buffer[i] = input[nChannels*i +1];
	}
}

//Here is where you write into the audio buffer
void app::audioRequested(float * output, int bufferSize, int nChannels)
{
	/*
	//Loop through incoming audio and split channels into buffers
	for (int i = 0; i < bufferSize; i++)
	{
		output[nChannels*i] = lin_buffer[i];
		output[nChannels*i +1] = rin_buffer[i];
		output[nChannels*i + 2] = lin_buffer[i];
		output[nChannels*i + 3] = rin_buffer[i];
	}
	return;
	*/



	//Only collect a new list if ptr has changed
	if(output_buffer != output)
	{
		fl_samples.clear();
		fr_samples.clear();
		sl_samples.clear();
		sr_samples.clear();

		for (int i = 0; i < bufferSize; i++)
		{
			fl_samples.push_back(&(output[nChannels*i]));
			fr_samples.push_back(&(output[nChannels*i +1]));
			if(nChannels > 2)
			{
				sl_samples.push_back(&(output[nChannels*i +2]));
				sr_samples.push_back(&(output[nChannels*i +3]));
				//if(nChannels > 4)
				//Then do other channels here
			}
		}
	}

	//Then call the appropriate buffer function for each audio effect
	int num_effects = audioEffects.size();
	for(int i = 0; i < num_effects; i++)
	{
		audioEffects[i]->flBufferRequested(&(fl_samples));
		audioEffects[i]->frBufferRequested(&(fr_samples));
		if(nChannels > 2)
		{
			audioEffects[i]->slBufferRequested(&(sl_samples));
			audioEffects[i]->srBufferRequested(&(sr_samples));
			//if(nChannels > 4)
			//Then do other channels here
		}
	}

	//Scale all audio by the global volume
	for (int i = 0; i < bufferSize*nChannels; i++)
	{
		output[i] *= global_volume;
	}

	//Stoire ptr to output
	output_buffer = output;
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
	else if(name == "KinectIndex")
	{
		current_gui_selected_kinect = atoi(value.c_str());
	}
	else if(name == "SkeletonStem")
	{
		replaceAll(value,"C\\","C:\\");
		current_gui_selected_skeleton_stem = value;
		//Add this stem to the appropriate skeleton object
		Skeleton * s = findSkeleton(current_gui_selected_skeleton,current_gui_selected_kinect);
		if(s)
		{
			//Found a skeleton
			//Check if this is a wav file or a input track
			AudioEffect * ae = NULL;
			SingleWAVFileAudioPlayer * swfap = NULL;
			AudioInput * ai = NULL;

			if(value.find("Left Channel Audio Input") != string::npos)
			{
				//LEft channe
				//Store as both ptr types
				s->ai = &leftAudioInput;
				s->ae = s->ai;
			}
			else if(value.find("Right Channel Audio Input") != string::npos)
			{
				//Right channnnnneeeeeellL!!
				//Store as both ptr types
				s->ai = &rightAudioInput;
				s->ae = s->ai;
			}
			else
			{
				//It is a wav file
				swfap = init_add_start_wavfileplayer(current_gui_selected_skeleton_stem);
				s->swfap=swfap;
				s->ae = s->swfap;
			}
		}
	}
	//Right hand map
	else if(name == app::Setting_HandRightMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_HandRight,value);
	}
	//Left hand map
	else if(name == app::Setting_HandLeftMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_HandLeft,value);
	}
	//Right wrist map
	else if(name == app::Setting_WristRightMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_WristRight,value);
	}
	//LEft wrist map
	else if(name == app::Setting_WristLeftMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_WristLeft,value);
	}
	//Right elbow map
	else if(name == app::Setting_ElbowRightMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_ElbowRight,value);
	}
	//Left elbow map
	else if(name == app::Setting_ElbowLeftMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_ElbowLeft,value);
	}
	//Right shoulder map
	else if(name == app::Setting_ShoulderRightMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_ShoulderRight,value);
	}
	//Lft shoulder map
	else if(name == app::Setting_ShoulderLeftMap)
	{
		applyJointMapping(current_gui_selected_skeleton, current_gui_selected_kinect, Joint::Type_ShoulderLeft,value);
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

	if(skeletonSettings(name,value)) return;

	//Add more settings above here as else ifs
	else
	{
		cout << "Unrecognized setting from GUI form: " << name << ":" << value << endl;
	}
}

bool app::chirpSettings(string name, string value)
{
	//First check that the name of this setting starts with a speaker id
	//TODO fix this, do other settings have that as starting string?
	//Get first two chars
	string id = name.substr(0,2);
	if( !( (id=="fl") || (id=="sl") || (id=="fr") || (id=="sr")) )
	{
		return false;
	}
	
	float val = atof(value.c_str());
	vector<Chirp *> chirpys = getChirpsFromString(name);
	int num_chirps = chirpys.size();

	//If no chirps were returned we may need to add them since this might be the first time...
	if(num_chirps==0)
	{
		//Run the init function with b/s values just give identifying speaker
		init_add_chirp(0,0,0,0,1,0,id);
		//Re-get chirps
		chirpys = getChirpsFromString(name);
		num_chirps = chirpys.size();
	}

	if(name.find("StartFreq") != string::npos)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			//gui vals in Hz
			chirpys[i]->freqStart = val;
		}
	}
	else if(name.find("EndFreq") != string::npos)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			//gui vals in Hz
			chirpys[i]->freqEnd = val;
		}
	}
	else if(name.find("Dur") != string::npos)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->duration_in_ms = val;
		}
	}
	else if(name.find("Time") != string::npos)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->offset_ms = val;
		}
	}
	else if(name.find("Per") != string::npos)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->repeatPeriodMS = val;
		}
	}
	else if(name.find("Volume") != string::npos)
	{
		for(int i = 0; i < num_chirps; i++)
		{
			chirpys[i]->setVol(val);
		}
	}
	else
	{
		return false;
	}

	//Start chirping with new settings
	//ONly if setting was not volume
	if(name.find("Volume") != string::npos) return true;
	for(int i = 0; i < num_chirps; i++)
	{
		chirpys[i]->startChirping();
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
	Skeleton * s = findSkeleton(skeletonIndex,kinectIndex);
	if(s)
	{
		int num_bones = s->bones.size();
		for(int k  = 0; k < num_bones; k++)
		{
			int num_joints = s->bones[k].num_joints;
			for(int j = 0; j < num_joints; j++)
			{
				if(s->bones[k].joints[j].type == JointType)
				{
					//add or update audio effect if needed
					if(mapping == "Disabled")
					{
						s->bones[k].joints[j].mapping = "";
					}
					else
					{
						s->bones[k].joints[j].mapping = mapping;
					}
					
					//Reset pan and vol when mapping changed
					if(s->ae)
					{
						s->ae->setPan(0,0);
						s->ae->setVol(1);
					}
					//TODO
				}
			}
		}
	}
}

void app::addOrUpdateAudioEffect(Skeleton * s, Bone * b, Joint * j)
{
	//If the mapping is something that required an audio effect
	if((j->mapping.find(Joint::Mapping_Track_Pan_Prefix) != string::npos) ||(j->mapping.find(Joint::Mapping_Track_Vol_Prefix) != string::npos) )
	{
		//Get file name
		string filename = getFileNameFromTrackMappingString(j->mapping,Joint::Mapping_Track_Vol_Prefix);
		
		//Create a wav player for this
		//Add a test effect to the global list
		SingleWAVFileAudioPlayer * swfap = new SingleWAVFileAudioPlayer();
		swfap->filename = filename;
		swfap->buffsize = global_buffsize;
		swfap->samplerate = global_sampleRate;
		swfap->fl_enabled = true;
		swfap->fr_enabled = true;
		swfap->sl_enabled = true;
		swfap->sr_enabled = true;
		swfap->setVol(1);
		swfap->init();
		//Add to global lists
		audioEffects.push_back(swfap);
		wavFilePlayers.push_back(swfap);
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
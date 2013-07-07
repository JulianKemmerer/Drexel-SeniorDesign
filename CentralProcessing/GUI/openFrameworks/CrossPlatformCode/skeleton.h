#pragma once

#include "ofMain.h"
#include <vector>
#include <string>
//#include "audioeffect.h"

class SingleWAVFileAudioPlayer;
class AudioEffect;
class AudioInput;

class Joint
{
public:
	//Joint is a point and a 'type'
	ofPoint point;
	string type;
	//Also include mapping
	string mapping;

	//Joint can also have a list of tracks associated with it
	//Though only one mapping per joint for now...
	vector<SingleWAVFileAudioPlayer*> wavFilePlayers;

	//Types
	static const string Type_HandLeft;
	static const string Type_HandRight;
	static const string Type_WristLeft;
	static const string Type_WristRight;
	static const string Type_ElbowLeft;
	static const string Type_ElbowRight;
	static const string Type_ShoulderLeft;
	static const string Type_ShoulderRight;

	//Possible mappings for effects
	static const string Mapping_Global_Pan;
	static const string Mapping_Global_Vol;
	static const string Mapping_Track_Vol_Prefix ;
	static const string Mapping_Track_Pan_Prefix ;
	static const string Mapping_Pan;
	static const string Mapping_Vol;

	Joint::Joint()
	{
	}

	Joint::Joint(ofPoint p, string t)
	{
		Joint();
		point = p;
		type = t;
	}
};





class Bone
{
	//Bone is just two joints
public:
	Joint joints[2];
	static const int num_joints =2;

	Bone::Bone(Joint j1, Joint j2)
	{
		joints[0] = j1;
		joints[1] = j2;
	}
};

class Skeleton
{
public:
	//Skeleton is just a collection of bones, and index and source kinect is useful
	vector<Bone> bones;
	int src_kinect;
	int idx;
	//vector<AudioEffect*> audioEffects;
	//Just one for now
	//Have it in multiple forms?
	AudioEffect * ae;
	SingleWAVFileAudioPlayer * swfap;
	AudioInput * ai;

	Skeleton::Skeleton()
	{
		src_kinect = -1;
		idx = -1;
		ae = NULL;
		swfap = NULL;
		ai = NULL;
	}
};
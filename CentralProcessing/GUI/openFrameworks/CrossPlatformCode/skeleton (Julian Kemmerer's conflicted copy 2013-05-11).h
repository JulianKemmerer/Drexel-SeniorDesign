#pragma once

#include "ofMain.h"
#include <vector>
#include <string>

class Joint
{
public:
	//Joint is a point and a 'type'
	ofPoint point;
	string type;
	//Also include mapping
	string mapping;

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

	Skeleton::Skeleton()
	{
		src_kinect = -1;
		idx = -1;
	}
};
#include "app.h"

//C++ is dumb and I have initilize these string outside of the class file
//MUST ALSO ADD TO CLASS FILE <class>::<variable>
const string Joint::Type_HandLeft = "HandLeft";
const string Joint::Type_WristLeft = "WristLeft";
const string Joint::Type_ElbowLeft = "ElbowLeft";
const string Joint::Type_ElbowRight = "ElbowRight";
const string Joint::Type_WristRight = "WristRight";
const string Joint::Type_HandRight = "HandRight";
const string Joint::Type_ShoulderLeft = "ShoulderLeft";
const string Joint::Type_ShoulderRight = "ShoulderRight";

//Mappings for joints
const string Joint::Mapping_Global_Pan = "Global Pan";
const string Joint::Mapping_Global_Vol = "Global Volume";
const string Joint::Mapping_Track_Vol_Prefix = "Track Volume";
const string Joint::Mapping_Track_Pan_Prefix = "Track Pan";
const string Joint::Mapping_Pan = "Pan";
const string Joint::Mapping_Vol = "Volume";

//Give values to the constant setting strings
const string app::Setting_Do_Close = "do_close";
const string app::Setting_SkeletonIndex = "SkeletonIndex";
const string app::Setting_HandRightMap = "HandRightMap";
const string app::Setting_HandLeftMap = "HandLeftMap";
const string app::Setting_WristRightMap = "WristRightMap";
const string app::Setting_WristLeftMap = "WristLeftMap";
const string app::Setting_ElbowRightMap = "ElbowRightMap";
const string app::Setting_ElbowLeftMap = "ElbowLeftMap";
const string app::Setting_ShoulderRightMap = "ShoulderRightMap";
const string app::Setting_ShoulderLeftMap = "ShoulderLeftMap";

const string app::Setting_GlobalVolume = "GlobalVolume";
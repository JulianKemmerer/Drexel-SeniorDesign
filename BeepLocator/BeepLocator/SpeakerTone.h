//
//  SpeakerTones.h
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MatlabFunctions.h"

@interface SpeakerTone : NSObject {
    
    
    float totalSeconds;
    float *timeIndex;
    int numSamples;
    float frequency;
    float bellDecay;
    BOOL isBellTone;
    BOOL isGammatone;
    
    float gammaBandwidth;
    int gammaOrder;
    float gammaAmplitude;
}

@property float *signal;

-(id) initWithFrequency:(float)freq length:(float)length sampleRate:(int)fs;

-(void) applyBellTone:(float)decayRate;

-(void) applyGammatone:(int)order bandwidth:(float)bw amplitude:(float)amp;

@end

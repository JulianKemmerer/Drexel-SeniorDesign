//
//  SpeakerTones.m
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import "SpeakerTone.h"

@implementation SpeakerTone

@synthesize signal;


-(id) initWithFrequency:(float)freq length:(float)seconds sampleRate:(int)fs {
    
    self = [super init];
    if (self) {
        isBellTone = NO;
        isGammatone = NO;
        totalSeconds = seconds;
        numSamples = (int)roundf(totalSeconds*fs);
        signal = (float*)calloc(numSamples, sizeof(float));
        timeIndex = (float*)calloc(numSamples, sizeof(float));
        frequency = freq;
        [MatlabFunctions linspace:0 max:seconds numElements:numSamples array:timeIndex];
        
        for (int i = 0;i<numSamples;i++) {
            signal[i] = sinf(2.0*M_PI*frequency*timeIndex[i]);
        }
        
    }
    return self;
}

-(void) applyBellTone:(float)decayRate {
    bellDecay = decayRate;
    for (int i = 0;i<numSamples;i++) {
        signal[i] *= expf(-timeIndex[i]/decayRate);
    }
}

-(void) applyGammatone:(int)order bandwidth:(float)bw amplitude:(float)amp {
    
    gammaBandwidth = bw;
    gammaOrder = order;
    gammaAmplitude = amp;
    for (int i = 0;i<numSamples;i++) {
        signal[i] *= (gammaAmplitude*powf(timeIndex[i],(float)(gammaOrder-1))*expf(-2.0*M_PI*gammaBandwidth*timeIndex[i]));
    }
}


@end

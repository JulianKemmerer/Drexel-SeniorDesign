//
//  SimpleFFT.h
//  AudioListener
//
//  Created by Matthew Prockup on 4/11/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#include <Accelerate/Accelerate.h>

@interface SimpleFFT : NSObject
{
    int					fftSize, 
    fftSizeOver2,
    log2n,
    log2nOver2,
    windowSize,
    bufferSize;



    float				*in_real, 
    *out_real,
    *window;
    
	float scale;
//    FFTSetup fftSetup;
    COMPLEX_SPLIT split_data;
    
    BOOL isRunning;


}

@property FFTSetup fftSetup;

-(int)fftSetSize:(int)size;
-(BOOL)forwardWithStart:(int)start withBuffer:(float*)buffer magnitude:(float*)magnitude phase:(float*)phase useWinsow:(bool)doWindow;
-(void)inverseWithStart:(int)start withBuffer:(float*)buffer magnitude:(float*)magnitude phase:(float*)phase useWinsow:(bool)doWindow;

@end

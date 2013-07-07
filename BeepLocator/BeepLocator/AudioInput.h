//
//  AudioInput.h
//  RealAcoustics
//
//  Created by Matthew Zimmerman on 6/21/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AudioToolBox/AudioToolbox.h>
#import "AudioParameters.h"

@protocol AudioInputDelegate <NSObject>

@required

-(void) processInputBuffer:(float*)buffer numSamples:(int)numSamples;

@end


@interface AudioInput : NSObject {
    
    @public
    int counter;
    float *audioBuffer;
    
    @private
	AudioStreamBasicDescription dataFormat;
	AudioQueueRef queue;
	AudioQueueBufferRef buffers[kInputNumBuffers];
	BOOL isRunning;
	int frameNumber;
	id <AudioInputDelegate> delegate;
}

+ (AudioInput *)defaultRecorder;

-(id) initWithDelegate:(id)inputDelegate;

- (BOOL)start;
- (BOOL)stop;

@property (readonly) AudioQueueRef queue;
@property (readonly) BOOL isRunning;
@property int frameNumber;
@property id <AudioInputDelegate> delegate;
@end
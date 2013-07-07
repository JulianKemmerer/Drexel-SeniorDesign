//
//  AudioInput.m
//  RealAcoustics
//
//  Created by Matthew Zimmerman on 6/21/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "AudioInput.h"

@interface AudioInput (PrivateMethods)

- (void)setupAudioFormat;

@end


@implementation AudioInput

@synthesize queue;
@synthesize isRunning;
@synthesize delegate;
@synthesize frameNumber;

static AudioInput *recorder = nil;

static void AudioInputCallback(void *inUserData, AudioQueueRef inAQ, AudioQueueBufferRef inBuffer, 
							   const AudioTimeStamp *inStartTime, UInt32 inNumberPacketDescriptions,
							   const AudioStreamPacketDescription *inPacketDescs) {
	OSStatus err = 0;
    @autoreleasepool {
        SInt16 *buffer = inBuffer->mAudioData;
        int numElements = inBuffer->mAudioDataBytesCapacity/kInputDataByteSize;
        float *audioBuffer = (float*)malloc(sizeof(float)*numElements);
        float normalizer = powf(2,kInputDataByteSize*8-1);
        for (int i = 0;i<numElements;i++) {
            audioBuffer[i] = buffer[i]/normalizer;
        }
        [[recorder delegate] processInputBuffer:audioBuffer numSamples:numElements];
        free(audioBuffer);
        err = AudioQueueEnqueueBuffer(inAQ, inBuffer, 0, NULL);
    }
	if (err)
		NSLog(@"AudioInput: Failed to enqueue buffer: %ld", err); 
}


+ (AudioInput *)defaultRecorder {
    
	if (!recorder)
		recorder = [[AudioInput alloc] init];
	return recorder;
}

-(id) initWithDelegate:(id)inputDelegate {
    
    self = [AudioInput defaultRecorder];
    [self setDelegate:inputDelegate];
    return self;
}


- (id) init {
	OSStatus err = 0;
	frameNumber = 0;
	if (self = [super init]) {
		[self setupAudioFormat];
		
		err = AudioQueueNewInput(&dataFormat, AudioInputCallback, NULL, NULL, NULL, 0, &queue);
		NSAssert1(!err, @"Failed to create input queue for recording: %ld", err);
        
		for(int i = 0; i < kInputNumBuffers; i++) {
			err = AudioQueueAllocateBuffer(queue, kInputNumSamples*kInputDataByteSize, &buffers[i]);
			NSAssert1(!err, @"Failed to allocate buffer: %ld", err);
			err = AudioQueueEnqueueBuffer(queue, buffers[i], 0, NULL);
			NSAssert1(!err, @"Failed to enqueue buffer: %ld", err);
		}
	}
	
	return self;
}

- (BOOL)start {
	BOOL result = NO;
    //    NSLog(@"trying to start");
	
	OSStatus err = AudioQueueStart(queue, NULL);
	if (err) {
		NSLog(@"AudioInput: Failed to start audio recording: %ld", err);
		result = NO;
	} else {
		isRunning = YES;
		result = YES;
	}
	
	return result;
}

- (BOOL)stop {
    
	BOOL result = NO;
	OSStatus err = AudioQueuePause(queue);
	if (err) {
		NSLog(@"AudioInput: Failed to stop audio recording: %ld", err);
		result = NO;
	} else {
		err = AudioQueueFlush(queue);
		if (err) 
			NSLog(@"AudioInput: Failed to flush buffers: %ld", err); 
		isRunning = NO;
		result = YES;
	}
	
	return result;
}


- (void)setupAudioFormat {
	memset(&dataFormat, 0, sizeof(dataFormat));
	dataFormat.mSampleRate = kInputSampleRate;
	dataFormat.mFormatID = kAudioFormatLinearPCM;
	dataFormat.mFramesPerPacket = 1;
	dataFormat.mChannelsPerFrame = 1;
	dataFormat.mBitsPerChannel = kInputDataByteSize*8;
	dataFormat.mReserved = 0;
    dataFormat.mBytesPerPacket = dataFormat.mBytesPerFrame = kInputDataByteSize * dataFormat.mChannelsPerFrame;
	dataFormat.mFormatFlags = 
    kLinearPCMFormatFlagIsSignedInteger |
    kLinearPCMFormatFlagIsPacked;
}

@end


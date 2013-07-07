//
//  AudioOutput.m
//  RealAcoustics
//
//  Created by Matthew Zimmerman on 6/21/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "AudioOutput.h"

OSStatus RenderTone(
                    void *inRefCon, 
                    AudioUnitRenderActionFlags 	*ioActionFlags, 
                    const AudioTimeStamp 		*inTimeStamp, 
                    UInt32 						inBusNumber, 
                    UInt32 						inNumberFrames, 
                    AudioBufferList 			*ioData)

{
	// Get the tone parameters out of the view controller
	AudioOutput *output =  (__bridge AudioOutput *)inRefCon;
    
    // Becuase the iphone speaker is mono, we need only to consider one buffer
	Float32 *buffer = (Float32 *)ioData->mBuffers[kOutputBufferChannel].mData;
    //  Calling out to the delegate to fill the buffere with a specific buffer length
    [output.delegate AudioDataToOutput:buffer bufferLength:inNumberFrames];
	return noErr;
}

void ToneInterruptionListener(void *inClientData, UInt32 inInterruptionState)
{
    //  here in case there is an interupption in the middle of generation, such as a phone call or something like that.
	AudioOutput *output = (__bridge AudioOutput *)inClientData;
	[output stopOutput];
}

@implementation AudioOutput
@synthesize delegate;


-(void) toggleOutput {
    
//    turn off if playing and vice versa
    if (isPlaying) {
        [self stopOutput];
    } else {
        [self startOutput];
    }
}

-(void) stopOutput {
    
    // stop audio output if it is playing
    if (isPlaying) {
        // Kill and remove the output audio unit when stopping to clear memory usage
        AudioOutputUnitStop(toneUnit);
        AudioUnitUninitialize(toneUnit);
        AudioComponentInstanceDispose(toneUnit);
        toneUnit = nil;
        isPlaying = false;
    }
}

-(void) startOutput {
    
    // start audio output if not already playing
    if (!isPlaying) {
        // Stop changing parameters on the unit
        [self setupAudioOutput];
        OSErr err = AudioUnitInitialize(toneUnit);
        NSAssert1(err == noErr, @"Error initializing unit: %hd", err);
        // Start playback
        err = AudioOutputUnitStart(toneUnit);
        NSAssert1(err == noErr, @"Error starting unit: %hd", err);
        isPlaying = YES;
    }
}

-(void) setupAudioOutput {
    
    // Configure the search parameters to find the default playback output unit
	// (called the kAudioUnitSubType_RemoteIO on iOS but
	// kAudioUnitSubType_DefaultOutput on Mac OS X)
	AudioComponentDescription defaultOutputDescription;
	defaultOutputDescription.componentType = kAudioUnitType_Output;
	defaultOutputDescription.componentSubType = kAudioUnitSubType_RemoteIO;
	defaultOutputDescription.componentManufacturer = kAudioUnitManufacturer_Apple;
	defaultOutputDescription.componentFlags = 0;
	defaultOutputDescription.componentFlagsMask = 0;
	
	// Get the default playback output unit
	AudioComponent defaultOutput = AudioComponentFindNext(NULL, &defaultOutputDescription);
	NSAssert(defaultOutput, @"Can't find default output");
	
	// Create a new unit based on this that we'll use for output
	OSErr err = AudioComponentInstanceNew(defaultOutput, &toneUnit);
	NSAssert1(toneUnit, @"Error creating unit: %hd", err);
	
	// Set our tone rendering function on the unit
	AURenderCallbackStruct input;
	input.inputProc = RenderTone;
	input.inputProcRefCon = (__bridge void*) self;
	err = AudioUnitSetProperty(toneUnit, 
                               kAudioUnitProperty_SetRenderCallback, 
                               kAudioUnitScope_Input,
                               0, 
                               &input, 
                               sizeof(input));
	NSAssert1(err == noErr, @"Error setting callback: %hd", err);
	
	// Set the format to 32 bit, single channel, floating point, linear PCM
	const int four_bytes_per_float = 4;
	const int eight_bits_per_byte = 8;
	AudioStreamBasicDescription streamFormat;
	streamFormat.mSampleRate = kOutputSampleRate;
	streamFormat.mFormatID = kAudioFormatLinearPCM;
	streamFormat.mFormatFlags =
    kAudioFormatFlagsNativeFloatPacked | kAudioFormatFlagIsNonInterleaved;
	streamFormat.mBytesPerPacket = four_bytes_per_float;
	streamFormat.mFramesPerPacket = 1;	
	streamFormat.mBytesPerFrame = four_bytes_per_float;		
	streamFormat.mChannelsPerFrame = kOutputNumChannels;	
	streamFormat.mBitsPerChannel = four_bytes_per_float * eight_bits_per_byte;
	err = AudioUnitSetProperty (toneUnit,
                                kAudioUnitProperty_StreamFormat,
                                kAudioUnitScope_Input,
                                0,
                                &streamFormat,
                                sizeof(AudioStreamBasicDescription));
	NSAssert1(err == noErr, @"Error setting stream format: %hd", err);
}

-(id) initWithDelegate:(id)audioDelegate {
    self = [super init];
    isPlaying = NO;
    [self setDelegate:audioDelegate];
    frequency = 440;
	OSStatus result = AudioSessionInitialize(NULL, NULL, ToneInterruptionListener, (__bridge void*)self);
	if (result == kAudioSessionNoError)
	{
		UInt32 sessionCategory = kAudioSessionCategory_MediaPlayback;
		AudioSessionSetProperty(kAudioSessionProperty_AudioCategory, sizeof(sessionCategory), &sessionCategory);
	}
	AudioSessionSetActive(true);
    return self;
}
@end

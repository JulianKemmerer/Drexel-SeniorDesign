//
//  AudioParameters.h
//  RealAcoustics
//
//  Created by Matthew Zimmerman on 6/21/12.
//  Copyright (c) 2012 Drexel University. All rights reserved.
//


// Audio Output Parameters
#define kOutputSampleRate 44100 // 44.1kHz Sampling Rate will give output buffer of 1024 samples
#define kOutputBufferSize 1024 // currently does nothing, just a place holder
#define kOutputBufferChannel 0 // If iDevice, should be 0 b/c mic is mono
#define kOutputNumChannels 1 // setting output to be mono

// Audio Input Parameters
#define kInputSampleRate 44100 // Sampling rate for microphone input
#define kInputNumSamples 512 // Number of desired samples in buffer
#define kInputFFTSize 512 // Setting FFT Size
#define kInputNumBuffers 1 // Only need 1 input buffer
#define kInputDataByteSize 2 // data type is SInt16, so requires 2 bytes
#define kInputNumChannels 1
#define kInputNumFramesPerPacket 1

#define kOutputBusNumber 0
#define kInputBusNumber 1

#define kNumBitsPerByte 8
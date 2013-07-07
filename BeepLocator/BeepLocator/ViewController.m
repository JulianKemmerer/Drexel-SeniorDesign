//
//  ViewController.m
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import "ViewController.h"

@interface ViewController ()

@end

@implementation ViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    inAudio = [[AudioInput alloc] initWithDelegate:self];
    fft = [[SimpleFFT alloc] init];
    [fft fftSetSize:kInputFFTSize];
    audioRunning = NO;
    sigFun = [[SignalFunctions alloc] init];
    
    magBuffer = (float*)calloc(roundf(kInputFFTSize/2.0), sizeof(float));
    phaseBuffer = (float*)calloc(roundf(kInputFFTSize/2.0), sizeof(float));
    SpeakerTone *speakTemp;
    
    speakArray = [[NSMutableArray alloc] init];
    freqList = (float*)calloc(roundf(kInputFFTSize/2.0), sizeof(float));
    [MatlabFunctions linspace:0 max:kInputSampleRate/2.0 numElements:roundf(kInputFFTSize/2.0) array:freqList];
    
    float baseFreq = 440;
    float toneLength = 1;
    for (int i = 0;i<4;i++) {
        speakTemp = [[SpeakerTone alloc] initWithFrequency:baseFreq*powf(2.0,i) length:toneLength sampleRate:kInputSampleRate];
        [speakTemp applyBellTone:0.1];
        [speakArray addObject:speakTemp];
    }
}

-(void) processInputBuffer:(float *)buffer numSamples:(int)numSamples {
    [fft forwardWithStart:0 withBuffer:buffer magnitude:magBuffer phase:phaseBuffer useWinsow:YES];
    float power = [SignalFunctions getSignalPower:buffer start:0 stop:roundf(kInputFFTSize/2.0)];
    NSLog(@"%f",power);
    
}

-(IBAction)startPressed:(id)sender {
    
    if (!audioRunning) {
        [inAudio start];
        audioRunning = YES;
    }
    
}

-(IBAction)stopPressed:(id)sender {
    
    if (audioRunning) {
        [inAudio stop];
        audioRunning = NO;
    }
    
}

-(IBAction)printPressed:(id)sender {
    
    for (int i = 0;i<roundf(kInputFFTSize/2.0);i++) {
        printf("%f ",magBuffer[i]);
    }
    
}

@end

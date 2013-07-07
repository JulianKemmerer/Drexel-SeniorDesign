//
//  ViewController.h
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "AudioParameters.h"
#import "AudioInput.h"
#import "SimpleFFT.h"
#import "SignalFunctions.h"
#import "SpeakerTone.h"
#import "VectorFunctions.h"
@interface ViewController : UIViewController <AudioInputDelegate> {
    
    AudioInput *inAudio;
    SimpleFFT *fft;
    float *magBuffer;
    float *phaseBuffer;
    BOOL audioRunning;
    
    float *freqList;
    SignalFunctions *sigFun;
    
    NSMutableArray *speakArray;
    
    
}



-(IBAction)startPressed:(id)sender;

-(IBAction)stopPressed:(id)sender;

-(IBAction)printPressed:(id)sender;

@end

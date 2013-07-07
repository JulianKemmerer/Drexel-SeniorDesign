//
//  AudioOutput.h
//  RealAcoustics
//
//  Created by Matthew Zimmerman on 6/21/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AudioUnit/AudioUnit.h>
#import <AudioToolbox/AudioToolbox.h>
#import "AudioParameters.h"

@protocol AudioOutputDelegate;

@interface AudioOutput : NSObject {
    
    AudioComponentInstance toneUnit;
    id <AudioOutputDelegate> delegate;
    BOOL isPlaying;
    @public
    double frequency;
    double theta;
}

@property (nonatomic, retain) id <AudioOutputDelegate> delegate;

-(void) startOutput;

-(void) stopOutput;

-(void) toggleOutput;

-(void) setupAudioOutput;

-(id) initWithDelegate:(id)audioDelegate;

@end

@protocol AudioOutputDelegate <NSObject>

@required

-(void) AudioDataToOutput:(float*)buffer bufferLength:(int)bufferSize;

@end

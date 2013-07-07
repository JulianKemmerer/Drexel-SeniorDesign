//
//  AudioFunctions.m
//  AudioAnalysis
//
//  Created by Matthew Zimmerman on 6/23/12.
//  Copyright (c) 2012 Drexel University. All rights reserved.
//

#import "AudioFunctions.h"

@implementation AudioFunctions 

+(float*) addImaginaryPart:(float *)array arraySize:(int)size {
    float *audioArray = (float*)malloc(sizeof(float)*size*2);
    int i,j = 0;
    for (i=0;i<size;i++) {
        audioArray[j] = audioArray[i];
        audioArray[j+1] = 0;
        j+=2;
    }
    return audioArray;
}

@end

//
//  CircularBuffer.m
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import "CircularBuffer.h"

@implementation CircularBuffer


-(id) initWithSize:(int)buffSize {
    
    self = [super init];
    if (self) {
        bufferSize = buffSize;
        buffer = (float*)calloc(bufferSize, sizeof(float));
    }
    return self;
}

-(void) pushOntoBuffer:(float *)pushBuffer pushSize:(int)size {
    int count = 0;
    for (int i = size;i<bufferSize;i++) {
        buffer[count] = buffer[i];
        count++;
    }
    count = 0;
    for (int i = bufferSize-size;i<bufferSize;i++) {
        buffer[i] = pushBuffer[count];
    }
    count++;
}

-(float*) getBuffer {
    return buffer;
}

-(void) clearBuffer {
    free(buffer);
    buffer = (float*)calloc(bufferSize, sizeof(float));
}

@end

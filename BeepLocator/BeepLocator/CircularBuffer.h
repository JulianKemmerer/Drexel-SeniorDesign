//
//  CircularBuffer.h
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface CircularBuffer : NSObject {
    
    int bufferSize;
    float *buffer;
    
}

-(id) initWithSize:(int)buffSize;

-(void) pushOntoBuffer:(float*)pushBuffer pushSize:(int)size;

-(float*) getBuffer;

-(void) clearBuffer;

@end

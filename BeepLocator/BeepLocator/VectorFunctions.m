//
//  VectorFunctions.m
//  AudioLocator
//
//  Created by Matthew Zimmerman on 11/23/12.
//  Copyright (c) 2012 Matthew Zimmerman. All rights reserved.
//

#import "VectorFunctions.h"
#import <Accelerate/Accelerate.h>

@implementation VectorFunctions

+(void) absVector:(float *)vector numElements:(int)size output:(float*)outVector {
    vDSP_vabs(vector, 1, outVector, 1, size);
}

+(void) diffVector:(float *)vector numElements:(int)size output:(float*)outVector {
    for (int i = 0;i<size-1;i++) {
        outVector[i] = vector[i+1]-vector[i];
    }
}

+(void) squareVector:(float *)vector numElements:(int)size output:(float*)outVector {
    vDSP_vsq (vector,1,outVector,1,size);
}


+(void) vectorMax:(float*)vector numElements:(int)size output:(float*)outValue maxIndex:(unsigned long*)index {
    vDSP_maxvi(vector, 1, outValue, index, size);
}


@end

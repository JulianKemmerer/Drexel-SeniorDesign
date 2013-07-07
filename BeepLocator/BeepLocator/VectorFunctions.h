//
//  VectorFunctions.h
//  AudioLocator
//
//  Created by Matthew Zimmerman on 11/23/12.
//  Copyright (c) 2012 Matthew Zimmerman. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface VectorFunctions : NSObject

+(void) squareVector:(float*)vector numElements:(int)size output:(float*)outVector;

+(void) diffVector:(float*)vector numElements:(int)size output:(float*)outVector;

+(void) absVector:(float*)vector numElements:(int)size output:(float*)outVector;

+(void) vectorMax:(float*)vector numElements:(int)size output:(float*)outValue maxIndex:(unsigned long*)index;

@end

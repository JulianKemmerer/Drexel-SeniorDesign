//
//  SignalFunctions.h
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Accelerate/Accelerate.h>

@interface SignalFunctions : NSObject


+(float) getSignalPower:(float*)buffer start:(int)start stop:(int)stop;

+(void) crossCorrelateSignal:(float *)signal signalSize:(int)sigSize withFilter:(float *)filter filterSize:(int)filtSize output:(float *)outsignal outputSize:(int)outSize;

@end

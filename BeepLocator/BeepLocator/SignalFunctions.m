//
//  SignalFunctions.m
//  BeepLocator
//
//  Created by Matthew Zimmerman on 3/6/13.
//  Copyright (c) 2013 Drexel University. All rights reserved.
//

#import "SignalFunctions.h"

@implementation SignalFunctions

+(float) getSignalPower:(float *)buffer start:(int)start stop:(int)stop {
    float power = 0;
    for (int i = start;i<stop;i++) {
        power += powf(buffer[i], 2.0);
    }
    return power;
}

+(void) crossCorrelateSignal:(float *)signal signalSize:(int)sigSize withFilter:(float *)filter filterSize:(int)filtSize output:(float *)outsignal outputSize:(int)outSize {
    vDSP_conv(signal, 1, filter, 1, outsignal, 1, outSize, filtSize);
}

@end

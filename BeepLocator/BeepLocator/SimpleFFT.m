//
//  SimpleFFT.m
//  AudioListener
//
//  Created by Matthew Prockup on 4/11/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "SimpleFFT.h"

int nextPow2(int count) {
    return (int)powf(2,ceilf(log2f((float)count)));
}

@implementation SimpleFFT

@synthesize fftSetup;
-(int)fftSetSize:(int)size
{
    
    windowSize = size;
    int calcFFTSize = nextPow2(size);
    free(window);
    window = (float *) calloc(windowSize, sizeof(float));
    vDSP_hann_window(window, windowSize, vDSP_HANN_NORM);
    if (calcFFTSize != fftSize) {
        fftSize = calcFFTSize;					// sample size
        fftSizeOver2 = fftSize/2;		
        log2n = log2f(fftSize);			// bins
        log2nOver2 = log2n/2;
        
        free(in_real);
        free(out_real);
        free(split_data.imagp);
        free(split_data.realp);
        in_real = (float *) calloc(fftSize, sizeof(float));
        out_real = (float *) calloc(fftSize, sizeof(float));
        split_data.realp = (float *) calloc(fftSizeOver2, sizeof(float));
        split_data.imagp = (float *) calloc(fftSizeOver2, sizeof(float));
        
    //
        scale = 1.0f/(float)(4.0f*fftSize);
        
        // allocate the fft object once
        if (fftSetup) {
            vDSP_destroy_fftsetup(fftSetup);
        }
        fftSetup = vDSP_create_fftsetup(log2n, FFT_RADIX2);
//        NSLog(@"%i -> %i",size,fftSize);
    }
    return fftSize;
}
-(BOOL)forwardWithStart:(int)start withBuffer:(float*)buffer magnitude:(float*)magnitude phase:(float*)phase useWinsow:(bool)doWindow
{
    

    
    if (doWindow) {
        //multiply by window
        vDSP_vmul(buffer, 1, window, 1, in_real, 1, fftSize);
    }
    else {
        cblas_scopy(fftSize, buffer, 1, in_real, 1);
    }
    
    //convert to split complex format with evens in real and odds in imag
    vDSP_ctoz((COMPLEX *) in_real, 2, &split_data, 1, fftSizeOver2);
    
    //calc fft
    vDSP_fft_zrip(fftSetup, &split_data, 1, log2n, FFT_FORWARD);
    
    split_data.imagp[0] = 0.0;
    
    /*
     for (i = 0; i < fftSizeOver2; i++) 
     {
     //compute power 
     float power = split_data.realp[i]*split_data.realp[i] + 
     split_data.imagp[i]*split_data.imagp[i];
     
     //compute magnitude and phase
     magnitude[i] = sqrtf(power);
     phase[i] = atan2f(split_data.imagp[i], split_data.realp[i]);
     }*/
    
    vDSP_ztoc(&split_data, 1, (COMPLEX *) in_real, 2, fftSizeOver2);
    vDSP_polar(in_real, 2, out_real, 2, fftSizeOver2);
    cblas_scopy(fftSizeOver2, out_real, 2, magnitude, 1);
    cblas_scopy(fftSizeOver2, out_real+1, 2, phase, 1);
    
    return YES;
}

-(void)inverseWithStart:(int)start withBuffer:(float*)buffer magnitude:(float*)magnitude phase:(float*)phase useWinsow:(bool)doWindow
{
    /*
     float	*real_p = split_data.realp, 
     *imag_p = split_data.imagp;
     for (i = 0; i < fftSizeOver2; i++) {
     *real_p++ = magnitude[i] * cosf(phase[i]);
     *imag_p++ = magnitude[i] * sinf(phase[i]);
     }
     */
    
    cblas_scopy(fftSizeOver2, magnitude, 1, in_real, 2);
    cblas_scopy(fftSizeOver2, phase, 1, in_real+1, 2);
    vDSP_rect(in_real, 2, out_real, 2, fftSizeOver2);
    
    //convert to split complex format with evens in real and odds in imag
    vDSP_ctoz((COMPLEX *) out_real, 2, &split_data, 1, fftSizeOver2);
    
    vDSP_fft_zrip(fftSetup, &split_data, 1, log2n, FFT_INVERSE);
    vDSP_ztoc(&split_data, 1, (COMPLEX*) out_real, 2, fftSizeOver2);
    
    vDSP_vsmul(out_real, 1, &scale, out_real, 1, fftSize);
    
    // multiply by window w/ overlap-add
    if (doWindow) {
        float *p = buffer + start;
        for (int i = 0; i < fftSize; i++) {
            *p++ += out_real[i] * window[i];
        }
    }
    else {
        cblas_scopy(fftSize, out_real, 1, buffer+start, 1);
    }
    
}



@end

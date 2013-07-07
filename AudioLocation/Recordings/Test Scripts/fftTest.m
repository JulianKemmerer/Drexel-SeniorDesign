clc; close all; clear all;

signal = wavread('A21.wav');
NFFT = 1024;
step = 1024;
start = 1;
f = 440*2.^(0:3);
fs = 44100;
freqs = linspace(0,fs/2,NFFT/2+1);
% soundsc(signal,fs)
while start<(length(signal)-NFFT)
    
    chunk = signal(start:start+NFFT);
    Y = abs(fft(chunk,NFFT)).^2;
    plot(freqs,Y(1:NFFT/2+1));
    waitforbuttonpress
    start = start+step;
end


clc; close all; clear all;

totalTime = 1;
f = 880;
T = 0.5;
fs = 44100;
tau = 0.05;
t = linspace(0,T,fs*T);
y = sin(2*pi*f*t);
bell = exp(-t/tau);
y = y.*bell;
padding = zeros(1,1500);
% y = [padding,y];
% plot(y)
y = wavread('A1.wav');

start = 13000;
step  = 64;
NFFT = 512;

hamWin = hamming(NFFT);
freqs = linspace(0,NFFT/2,NFFT/2+1);

while start < length(y)-NFFT
    subplot(3,1,1)
    chunk = y(start:start+NFFT-1).*(hamWin.^2);
    plot(chunk)
    Y = fft(chunk,NFFT).^2;
    mag = abs(Y);
    phase = unwrap(angle(Y)); 
    subplot(3,1,2)
    plot(freqs,mag(1:NFFT/2+1))
    subplot(3,1,3)
    plot(freqs,phase(1:NFFT/2+1))
    waitforbuttonpress
    start = start+step;
    start
end
    

function out = phaseModulation(M1,M2,fs,dur)
%PHASEMODULATION Summary of this function goes here
%   Creates Chirp using phase variation

if nargin < 3, fs = 16000; end
if nargin < 4, dur = 2; end


t = [0:dur*fs]/fs;			% time of samples (sec)
t_m = t * 1000;				% time of samples (msec)
A = 1;


M = linspace(500,1000,length(t));
g1 = cos(2*pi*M.*t);
signal = A* cos(2*pi*1*t + g1);

plot(t_m,x_d1)
str = sprintf('Phase Modulation: Sweep from M= %.1f to M = %.1f',M1,M2);
title(str);
axis([0 40 -1 1])


out  = signal;
end






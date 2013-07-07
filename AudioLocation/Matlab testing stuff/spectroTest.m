clc; close all; clear all;
f = 440;
fs = 44100;
T = 1.5;
t = linspace(0,T,fs*T);
tone = sin(2*pi*f*linspace(0,1,fs));
bell = exp(-linspace(0,1,length(tone))/0.1);
tone = tone.*bell;
plot(tone)

start = randi([0,length(t)-length(tone)]);
sound = rand([1,length(t)])/1000;
sound(start:start+fs-1) = tone;
% figure
tic
[S,F,T,P] = spectrogram(sound,2048,2048-32,2048,fs,'yaxis'); 
toc
% soundsc(sound,fs);


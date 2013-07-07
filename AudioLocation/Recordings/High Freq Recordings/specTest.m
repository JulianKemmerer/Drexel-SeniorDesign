% [audio,fs] = wavread('./Seat_BR5.wav');
clc; close all; clear all;
[audio,fs] = wavread('../Mixed_Recordings/Seat_M15.wav');
audio = mean(audio,2);
spectrogram(audio,1024,512,1024,fs,'yaxis'); 
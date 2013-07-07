clc; close all; clear all;
fs = 44100;
load signal
plot(signal)
soundsc(signal,fs);
function [y,t,start,f,bell] = randomBell(f,totalTime,bellLength,tau,fs)

    if nargin < 3 
        fs = 44100;
    end
    close all;
    t = linspace(0,totalTime,totalTime*fs);
    y = 0.1*(rand(size(t))-0.5);
    bt = linspace(0,bellLength,fs*bellLength);
    bell = exp(-bt/tau).*sin(2*pi*f*bt);
    start = randi([1,totalTime*fs-length(bell)]);
    y(start:start+length(bell)-1) = y(start:start+length(bell)-1)+ bell;
    y = y./max(abs(y));
    save bell.mat t y f start
end
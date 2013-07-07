clc; close all; clear all;
signal = zeros(1,100);
filter = zeros(1,50);

count = 1;
for i = 21:30
    signal(i) = count;
    count = count + 1;
end

filter(6:10) = 1; 
result = xcorr(signal,filter);

plot(result)

load crosscorr
figure
plot(crosscorr)
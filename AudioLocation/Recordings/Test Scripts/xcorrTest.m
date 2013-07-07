close all;
toneFreqs = 440*2.^(0:3);
fs = 44100;
toneLength = 1;
t = linspace(0,toneLength,round(toneLength*fs));
tau = 0.075;
bell = exp(-t/tau);
tones = zeros(length(toneFreqs),length(t));
for i = 1:length(toneFreqs)
   tones(i,:) = sin(2*pi*toneFreqs(i)*t).*bell;
end

[microphone, iphone, fs] = getRecordings;

signal = microphone{3};
plot(signal)
figure
crossCorr = xcorr(signal ,tones(1,:));
plot(crossCorr)
[val,idx] = max(abs(crossCorr));
best = length(signal)-idx;
hold on
stem(best,1,'r')
hold off
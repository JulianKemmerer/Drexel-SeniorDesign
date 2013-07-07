clc; close all; clear all;
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

overlap = 64;
numSamples = 1024;
NFFT = 2^nextpow2(numSamples);
freqs = linspace(0,fs/2,NFFT/2+1);
perc = 0.01;
idxOffset = round(NFFT/2*perc/2);
threshold = 0.25;

percPower = [];

count = 0;
freqFound = zeros(size(toneFreqs));
i = 1;
foundStarts = [];
speakerNum = [];
numFound = 0;
full = microphone{1};

spectrogram(full,NFFT,overlap,NFFT,fs,'yaxis'); 
while (i+numSamples -1 < length(full))
    count = count+1;
    spec = abs(fft(full(i:i+numSamples-1),NFFT));
    power = spec(1:NFFT/2+1).^2;
    totalPower = sum(power);
    
    index = 0;
    for f = toneFreqs
       index = index+1;
       [~,idx] = min(abs(freqs-f)); 
       minIdx = idx-idxOffset;
       if minIdx < 1
           minIdx = 1;
       end
       maxIdx = idx+idxOffset;
       if maxIdx > NFFT/2+1
           maxIdx = NFFT/2+1;
       end
       fpower = sum(power(minIdx:maxIdx));
       percPower = fpower/totalPower;
       if percPower > threshold
           numFound = numFound+1;
           freqFound(index) = 1;
           startIndex = round(i-fs/2);
           if startIndex < 1
               startIndex = 1;
           end
           endIndex = round(i+3*fs/4);
           if endIndex > length(full)
               endIndex = length(full);
           end
           
           start = findAlignment(full(startIndex:endIndex),tones(index,:));
%            figure(2)
%            plot(full)
%            hold on
%            stem(i,1,'r','LineWidth',1.5)
%            hold off
           i = i+fs;
           foundStarts(numFound) = startIndex+start;
           speakerNum(numFound) = index;
%            figure(3)
%            plot(power)
%            pause(0.5)
%            keyboard
%            close 2 3
       end
    end
    i = i+overlap;
end

disp(['Found Start Points: ',num2str(foundStarts)])
disp(['Speaker Number: ',num2str(speakerNum)])
disp(['Speaker Time Delays: ',num2str(diff(foundStarts))])

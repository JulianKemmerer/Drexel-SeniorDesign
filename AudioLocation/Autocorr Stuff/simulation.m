clc; close all; clear all;
tic
toneLength = 0.5;
fs = 44100;
tau = 0.1;
toneFreqs = [1000,3000,5000,7000];
tones = zeros(4,fs*toneLength);
t = linspace(0,toneLength,fs*toneLength);
bell = exp(-t/tau);
count = 1;
for f = toneFreqs
    tones(count,:) = sin(2*pi*f*t).*bell;
    count = count+1;
end

totalTime = 8;

randStart = (totalTime-5)*rand*fs;
randDelays = rand([1,3])*fs;

starts = zeros(1,4);
starts(1) = randStart;
for i = 1:length(randDelays)
   starts(i+1) = randDelays(i)+starts(i)+fs;
end
full = 0.25*(2*rand(1,fs*totalTime)-1);
for i = 1:length(starts)
    full(round(starts(i)):round(starts(i))+toneLength*fs-1) = full(round(starts(i)):round(starts(i))+toneLength*fs-1)+tones(i,:);
end

full = full/(max(abs(full)));
plot(full)
overlap = 512;
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
numFound = 0;
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
           startIndex = round(i-fs);
           if startIndex < 1
               startIndex = 1;
           end
           endIndex = round(i+fs);
           if endIndex > length(full)
               endIndex = length(full);
           end
           
           start = findAlignment(full(startIndex:endIndex),tones(index,:));
           figure()
           plot(full)
           hold on
           stem(i,1,'r','LineWidth',1.5)
           hold off
           i = i+fs;
           foundStarts(numFound) = startIndex+start;
           figure()
           plot(power)
           keyboard
       end
    end
    i = i+overlap;
end
toc
disp(['Found Start Points: ',num2str(foundStarts)])
disp(['Known Start Points: ',num2str(round(starts))])

% diffs 
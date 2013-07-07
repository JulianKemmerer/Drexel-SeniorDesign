clc; close all; clear all;
f = 440*2^3;
fs = 44100;
T = 1;
tau = 0.1;
t = linspace(0,T,fs*T);
y = sin(2*pi*f*t);
bell = exp(-t/tau);
y = bell.*y;
offset = round(rand*fs);
% y = [zeros(1,offset),y,zeros(1,offset)];
y = wavread('H12.wav');

NFFT = 1024;
step = 64;
freqs = linspace(0,fs/2,NFFT/2+1);


maxF = fs/2;
split = maxF/(NFFT/2+1);
perc = 0.1;
matches = [];
hamWin = hanning(NFFT);

fMatches = [];
count = 0;
numMatches = 0;


for f = [440*2.^(0:3)]
    count = count+1;
    numMatches = 0;
    start = 1;
    freq2find = f;
    minFIndex = round(f*(1-perc)/split);
    maxFIndex = round(f*(1+perc)/split);
    starts = [];

    while (start < (length(y) - NFFT))

        chunk = y(start:start+NFFT-1).*hamWin.^4;
        
        Y = abs(fft(chunk,NFFT));
        [~,idx] = max(Y);
        if idx >= minFIndex && idx <= maxFIndex
            if numel(starts) >= 1 && abs(start-starts(end) > fs)
                numMatches = numMatches + 1;
                fMatches(count,numMatches) = starts(1);
                starts = [start];
            else
                starts = [starts,start];
            end
        end
        
        start = start+step;
    end
    index = NFFT/step;
    match = starts(index)+2*NFFT;
    matches = [matches,match];

end

% matches = [matches,matches(1)+fs*4]
matches = reshape(fMatches,1,numel(fMatches));
plot(y)
hold on
stem(matches,ones(size(matches)),'r')
hold off

delays = diff(matches/fs-(0:length(matches)-1))

% delays = diff(matches/fs-[0:4]);
% outString = '';
% 
% for i = 1:numel(delays)
%     outString = [outString,num2str(delays(i)),','];
% end
% outString = outString(1:end-1)




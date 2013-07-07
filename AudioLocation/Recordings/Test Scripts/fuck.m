clc; close all; clear all; 
y = wavread('H1.wav'); 
plot(y)
hold on
colors = jet(4);
count = 0 ;
vals = [];
for f = 440*2.^(0:3)
    count = count+1;
    fs = 44100;
    NFFT = 1024;
    step = 32;
    start = 1;

    freqs = linspace(0,fs/2,NFFT/2+1);
    window = hamming(NFFT).^2;

    perc = 0.1;
    split = fs/2/(NFFT/2+1);
    minFIndex = round(f*(1-perc)/split);
    maxFIndex = round(f*(1+perc)/split);
    starts = [];
    powerPerc = 0.15;

    plot(y)
    starts = [];
    while (start < length(y)-NFFT)

        chunk = y(start:start+NFFT-1).*window;
        Y = abs(fft(chunk)).^2;
        [val,idx] = max(Y);

        totalPower = sum(Y.^2);
        freqPower = sum(Y(minFIndex:maxFIndex).^2);
        if freqPower/totalPower > powerPerc
            starts = [starts,start+NFFT/2+step];
            start = start+2*fs;
        else
            start =  start+step;
        end
    end


    starts
    vals = [vals;starts];
    stem(starts,ones(size(starts)),'Color',colors(count,:))
end
vals = vals(:)/fs;
vals = vals-(0:length(vals)-1)';
delays = diff(vals)'




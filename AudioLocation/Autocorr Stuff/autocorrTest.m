
[y,t,start,f,bell] = randomBell(f,T,bellLength,tau,fs);
ham = hamming(NFFT);

plot(t,y)
figure

freqs = fs/2*linspace(0,1,NFFT/2+1);
[~,freqIndex] = min(abs(f-freqs));
offset = 0.25;
highFreq = f*(1+offset);
lowFreq = f*(1-offset);

[~,minIndex] = min(abs(lowFreq-freqs));
[~,maxIndex] = min(abs(highFreq-freqs));

percs = [];
count = 1;

for i = 1:overlap:length(y)-windowSize    
    window = transpose(ham).*y(i:i+NFFT-1);
    Y = fft(window,NFFT)/windowSize;
    energy = sum(abs(Y).^2);
    freqEnergy = sum(abs(Y(minIndex:maxIndex)).^2);
    percs(count,1) = freqEnergy/energy;
    percs(count,2) = i;
    percs(count,3) = i+windowSize;
    count = count+1;
end

suitable = percs(percs(:,1)>0.1,:);
crosscorrStart = min(suitable(:,2)) - length(bell)/2;
crosscorrEnd = max(suitable(:,2)) + length(bell)/2;

if crosscorrStart < 0
    crosscorrStart = 1;
end
if crosscorrEnd > length(y)
   crosscorrEnd = length(y); 
end

crossCorr = xcorr(bell,y(crosscorrStart:crosscorrEnd));
[~,corrmaxIndex] = max(abs(crossCorr));
plot(crossCorr)
foundStart = crosscorrStart+corrmaxIndex-length(bell)/2;

diffSamples = abs(start-foundStart);
distDiff = c*(diffSamples/fs);

disp(['Found start at sample ',num2str(foundStart),' or ',num2str(foundStart/fs),' seconds']);
disp(['Actual start at sample ',num2str(start),' or ',num2str(start/fs),' seconds']);
disp(['Calculations off by ',num2str(diffSamples),' samples']);
disp(['Translates to ',num2str(distDiff),' inches']);

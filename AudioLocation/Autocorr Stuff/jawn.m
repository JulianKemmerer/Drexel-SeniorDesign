
T = 0.5;
fs = 44100;
t = linspace(0,T,T*fs);
f = 440;
tau = 0.1;
tone = sin(2*pi*f*t);
bell = exp(-t/tau);
tone = tone.*bell;

bigT = 5;
offset = 3*rand;
full = 0.25*(2*rand(1,fs*bigT)-0.5);
full(round(offset*fs+1):round(offset*fs+length(tone))) = full(round(offset*fs+1):round(offset*fs+length(tone))) + tone;

NFFT = 512;
overlap = 32;
freqs = linspace(0,fs,NFFT/2+1);
[~,idx] = min(abs(freqs-f));
perc = 0.25;
minF = f*(1-perc/2);
maxF = f*(1+perc/2);
[~,minIdx] = min(abs(freqs-minF));
[~,maxIdx] = min(abs(freqs-maxF));
for i = 1:overlap:length(full)
    if (i+NFFT-1>length(full)) 
        break;
    end
    spec = abs(fft(full(i:i+NFFT-1),NFFT));
    power = spec(1:NFFT/2+1).^2;
    powerAtF = sum(power(minIdx:maxIdx))/sum(power);
    if powerAtF > 0.05
        break
    end
end

startIdx = round(i - fs);
if startIdx < 1
    startIdx = 1;
end
endIdx = round(i+fs);
temp = full(startIdx:endIdx);

foundStart = (findAlignment(temp,tone)+startIdx-1)
actualStart = offset*fs
diff = foundStart-round(actualStart)




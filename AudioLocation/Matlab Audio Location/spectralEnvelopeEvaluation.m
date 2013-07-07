close 

fs = data.sample_rate;
N = 1024;
frame = data.full_mix(20*fs:20*fs + N-1);
freq = linspace(0, fs/2, N/2+1);
offset = 20*log10(2/N);

X = abs(fft(frame, N));
X = X(1:N/2+1);

% Find zero crossings of differentiated spectrum
diffSpec = diff(X);
diffSpec(end +1) = diffSpec(end);
peak_locations = [true; (diffSpec(1:end-1) >=0) & (diffSpec(2:end) <=0)]; % Account for offset of 1
peak_indicies = find(peak_locations == 1);
peak_freqs = freq(peak_locations);

% Plot spectrum and peaks
subplot(3,1,1)
plot(freq,db(X) + offset); hold on
title('Peak Picked Envelope')
xlabel('Frequency (Hz)', 'FontSize', 12);
ylabel('Magnitude (dB)', 'FontSize', 12);
plot(peak_freqs, db(X(peak_locations)) + offset, 'r', 'LineWidth', 2)
xlim([0 15000])
peaks_per_octave = 2;

octave = [0 2.^(1:8)*100];

newPeaks = [1 ];
for octaveNum=2:length(octave)

	idx = find(peak_freqs < octave(octaveNum) & peak_freqs > octave(octaveNum-1));
	foundPeaks = length(idx);
	
	if(foundPeaks > peaks_per_octave)
		peakSample = floor(1/(peaks_per_octave/foundPeaks));		
	 	newPeaks = [newPeaks downsample(idx, peakSample)];		
	else
		newPeaks = [newPeaks idx];
	end
	
end

subplot(3,1,2)
plot(freq,db(X) + offset); hold on
plot(peak_freqs(newPeaks), db(X(peak_indicies(newPeaks))) + offset, 'g', 'LineWidth', 2)
title([num2str(peaks_per_octave) ' Peaks Per Octave Envelope'])
xlabel('Frequency (Hz)', 'FontSize', 12);
ylabel('Magnitude (dB)', 'FontSize', 12);
xlim([0 15000])
p = 50;

[a g] = lpc(frame, p);
lpEnv = abs(fft(a,N));
lpEnv = lpEnv(1:N/2+1);


%lpEnv = freqz(g,a,freq,fs);
subplot(3,1,3)
plot(freq,db(X) + offset); hold on
plot(freq, -db(lpEnv) + offset, 'm');
title('LP Envelope')
xlabel('Frequency (Hz)', 'FontSize', 12);
ylabel('Magnitude (dB)', 'FontSize', 12);
xlim([0 15000])
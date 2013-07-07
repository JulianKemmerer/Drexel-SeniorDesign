close all

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%          LOAD FILES
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


[recording fs] = wavread('recordings/chi2_chirp_200/M12.wav');
recording_time = linspace(0, length(recording)/fs, length(recording));

all_tones = wavread('./Tones/chi2_chirp_200.wav');
tones_time = linspace(0, length(all_tones)/fs, length(all_tones));

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%          PARAMETERS
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Low pass filter
Hd = ellipFilt;

thresh = 200;	%Peak threshold
numPlots = 2;

% Get source tones used for recordings
numTones = 4;
toneLength = floor(fs*0.1);
tones = zeros(toneLength,numTones);
for t = 1:numTones	
	tones(:,t) = all_tones((t-1)*fs + 1:(t-1)*fs+toneLength);	
end

figure('Position',[500 100 1200 800])

% Find onsets via cross-correlation
for t = 1:numTones

	% Plot audio
	subplot(numPlots,1,1);plot(recording_time,recording);
	ylabel('Magnitude', 'FontSize', 12);
	xlim([0 19])
	title(['Tone: ' num2str(t)], 'FontSize', 12);
	
	% Compute cross-correlation
	[Rxx lags] = xcorr(recording, tones(:,t));

	% Only want from zero lag onward
	zero_lag = floor(length(lags)/2);
	lags = lags(zero_lag:end);
	Rxx = Rxx(zero_lag:end);
	
	subplot(numPlots,1,2);plot(recording_time, Rxx(1:end-1));	
	xlim([0 19])
	hold on

	% Square the cross-correlation
	Rxx_squared = Rxx(1:end-1).^2;
	plot(recording_time, Rxx_squared, 'r--');
	
	% LP filter 
	y = filter(Hd, Rxx_squared);
	
	% Square signal again
	%y = y.*y;
	
	plot(recording_time-0.0016, y, 'g');
	
	% Find peaks in envelope
	yDiff = diff(y);
	yDiff(end+1) = yDiff(end);
	peak_locations = [false; (yDiff(1:end-1) >=0) & (yDiff(2:end) <=0)]; % Account for offset of 1
	peak_indicies = find(peak_locations == 1);
	
	% Find peaks greater than threshold	
	peak_indicies = peak_indicies(y(peak_indicies) > thresh);
	
	plot(recording_time(peak_indicies)-0.0016, y(peak_indicies), 'k+', 'MarkerSize', 10, 'LineWidth', 2);

	% Remove peaks trailing after the actual onset
	onsets = peak_indicies(1);
	onsetIdx = 2;
	for p = 2:length(peak_indicies)

		if((recording_time(peak_indicies(p)) - recording_time(peak_indicies(p-1))) > 0.9)
			onsets(onsetIdx) = peak_indicies(p);
			onsetIdx = onsetIdx + 1;
		end
	end

	plot(recording_time(onsets)-0.0016, y(onsets), 'm+', 'MarkerSize', 10, 'LineWidth', 2);
	xlabel('Time (sec)', 'FontSize', 12);
	
	onsetTimes{t} = onsets;
	% pause
	% clf

end

numOnsets = 10;
for t=1:numTones-1
	if(length(onsetTimes{t}) < numOnsets)
		numOnsets = length(onsetTimes{t});
	end
end

%onsetTimes{numTones+1} = onsetTimes{1};

for t=1:numTones-1

	delta_t(t) = mean(recording_time(onsetTimes{t+1}) - recording_time(onsetTimes{t}));
	
end

delta_t(numTones) = mean(recording_time(onsetTimes{1}(2:end)) - recording_time(onsetTimes{numTones}(1:end-1)));


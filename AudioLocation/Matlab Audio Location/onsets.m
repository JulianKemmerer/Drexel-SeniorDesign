clear 
figure('Position', [0 0 1200 800])

% Select tone
tone = 'hamming_chirp_110';

% Get directory/file info
filepath = ['recordings/' tone '/'];
D = dir([filepath '*wav']);
x = wavread(['Tones/' tone '.wav']);

% Square signal, differentiate, half-wave rectify
x = x.*x;
xDiff = diff(x);
xDiffAbs = abs(xDiff);
	
for d=1:length(D)

	% Get filename
	file = D(d).name;
	seat = file(1:3);
	
	[t fs] = wavread([filepath file]);

	% Square signal, differentiate, half-wave rectify
	t = t.*t;
	tDiff = diff(t);
	tDiffAbs = abs(tDiff);

	%%%%%%%%%%%% LOWPASS FILT %%%%%%%%%%%%%%%%%%
	Hd = ellipFilt;
	y = filter(Hd, xDiffAbs);
	v = filter(Hd, tDiffAbs);

	y_peaks = peakPick(y, 0.01);
	v_peaks = peakPick(v, 0.002)
	
	% Plot
	subplot(2,1,1)%, plot(xDiffAbs)
	title('Original Test Sequence')
	hold on
	plot(y, 'r')
	plot(y_peaks, y(y_peaks), 'k+');
	% axis([2e4 16e4 0 0.4])
	hold off


	subplot(2,1,2)%, plot(tDiffAbs)
	title(['Seat Number: ' num2str(seat)])
	hold on
	plot(v, 'r')
	plot(v_peaks, v(v_peaks), 'k+');
	axis([0 4e5 0 0.01])
	hold off

	pause
	clf

end
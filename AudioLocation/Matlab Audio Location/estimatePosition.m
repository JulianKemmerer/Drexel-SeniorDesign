filepath = 'Audio Location Recordings/';

t = wavread([filepath '147_173.wav']);
t = abs(t);
tDiff = diff(t);
tDiffAbs = abs(tDiff);

[x fs] = wavread('testSequence.wav');
audio = x;
xDiff = diff(x);
xDiffAbs = abs(xDiff);

%%%%%%%%%%%% LOWPASS FILT %%%%%%%%%%%%%%%%%%

N     = 5;   % Order
Fpass = 100;  % Passband Frequency
Apass = 1;    % Passband Ripple (dB)
Astop = 80;   % Stopband Attenuation (dB)

% Construct an FDESIGN object and call its ELLIP method.
h  = fdesign.lowpass('N,Fp,Ap,Ast', N, Fpass, Apass, Astop, fs);
Hd = design(h, 'ellip');

y = filter(Hd, xDiffAbs);
v = filter(Hd, tDiffAbs);
	
% Plot
subplot(2,1,1)
title('Original Test Sequence')
plot(xDiffAbs);
axis([1.55e5 1.57e5 0 0.3])

subplot(2,1,2)
plot(y, 'r');
axis([1.55e5 1.57e5 0 0.1])


% Plot
subplot(2,1,1)
plot(tDiffAbs);
title('Diff, HWR')
%axis([2.68e5 2.71e5 0 0.7])

subplot(2,1,2)
plot(v, 'r');
title('Low Pass')
%axis([2.68e5 2.71e5 0 0.2])

dcmObj = datacursormode;
set(dcmObj, 'UpdateFcn', @cursorUpdateFnc);

true_onsets = [183572 227196 271843 315795 359935]/fs;
est_onsets = [138800 183000 226700 270200 315100]/fs;

diff(true_onsets)
diff(est_onsets)

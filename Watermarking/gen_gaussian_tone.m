function y = gen_gaussian_tone(f, f_upper)
% GEN_HANNING_TONE generates a 1/2 s Hanning windowed tone at a constant
% frequency of F with a sampling rate of 44,100 kHz. For a chrip, specify
% an upper frequency.

fs = 44100;
winLength = fs/2;
t = linspace(0, 0.5, winLength);
x = linspace(0,15,winLength);

win = pdf('chi2', x, 2.5);

if nargin < 2
    y = win.*sin(2*pi*f*t);
else
    f_c = linspace(f, f_upper, winLength);
    y = win.*sin(2*pi.*f_c.*t);
end

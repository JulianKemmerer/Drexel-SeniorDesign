function y = gen_hanning_tone(f, f_upper)
% GEN_HANNING_TONE generates a 1/2 s Hanning windowed tone at a constant
% frequency of F with a sampling rate of 44,100 kHz. For a chrip, specify
% an upper frequency.

fs = 44100;
t = linspace(0, 0.5, fs/2);
if nargin < 2
    y = hann(numel(t))'.*sin(2*pi*f*t);
else
    f_c = linspace(f, f_upper, fs/2);
    y = hann(numel(t))'.*sin(2*pi.*f_c.*t);
end
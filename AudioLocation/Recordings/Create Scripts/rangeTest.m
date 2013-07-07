
fs = 44100;
T = 10;
t = linspace(0,T,T*fs);

f = linspace(400,15000,length(t));

y = sin(2*pi*f.*t);

soundsc(y,fs)

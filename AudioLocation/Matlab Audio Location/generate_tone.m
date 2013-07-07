fs = 44100;
duration = 0.5;

t = 0:1/fs:duration;
freq = 2000;
x = sin(2*pi*freq*t);
plot(x,t)
xlabel('Time (sec)', 'FontSize', 12);	
ylabel('Amplitude', 'FontSize', 12);

wavwrite(x, fs, 'sine.wav');

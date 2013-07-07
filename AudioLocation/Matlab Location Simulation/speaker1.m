fs = 44100;
dur = 0.1;
t = 0:1/fs:dur;
a  = cos(2*pi*440*t);

soundsc(a,fs);

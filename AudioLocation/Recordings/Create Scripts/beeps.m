f = 440*2.^(0:3);
f = [16000,17000,18000,19000];
fs = 44100;
toneLength = 1;
t = linspace(0,toneLength,round(toneLength*fs));
tau = 0.075;
bell = exp(-t/tau);
offset = 1000;
ramp = linspace(0,1,offset);
bell(1:offset) = bell(1:offset).*ramp;
tones = zeros(length(f),length(t));
for i = 1:length(f)
   tones(i,:) = sin(2*pi*f(i)*t).*bell;
end
plot(tones(1,:))

silence = zeros(1,fs);

speak1 = [tones(1,:),silence,silence,silence];
speak2 = [silence,tones(2,:),silence,silence];
speak3 = [silence,silence,tones(3,:),silence];
speak4 = [silence,silence,silence,tones(4,:)];

wavwrite(speak1,fs,'speak1.wav')
wavwrite(speak2,fs,'speak2.wav')
wavwrite(speak3,fs,'speak3.wav')
wavwrite(speak4,fs,'speak4.wav')
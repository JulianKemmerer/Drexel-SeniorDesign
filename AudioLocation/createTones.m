f = [440,466.164,493.883,523.251];
fs = 44100;
rampTime = 1/200;
t = linspace(0,1,fs);
tau  = 0.05;
bell = exp(-t/tau);
ramp = linspace(0,bell(round(rampTime*fs)),round(rampTime*fs));
bell(1:length(ramp)) = ramp;

plot(t,bell)
speakerTones = zeros(4,44100);
for i = 1:length(f)
    speakerTones(i,:) = sin(2*pi*f(i)*t).*bell;
end
blank = zeros(1,fs);

audio1 = [speakerTones(1,:),blank,blank,blank];
audio2 = [blank,speakerTones(2,:),blank,blank];
audio3 = [blank,blank,speakerTones(3,:),blank];
audio4 = [blank,blank,blank,speakerTones(4,:)];

wavwrite(audio1,fs,'speaker1.wav')
wavwrite(audio2,fs,'speaker2.wav')
wavwrite(audio3,fs,'speaker3.wav')
wavwrite(audio4,fs,'speaker4.wav')
soundsc(audio4,fs)

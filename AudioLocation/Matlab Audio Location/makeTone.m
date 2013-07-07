fs = 44100;
freq = 880;
t= 1;
L = 40;
numSpeakers = 4;
time = linspace(0,t,t*fs);
tone = sin(2*pi*freq*time);
tau = 0.125;
bell = exp(-time./tau);
ramp = linspace(0,1,25);
newbell = [ramp,bell];
bell = newbell(1:numel(bell));

y = tone.*bell;
% plot(time,y)
% soundsc(y,fs)

space = zeros(1,t*fs);
tones = [];
for i = 1:numSpeakers
    temp = [];
    for j = 1:L
       if mod(j,numSpeakers) == 0
           temp = [temp,y];
       else
           temp = [temp,space];
       end
    end
    for k = 2:i
       temp = [space,temp]; 
    end
    temp = temp(1:L*fs);
    tones = [tones;temp];
end

for i = 1:numSpeakers
   wavwrite(tones(i,:),fs,['./beeps/speaker_',num2str(i),'.wav']);
end

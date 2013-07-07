%function out_sig = embed(sig1,sig2)

% Load the .wav data
signal1=wavread('acapella.wav',44100*10);
signal2=wavread('speak1.wav');
signal3=wavread('speak2.wav');
signal4=wavread('speak3.wav');
signal5=wavread('speak4.wav');

%signal1 = wavread(inputname(1),44100*2);
%signal2 = wavread(inputname(2),44100*2);

% Add it up
beeps = signal2+signal3+signal4+signal5;


%sig_out=signal1+signal2;
%out_sig = beeps;

% Normalise it to lie between -1 and +1 again
beeps=2*beeps/(max(beeps)-min(beeps));
beeps=beeps-min(beeps)-1;

% Loop beeps for length of audio track
ratio = length(signal1)/length(beeps);
fl_ratio = floor(ratio);
sig_out_rat=(ratio-fl_ratio)*length(beeps);


sig_end = beeps(1:sig_out_rat);
beep_loop = beeps;

for i = 1:fl_ratio - 1
    beep_loop = [beep_loop;beeps];
end;

beep_loop = [beep_loop;sig_end];

% Embed beeps in audio signal
sig_out = beep_loop + signal1;

% Write .wav file
wavwrite(beep_loop,44100,'beeps.wav');
wavwrite(sig_out,44100,'output.wav');


c = 13397.2441;
f = 440;
fs = 44100;
T = 5;
bellLength = 1;
tau = 0.15;
overlap = 512;
windowSize = 1024;
NFFT = 2^nextpow2(windowSize);

iterations = 1;

foundDistances = zeros(1,iterations);
sampleDistances = zeros(1,iterations);
temp = 1;
for j = 1:iterations
    fprintf('\n')
    disp(['Iteration ',num2str(j)]);
    autocorrTest;
    foundDistances(temp) = distDiff;
    sampleDistances(temp) = diffSamples;
    temp = temp+1;
end

fprintf('\n\n')
disp(['Average distance off is ',num2str(mean(foundDistances)),' inches']);
disp(['Average sample diff is ',num2str(mean(sampleDistances))]);
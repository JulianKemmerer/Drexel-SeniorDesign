count = 0;
for i = 1:10000
    N = 100;
    sig1 = zeros(1,N);
    sigPingIndex = randi([1,N]);
    sig1(sigPingIndex) = 1;
    sig2 = zeros(1,N);
    sig2(50) = 1;

    out = xcorr(sig2,sig1);
    [val,index] = max(out);

    count = count + (index - sigPingIndex);
end

count/10000
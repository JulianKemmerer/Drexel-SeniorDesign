function index = findAlignment(signal,known)
    crosscorr = xcorr(known,signal);
    [~,idx] = max(crosscorr);
    index = length(signal)-idx;
end
load values
fs =44100;
f = linspace(0,fs/2,length(values));
plot(f,values)
[~,idx] = max(values);
f(idx)
function [microphone,iphone,fs] = getRecordings
    % Get all recordings from microphone
    [~, microphone] = system('find ../Microphone -iname "*.wav"');
    microphone = textscan(microphone,'%s');
    microphone = microphone{1}
    micro = cell(1,length(microphone));
    
    for i = 1:length(microphone)
       [micro{i},fs] = wavread(microphone{i});
    end
    
    [~, iphone] = system('find ../iPhone -iname "*.wav"');
    iphone = textscan(iphone,'%s');
    iphone = iphone{1};
    phone = cell(1,length(iphone));

    for i = 1:length(iphone)
       [phone{i},fs] = wavread(iphone{i});
    end
    
    microphone = micro;
    iphone = phone;
end


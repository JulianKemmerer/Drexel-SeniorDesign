% clc; close all; clear all;
[starts,speakers] = getRealMatches(2);
close all;
c = 13397.2441;
fs = 44100;
delays = (diff(starts)/fs);
% dists = delays*c/12;
% sx = [0   516 419 97];
% sy = [772 772 0   0];
% angles = -pi:0.01:pi;
addpath('/Users/Zimmerman/Dropbox/Senior Design/AudioLocation/Matlab Audio Location')

delays(5:9)
diff(delays(5:9))
% findLocationFromDelay(delays(5:8))

% speaks = speakers(6:9);
% deltas = delays(5:8);
% 
% deltas = abs(deltas-1);
% dists = deltas*c;
% angles = -pi:0.01:pi;
% 
% figure
% 
% hold on
% colors = hsv(length(deltas));
% count = 0;
% for i = 1:length(deltas)
%     count = count + 1;
%     circX = dists(i)*cos(angles) + sx(speaks(i));
%     circY = dists(i)*sin(angles) + sy(speaks(i));
%     plot(circX,circY,'color',colors(count,:));
% end
% % axis([min(sx),max(sx),min(sy),max(sy)]);
% hold off
function peak_indicies = peakPick(y, thresh)

% Find peaks in envelope
yDiff = diff(y);
yDiff(end+1) = yDiff(end);
peak_locations = [false; (yDiff(1:end-1) >=0) & (yDiff(2:end) <=0)]; % Account for offset of 1
peak_indicies = find(peak_locations == 1);

% Find peaks greater than threshold	
peak_indicies = peak_indicies(y(peak_indicies) > thresh);

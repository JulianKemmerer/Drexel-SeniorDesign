clc; close all; clear all;
sx = [0,516,419,97];
sy = [772,772,0,0];
sz = [120,120,120,120];
maxX = max(sx);
minX = min(sx);
minY = min(sy);
maxY = max(sy);
maxZ = max(sz);
xrange = maxX - minX;
yrange = maxY - minY;
zrange = max(sz);
xStep = 24;
yStep = 24;
zStep = 36;
numX = round(xrange/xStep);
numY = round(yrange/yStep);
numZ = round(zrange/zStep);
xVals = linspace(minX,maxX,numX);
yVals = linspace(minY,maxY,numY);
zVals = linspace(0,maxZ,numZ);
tic
numSpeakers = numel(sx);
dists = zeros(numX,numY,numZ,numSpeakers);
for s = 1:numSpeakers
    for x = 1:numX
        xDist = (sx(s)-xVals(x))^2;
        for y = 1:numY
            yDist = (sy(s)-yVals(y))^2;
            for z = 1:numZ
                zDist = (sz(s)-zVals(z))^2;
                dists(x,y,z,s) = sqrt(xDist+yDist+zDist);
            end
        end
    end
end
toc
c = 13397.2441;
dists(:,:,:,5) = dists(:,:,:,1);
times = dists/c;
x = xVals(x);
y = yVals(1);
z = zVals(1);
delays = diff(times,1,4);
testDelays = [0.0136961451247165      -0.00952380952380949       -0.0109750566893423       0.00734693877551029];
minDist = 2^31;
count = 0;
bestX = 0;
bestY = 0;
bestZ = 0;
for x = 1:numX
    for y = 1:numY
        for z = 1:numZ
            delayVals = reshape(delays(x,y,z,:),1,numel(sx));
            distance = sqrt(sum((delayVals-testDelays).^2));
            if distance <= minDist
               minDist = distance; 
               bestX = x;
               bestY = y;
               bestZ = z;
            end
        end
    end
end
xVals(bestX)
yVals(bestY)
zVals(bestZ)
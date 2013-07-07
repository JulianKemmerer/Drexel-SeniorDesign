xLoc = [40,500,0,450];
yLoc = [5,1000,1000,90];
zLoc = [50,50,50,50];

plot(xLoc,yLoc,'rx','MarkerSize',20)

xMin = min(xLoc);
yMin = min(yLoc);
xMax = max(xLoc);
yMax = max(yLoc);
zMin = 0;
zMax = max(zLoc);

c = 13397.2441;

fullGrid = [xMin,yMin,xMax,yMax];

xGrid = linspace(xMin,xMax,500);
yGrid = linspace(yMin,yMax,500);
zGrid = linspace(zMin,zMax,zMax-zMin);

grids = cell(1,numel(xLoc));
for i = 1:numel(xLoc)
   grids{i} = zeros(size(xGrid,2),size(yGrid,2),size(zGrid,2))  
end

for i = 1:size(xGrid,2)
    for j = 1:size(yGrid,2)
        for k = 1:size(zGrid,2)
            
            
            
        end
    end
end

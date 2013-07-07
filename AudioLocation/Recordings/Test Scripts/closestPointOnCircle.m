function closest = closestPointOnCircle(center,radius,point)
    closest = zeros(size(point));
    base = sqrt((point(1)-center(1))^2 + (point(2)-center(2))^2);
    closest(1) = center(1) + radius*(point(1)-center(1))/base;
    closest(2) = center(2) + radius*(point(2)-center(2))/base;
end
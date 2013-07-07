function area = getTriangleArea(point1,point2,point3)
    area = abs((point1(1)*(point2(2)-point3(2)) + point2(1)*(point3(2)-point1(2)) ...
        + point3(1)*(point1(2)-point2(2)))/2);
end
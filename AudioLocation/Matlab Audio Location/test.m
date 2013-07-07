x = linspace(5,6,100);
y = diff(x);
result = size(1,numel(x)-1);
for i = 1:numel(x)-1
    result(i) = x(i+1)-x(i);
end

y
result
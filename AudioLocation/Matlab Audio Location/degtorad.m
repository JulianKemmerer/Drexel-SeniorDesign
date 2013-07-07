function out = degtorad(in)
    out = zeros(1,numel(in));
    for i  = 1:numel(in)
       out(i) =  in(i)*(pi/180);
    end
end
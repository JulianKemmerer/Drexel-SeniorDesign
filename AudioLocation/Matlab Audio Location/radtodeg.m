function out = radtodeg(in)
    out = zeros(1,numel(in));
    for i  = 1:numel(in)
       out(i) =  in(i)*(180/pi);
    end
end
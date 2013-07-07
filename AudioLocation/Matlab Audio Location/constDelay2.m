clear
close
clc

%%%% ALL LENGTH MEASUREMENTS ARE IN INCHES %%%%%%

c = 1129.5202682997958*12; % in/sec

% Speaker locations
sx = [0 197 197 0 0];
sy = [259 259 0 0 259];

% Distance between speakers
Lx = 197;
Ly = 259;
L = [Lx Ly Lx Ly];

xOff = [0 0 180 270];
yOff = [0 90 180 270];

% Show speakers
plot(sx, sy,'+', 'MarkerSize', 10, 'LineWidth', 2); hold on
plot(147, 173, 'r+', 'MarkerSize', 8, 'LineWidth', 2);
grid on;
axis([-10 Lx+10 -10 Ly+10])

% Vector of potential side lengths
%delta_t = 1 - [1.007 0.9913 0.9851 1.0232]; % 0_0.wav
%delta_t = 1 - [1.002 0.9971 0.9929 1.0109]; % 49_87.wav
delta_t = 1 - [0.9892 1.0124 0.9966 1.0009]; % 174_173.wav

const = c*delta_t;
H = 0:325;

% Calculate possible triangles
for s = 1:length(L)
	for h = 1:length(H)

	    % Possible side lengths d1,d2
	    [side1(s,h) d1] = deal(H(h));
	    [side2(s,h) d2] = deal(d1 - const(s));

	    if(d1 + d2 <= L(s) || d1 > 325 || d2 > 325|| d1 <=0 || d2 <=0)
	        continue
	    end

	    % Given side lengths, compute angles of triangle
	    theta(s,h) = radtodeg(acos((d1^2 + d2^2 - L(s)^2)/(2*d1*d2)));
	    phi_1(s,h) = radtodeg(acos((-d1^2 + d2^2 + L(s)^2)/(2*d2*L(s))));
	    phi_2(s,h) = radtodeg(acos((d1^2 - d2^2 + L(s)^2)/(2*d1*L(s))));

	end

end

%%%%%%%%%%%%%%%%% PLOT %%%%%%%%%%%%%%%%%%
rotation = [360 90 180 270];
for s=1:4
	
	valid_points = phi_2(s,:) ~= 0;
	[x y] = pol2cart(degtorad(rotation(s) - phi_2(s,valid_points)), side1(s,valid_points));
	
	if(s==1 || s==3)
		xCoords = [sx(s) (sx(s) + x)];
		yCoords = [sy(s) (sy(s) + y)];
	else
		xCoords = [sx(s) (sx(s) - x)];
		yCoords = [sy(s) (sy(s) - y)];
	end
	
	plot(xCoords, yCoords);
end

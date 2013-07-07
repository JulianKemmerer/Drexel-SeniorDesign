function findLocationFromDelay(delta_t)

    close all
    delta_t = 1 - delta_t;

    %%%% ALL LENGTH MEASUREMENTS ARE IN INCHES %%%%%%

    c = 10000.2441; % in/sec

    % Speaker locations
    sx = [0   516 419 97 0];
    sy = [772 772 0   0  772];

    % Distance between speakers
    L = zeros(1,numel(sx)-1);
    for i = 1:numel(sx)-1
       L(i) = sqrt((sx(i+1)-sx(i))^2+(sy(i+1)-sy(i))^2);
    end

    % Show speakers
    plot(sx, sy,'+', 'MarkerSize', 10, 'LineWidth', 2); hold on
    % plot(147, 173, 'r+', 'MarkerSize', 8, 'LineWidth', 2);
    grid on;
    axis([-10,max(sx)+10,-10,max(sy)+10])

    const = c*delta_t;
    H = 0:900;

    xVals = [];
    yVals = [];
    % Calculate possible triangles
    for s = 1:length(L)
        for h = 1:length(H)
            % Possible side lengths d1,d2
            [side1(s,h),d1] = deal(H(h));
            [side2(s,h),d2] = deal(d1 - const(s));
            if(d1 + d2 <= L(s) || d1 > 900 || d2 > 900|| d1 <=0 || d2 <=0)
                [theta(s,h),phi_1(s,h),phi_2(s,h)] = deal(0);
            end
            % Given side lengths, compute angles of triangle
            theta(s,h) = radtodeg(acos((d1^2 + d2^2 - L(s)^2)/(2*d1*d2)));
            phi_1(s,h) = radtodeg(acos((-d1^2 + d2^2 + L(s)^2)/(2*d2*L(s))));
            phi_2(s,h) = radtodeg(acos((d1^2 - d2^2 + L(s)^2)/(2*d1*L(s))));
        end
    end

    %%%%%%%%%%%%%%%%%  PLOT  %%%%%%%%%%%%%%%%%%
    rotation = [0,90,180,270];
    for s=1:4
        valid_points = phi_2(s,:) > 0;
        [x,y] = pol2cart(degtorad(rotation(s) - phi_2(s,valid_points)), side1(s,valid_points));
        if(s==1 || s==3)
            xCoords = [sx(s) (sx(s) + x)];
            yCoords = [sy(s) (sy(s) + y)];
        else
            xCoords = [sx(s) (sx(s) - x)];
            yCoords = [sy(s) (sy(s) - y)];
        end
        
        xSmall = xCoords(xCoords<max(sx));
        x = xSmall(xSmall>min(sx));
        xVals = [xVals,mean(x)];
        ySmall = yCoords(yCoords<max(sy));
        y = ySmall(ySmall>min(sy));
        yVals = [yVals,mean(y)];
%         mean(xCoords(xCoords<max(sx)))
%         mean(yCoords(yCoords<max(sy)))
%         plot(xCoords, yCoords);
    end
    
    xMean = mean(xVals(~isnan(xVals)));
    yMean = mean(yVals(~isnan(yVals)));
    
    disp(['(',num2str(xMean),',',num2str(yMean),')']);
end
    
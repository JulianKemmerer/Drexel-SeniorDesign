% Sphere 1 =  x^2 + y^2 + z^2  = R^2
% Sphere 2 = (x-d)^2 + y^2 + z^2 = r^2
clc; close all; clear all;

d = 4;
r = 5;
R = 2.25;

sound = 13397.2441;

x = (d^2-r^2+R^2)/(2*d);
a = (1/(2*d))*sqrt(4*d^2*R^2-(d^2-r^2+r^2)^2);
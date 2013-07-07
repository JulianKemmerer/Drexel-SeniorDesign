  % Initialize spectrogram parameters
  windowSize=1024;
  NFFT = 1024;
  hopSize = 512;
  window = hann(windowSize);
  numpeaks = 1;
  fs = 44100;
  
  % Read in audio file and take a spectrogram
  x=wavread('acapella.wav');
  [S,F,T] = spectrogram(x, window, hopSize, NFFT, fs) ;
  
  % Find 
  A = abs(S);
  Phi = angle(S);
  
  for i=1:numFrames
    [p,l]= findpeaks(A(:,i),'NPEAKS',numpeaks);
    y=isempty(p);
    if y==0
       pks(:,i)=p;
       locs(:,i)=l;
     end
  end

   [r2,c] = size(locs);
   for j=1:c
       Phi_j=Phi(:,j);
       locs_j=locs(:,j);
       z=isequal(zeros(r2,1),locs(:,j));
       if z==1
       Phase(:,j) = zeros(r2,1);
       Freq(:,j)=zeros(r2,1);
       else
       Phase(:,j) = Phi_j(locs_j(1:length(locs_j)));
       Freq(:,j) = F(locs_j(1:length(locs_j)));
       end;
    end;
    [max_peak,max_ind] = max(pks);
    
    
    
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%  
    Freq;
    A=cat(1,double(encodedData.amplitude1),encodedData.amplitude2);
    Phase=cat(1,double(encodedData.phase1),encodedData.phase2);
    
    
    %Phase = encodedData.phases;
    t = [0:1/fs:(encodedData.windowSize-1)/fs] ;
   
    [r,frames]=size(A);
    audio=zeros(1,encodedData.audioLength);
    Out=zeros(1,encodedData.windowSize);

    for j=1:frames  
        
        for i=1:r
            Out = Out +A(i,j)*cos((2*pi*f(i,j)*t)+Phase(i,j)) ; 
        end
       
        Out=Out.*hann(encodedData.windowSize)'./(encodedData.windowSize/2);
        start = 1+(j-1)*encodedData.hopSize;
        stop = (encodedData.windowSize+((j-1)*encodedData.hopSize));
        audio(start:stop)= audio(start:stop)+ Out;
    end
   
    
	fs = encodedData.fs;
    
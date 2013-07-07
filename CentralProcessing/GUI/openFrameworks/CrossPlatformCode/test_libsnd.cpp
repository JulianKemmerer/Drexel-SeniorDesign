#include <sndfile.hh>
#include <stdio.h>

void test_libsnd::setup(){

const char* fn = "C:\Users\Zimmerman\Music\Sample WAV files\Radiohead.wav";
SndfileHandle myf = SndfileHandle(fn);

cout<<"opened file"<<fn<<endl;
cout<<"Sample rate"<<myf.samplerate())<<endl;
cout<<"Channels: "<<myf.channels())<<endl;
cout<<"Error:"<<myf.strError())<<endl;
cout<<"Frames:"<<int(myf.frames()))<<endl;

}
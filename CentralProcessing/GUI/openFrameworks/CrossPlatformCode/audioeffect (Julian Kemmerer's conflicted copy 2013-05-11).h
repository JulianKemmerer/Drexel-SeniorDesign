#include <vector>
#include <ctime>

class Buffer
{
public:
	int buffsize;
private:
	int Buffer::copyTo(float * destination)
	{
		allocDataIfNeeded();
		return copy(data,destination);
	}
	int Buffer::copyFrom(float * source)
	{
		allocDataIfNeeded();
		return copy(source,data);
	}

	float * data;
	int Buffer::copy(float * source, float * destination)
	{
		if(source == NULL || destination == NULL)
		{
			return 0;
		}

		for(int i = 0; i < buffsize; i++)
		{
			destination[i] = source[i];
		}
		return buffsize;
	}

	void Buffer::allocDataIfNeeded()
	{
		if(data==NULL)
		{
			data = new float[buffsize];
		}
	}
};


//Abstract class that is base class for audio effects
//Gives init function, close function, and buffers for audio
class AudioEffect 
{
public:
	int buffsize;
	int samplerate;
	bool fl_enabled;
	bool fr_enabled;
	bool sl_enabled;
	bool sr_enabled;
	virtual void init() = 0;
	virtual void close() = 0;
	virtual void slBufferRequested(vector<float *> * buffer) = 0;
	virtual void flBufferRequested(vector<float *> * buffer) = 0;
	virtual void srBufferRequested(vector<float *> * buffer) = 0;
	virtual void frBufferRequested(vector<float *> * buffer) = 0;
	//Volume
	float volume;

	void AudioEffect::writeFloatPtrBufToFloatPtrVec(float * ptrbuf, vector<float *> * vecbuf)
	{
		if(buffsize != vecbuf->size())
		{
			cout << "Audio effect could not write into buffer - sizes do not match." << endl;
			return;
		}

		//Loop through and write
		for (int i = 0; i < buffsize; i++)
		{
			(*(*vecbuf)[i]) += ptrbuf[i] * volume;
		}
	}
};

//Derived class 
class SingleWAVFileAudioPlayer : public AudioEffect
{
public:
	string filename;
	bool paused;
	//Easiest way to get repeated samples from file...
	SndfileHandle fl_sndfilehandle;
	SndfileHandle fr_sndfilehandle;
	SndfileHandle sl_sndfilehandle;
	SndfileHandle sr_sndfilehandle;
	float * readBuffer;

	SingleWAVFileAudioPlayer::SingleWAVFileAudioPlayer()
	{
	}

	void SingleWAVFileAudioPlayer::init()
	{
		paused = false;
		fl_enabled = true;
		fr_enabled = true;
		sl_enabled = true;
		sr_enabled = true;
		string wav_filename = filename;
		fl_sndfilehandle = SndfileHandle(wav_filename.c_str());
		fr_sndfilehandle = SndfileHandle(wav_filename.c_str());
		sl_sndfilehandle = SndfileHandle(wav_filename.c_str());
		sr_sndfilehandle = SndfileHandle(wav_filename.c_str());
		readBuffer = new float[buffsize];
	}
	void SingleWAVFileAudioPlayer::close()
	{
		free(readBuffer);
	}

	float * SingleWAVFileAudioPlayer::getAudioFromFile(SndfileHandle * handle)
	{
		handle->read(readBuffer,buffsize);
		return readBuffer;
	}

	void SingleWAVFileAudioPlayer::slBufferRequested(vector<float *> * buffer)
	{
		//Read from the audio file
		float * audioFromFile = getAudioFromFile(&sl_sndfilehandle);

		//Only write into the buffer if enabled
		if(sl_enabled)
		{
			writeFloatPtrBufToFloatPtrVec(audioFromFile,buffer);
		}
	}
	void SingleWAVFileAudioPlayer::flBufferRequested(vector<float *> * buffer)
	{
		//Read from the audio file
		float * audioFromFile = getAudioFromFile(&fl_sndfilehandle);

		//Only write into the buffer if enabled
		if(fl_enabled)
		{
			writeFloatPtrBufToFloatPtrVec(audioFromFile,buffer);
		}
	}
	void SingleWAVFileAudioPlayer::srBufferRequested(vector<float *> * buffer)
	{
		//Read from the audio file
		float * audioFromFile = getAudioFromFile(&sr_sndfilehandle);

		//Only write into the buffer if enabled
		if(sr_enabled)
		{
			writeFloatPtrBufToFloatPtrVec(audioFromFile,buffer);
		}
	}
	void SingleWAVFileAudioPlayer::frBufferRequested(vector<float *> * buffer)
	{
		//Read from the audio file
		float * audioFromFile = getAudioFromFile(&fr_sndfilehandle);

		//Only write into the buffer if enabled
		if(fr_enabled)
		{
			writeFloatPtrBufToFloatPtrVec(audioFromFile,buffer);
		}
	}
};

class Chirp : public AudioEffect
{
public:
	//Start end freqs
	int freqStart;
	int freqEnd;
	//How long is the chirp
	//Under 10ms all comes out of speakers as around 10ms long...
	int duration_in_ms;
	//How often to chirp?
	int repeatPeriodMS;
	//Need to know sample rate


	float * chirpBuffer;
	//Keep track of how many samples requested on each channel
	int sl_sample_counter;
	int sr_sample_counter;
	int fl_sample_counter;
	int fr_sample_counter;
	int duration_in_samples;
	//Clock for keeping time
	clock_t last_chirp_start_time;
	bool chirp_write_enable;

	Chirp::Chirp()
	{

	}

	void Chirp::init()
	{
		fl_enabled = true;
		fr_enabled = true;
		sl_enabled = true;
		sr_enabled = true;
		chirpBuffer = new float[buffsize];
		sl_sample_counter = 0;
		sr_sample_counter = 0;
		fl_sample_counter = 0;
		fr_sample_counter = 0;
		chirp_write_enable = false;
		srand(time(NULL));
	}
	void Chirp::close()
	{

	}

	void Chirp::startChirping()
	{
		//Initialize counter...etc.
		last_chirp_start_time = clock();
		//Only init time, do not write enable
		//Want to wait one period of time before chirping for first time
	}

	void Chirp::playChirp()
	{
		last_chirp_start_time = clock();
		chirp_write_enable = true;
		sl_sample_counter = 0;
		sr_sample_counter = 0;
		fl_sample_counter = 0;
		fr_sample_counter = 0;
	}

	float * Chirp::getChirpAudio(int sample_count)
	{
		// r samples per second
		// r/1000 samples per milisecond
		// (r/1000) * ms = samples
		duration_in_samples = (samplerate/1000.0)*duration_in_ms;

		//Chirp must go from start to end freq in time t
		//k = end-start /t
		float k = ((float)(freqEnd - freqStart))/((float)duration_in_ms/1000.0);

		//'Time' is sample_count
		//Buffer to write into is: chirpBuffer, size = buffsize
		float * buffer = chirpBuffer;
		int bufferSize = buffsize;
		//Write the buffsize number of samples starting at sample_count
		for(int i = 0; i < bufferSize; i++)
		{
			// x(t) = sin(2pi * f(t) )
			float t_ms = ((float)sample_count / (float)duration_in_samples)*duration_in_ms;
			float t = t_ms/1000.0;
			float f = freq_at_time(t,k);
			float sample = sin(2*PI*f);
			buffer[i] = sample;
			//Keep track of sample count in addition to 'i' position in buffer
			sample_count++;
		}
		return buffer;
	}

	float Chirp::freq_at_time(float t_sec, float k)
	{
		// f(t) = fstart*t + 0.5*k*t^2
		return freqStart*t_sec + 0.5*k*t_sec*t_sec;
	}

	void Chirp::checkClockAndUpdateWriteFlag()
	{
		clock_t current_time = clock();
		if((current_time - last_chirp_start_time) > repeatPeriodMS)
		{
			//We need play another chirp now
			playChirp();
		}
		else
		{
			//Is it time to stop playing?
			if((current_time - last_chirp_start_time) > duration_in_ms)
			{
				chirp_write_enable = false;
			}
		}
	}

	void Chirp::slBufferRequested(vector<float *> * buffer)
	{
		//Check the time, enable chirp writing to buffer
		checkClockAndUpdateWriteFlag(); 
		
		//Only write into the buffer if enabled
		if(sl_enabled)
		{
			//Only write if chirping is enabled
			if(chirp_write_enable)
			{
				float * audio = getChirpAudio(sl_sample_counter);
				writeFloatPtrBufToFloatPtrVec(audio,buffer);
			}
		}

		//A series of samples were written
		sl_sample_counter+=buffsize;
	}
	void Chirp::flBufferRequested(vector<float *> * buffer)
	{
		//Check the time, enable chirp writing to buffer
		checkClockAndUpdateWriteFlag();

		//Only write into the buffer if enabled
		if(fl_enabled)
		{
			//Only write if chirping is enabled
			if(chirp_write_enable)
			{
				float * audio = getChirpAudio(fl_sample_counter);
				writeFloatPtrBufToFloatPtrVec(audio,buffer);
			}
		}

		//A series of samples were written
		fl_sample_counter+=buffsize;
	}
	void Chirp::srBufferRequested(vector<float *> * buffer)
	{
		//Check the time, enable chirp writing to buffer
		checkClockAndUpdateWriteFlag();

		//Only write into the buffer if enabled
		if(sr_enabled)
		{
			//Only write if chirping is enabled
			if(chirp_write_enable)
			{
				float * audio = getChirpAudio(sr_sample_counter);
				writeFloatPtrBufToFloatPtrVec(audio,buffer);
			}
		}

		//A series of samples were written
		sr_sample_counter+=buffsize;
	}
	void Chirp::frBufferRequested(vector<float *> * buffer)
	{
		//Check the time, enable chirp writing to buffer
		checkClockAndUpdateWriteFlag();

		//Only write into the buffer if enabled
		if(fr_enabled)
		{
			//Only write if chirping is enabled
			if(chirp_write_enable)
			{
				float * audio = getChirpAudio(fr_sample_counter);
				writeFloatPtrBufToFloatPtrVec(audio,buffer);
			}
		}

		//A series of samples were written
		fr_sample_counter+=buffsize;
	}

};
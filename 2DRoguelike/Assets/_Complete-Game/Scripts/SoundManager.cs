using UnityEngine;
using System.Collections;
using NeuroFuzzy.training;
using NeuroFuzzy.rextractors;
using NeuroFuzzy;
using NeuroFuzzy.membership;

namespace Completed
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource efxSource;					//Drag a reference to the audio source which will play the sound effects.
		public AudioSource musicSource;					//Drag a reference to the audio source which will play the music.
		public static SoundManager instance = null;		//Allows other scripts to call functions from SoundManager.				
		public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.

        private int[] roles = {0, 1, 10, 16, 20, 21, 51, 52, 901, 907, 912, 913, 914, 915, 916, 922, 923, 924, 932, 101, 201, 202, 102, 203, 206, 103, 104, 105, 107, 108, 209, 110, 109, 111, 121, 204, 205, 122, 207, 208, 210, 123, 124, 127, 128, 129, 130, 132, 131, 135, 136, 137, 144, 140, 149, 141, 142, 143, 145, 151, 153, 154, 161, 162, 163, 169, 170, 180, 183, 181, 182, 302, 316, 301, 303, 304, 305, 313, 306, 307, 314, 315, 340, 32, 341, 32, 312, 308, 309, 310, 311, 317, 350, 351, 501, 515, 516, 502, 520, 521, 503, 511, 545, 504, 505, 544, 512, 541, 542, 543, 513, 510, 514, 570, 536, 540, 552, 551, 537, 538, 561, 562};

        double[][] gameParams = new double[20][] {
            //           lvl, hp, food
            new double[] { 1, 103, 5},
            new double[] { 10, 132, 3},
            new double[] { 12, 43, 2},
            new double[] { 15, 25, 1},
            new double[] { 20, 218, 4},
            new double[] { 3, 291, 3},
            new double[] { 20, 286, 2},
            new double[] { 10, 59, 1},
            new double[] { 5, 43, 1},
            new double[] { 6, 103, 3},
            new double[] { 4, 110, 3},
            new double[] { 13, 127, 2},
            new double[] { 13, 11, 3},
            new double[] { 14, 107, 1},
            new double[] { 2, 63, 1},
            new double[] { 3, 249, 4},
            new double[] { 2, 312, 5},
            new double[] { 1, 88, 3},
            new double[] { 2, 239, 5},
            new double[] { 9, 231, 3}
        };

        double[][] musicParams = new double[20][] {

            //  ruletype, rule, seed, tempo, pitch, r1, r2, r3
            new double[] { 15, 4124, 16064, 69, 50, 101, 10, 302 },
            new double[] { 7, 30, 14017, 60, 50, 901, 0, 0 },
            new double[] { 62, 2781349191, 9050, 123, 36, 104, 505, 907 },
            new double[] { 31, 25950663, 8584, 100, 52, 512, 513, 206 },
            new double[] { 31, 473837441, 12704, 136, 43, 351, 154, 0 },
            new double[] { 31, 571251253, 14684949, 80, 43, 101, 101, 121 },
            new double[] { 31, 680094502, 825553, 92, 45, 901, 0, 0 },
            new double[] { 31, 881591897, 95754, 112, 55, 540, 206, 122 },
            new double[] { 31, 943754315, 15291, 100, 52, 512, 513, 206 },
            new double[] { 31, 1618028417, 14226, 108, 50, 901, 0, 0 },
            new double[] { 31, 1760396040, 12488, 100, 49, 901, 0, 0 },
            new double[] { 31, 2595066377, 12830, 100, 52, 512, 513, 206},
            new double[] { 31, 2595066377, 14740, 100, 52, 512, 513, 206},
            new double[] { 31, 2595066377, 15084, 100, 52, 512, 513, 206},
            new double[] { 31, 2595067401, 14774, 100, 52, 512, 513, 206},
            new double[] { 167, 30148753, 38628639, 72, 42, 101, 101, 121},
            new double[] { 167, 30148753, 58783685, 72, 42, 101, 101, 121},
            new double[] { 167, 30148753, 64756330, 72, 42, 101, 101, 121},
            new double[] { 31, 1113067830, 12841026, 100, 44, 101, 101, 204 },
            new double[] { 167, 29624467, 41050427, 72, 42, 101, 101, 101 }
        };

        ANFIS fis = null;

        void Awake ()
		{
			//Check if there is already an instance of SoundManager
			if (instance == null)
				//if not, set it to this.
				instance = this;
			//If instance already exists:
			else if (instance != this)
				//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
				Destroy (gameObject);
			
			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad (gameObject);
            Backprop bprop = new Backprop(1e-2);
            KMEANSExtractorIO extractor = new KMEANSExtractorIO(5);
            fis = ANFISBuilder<GaussianRule>.Build(gameParams, musicParams, extractor, bprop, 5);
        }
		
		
		//Used to play single sound clips.
		public void PlaySingle(AudioClip clip)
		{
			//Set the clip of our efxSource audio source to the clip passed in as a parameter.
			efxSource.clip = clip;
			
			//Play the clip.
			efxSource.Play ();
		}

        public int getRole(int target, int[] array)
        {
            int currentDifference = 10000;
            int currentNearest = 0;
            for (int i = 1; i < array.Length; i++)
            {
                int diff = array[i] - target;
                if (diff < 0)
                {
                    diff = -diff;
                }
                if (diff < currentDifference)
                {
                    currentDifference = diff;
                    currentNearest = array[i];
                }
            }

            //Debug.Log(currentNearest);
            return currentNearest;
        }

        public void getMusic (double[] levelParam)
        {
            double[] levelMusicParam = fis.Inference(levelParam);
            for (int i=0; i<levelMusicParam.Length; i++)
            {
                Debug.Log(levelMusicParam[i]);
            }
            int step = (int)(levelMusicParam[3]/60)*30;
            string url = 
                "https://www.wolframcloud.com/objects/user-a13d29f3-43bf-4b00-8e9b-e55639ecde19/NKMMusicDownload"+
                "?id=NKM-G-10-"+(int)levelMusicParam[0]+"-"+ (int)levelMusicParam[1] + "-1-"+ (int)levelMusicParam[2] + "-"+step+"-"+
                (int)levelMusicParam[3] + "-4-2773-"+ (int)levelMusicParam[4] + "-0-1-"+ getRole((int)levelMusicParam[5], roles) + "-1-"+ 
                getRole((int)levelMusicParam[6], roles) + "-1-"+ getRole((int)levelMusicParam[7], roles) + "-0-0-0-0-0&form=WAV";
            WWW www = new WWW(url);
            //yield return www
            //musicSource = GetComponent<AudioSource>();
            musicSource.clip = www.GetAudioClip(false, false, AudioType.WAV);
        }
		
        public bool isMusicReady ()
        {
            return musicSource.clip.loadState == AudioDataLoadState.Loaded;
        }

        public void playMusic()
        {
            if (isMusicReady())
            {
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        public void stopMusic ()
        {
            musicSource.Stop();
        }
		
		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void RandomizeSfx (params AudioClip[] clips)
		{
			//Generate a random number between 0 and the length of our array of clips passed in.
			int randomIndex = Random.Range(0, clips.Length);
			
			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			
			//Set the pitch of the audio source to the randomly chosen pitch.
			efxSource.pitch = randomPitch;
			
			//Set the clip to the clip at our randomly chosen index.
			efxSource.clip = clips[randomIndex];
			
			//Play the clip.
			efxSource.Play();
		}
	}
}

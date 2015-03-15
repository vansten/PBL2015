using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TrashSoup
{
    class AudioManager : Singleton<AudioManager>
    {
        #region Variables
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;

        public SoundBank SoundBank { get { return soundBank; } }
        #endregion

        #region Methods
        public AudioManager()
        {

        }

        public void Update()
        {
            audioEngine.Update();
        }

        /// <summary>
        /// 
        /// For each .xap file (XACT project file), the content pipeline generates an .xgs file. For each wave bank within the project,
        /// it generates an .xwb file, and for each sound bank it generates an .xsb file. As far as I know the names are the same like we set 
        /// in XACT project
        /// It is important to save .xap file in our Audio directory.
        /// </summary>
        public void LoadContent()
        {
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
        }

        /// <summary>
        /// 
        /// Audio is played by identifying and playing cues that we created in our XACT project.
        /// </summary>
        /// <param name="cue"></param> Cue from our cue list placed in ResourceManager class
        public void PlayCue(Cue cue)
        {
            cue.Play();
        }
        #endregion

    }
}

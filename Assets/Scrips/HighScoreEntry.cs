using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scrips
{
    [System.Serializable]
    public class HighScoreEntry
    {
        public string playerName;
        public float score;

        public HighScoreEntry(string name, float score)
        {
            this.playerName = name;
            this.score = score;
        }
    }
}

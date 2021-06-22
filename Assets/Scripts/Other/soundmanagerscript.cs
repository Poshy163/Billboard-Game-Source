#region

using UnityEngine;

#endregion

// ReSharper disable All
namespace Other
{
    public class soundmanagerscript:MonoBehaviour
    {
        private static AudioSource source;

        private static AudioClip arrowshoot;

        private static AudioClip enemyshoot;

        private static AudioClip kick;

        private static AudioClip playerhurt;

        private static AudioClip won;

        private static AudioClip dodge;

        private void Start ()
        {
            source = GetComponent<AudioSource>();
            arrowshoot = Resources.Load<AudioClip>("arrowshoot");
            enemyshoot = Resources.Load<AudioClip>("newenemyshoot");
            kick = Resources.Load<AudioClip>("kick");
            playerhurt = Resources.Load<AudioClip>("playerhurt");
            won = Resources.Load<AudioClip>("won");
            dodge = Resources.Load<AudioClip>("dodge");
        }

        public static void playsound ( string name )
        {
            if(name == "arrowshoot")
            {
                source.PlayOneShot(arrowshoot);
                return;
            }

            if(name == "enemyshoot")
            {
                source.PlayOneShot(enemyshoot,0.1f);
                return;
            }

            if(name == "kick")
            {
                source.PlayOneShot(kick);
                return;
            }

            if(name == "playerhurt")
            {
                source.PlayOneShot(playerhurt);
                return;
            }

            if(name == "won")
            {
                source.PlayOneShot(won);
                return;
            }

            if(!(name == "dodge"))
            {
                return;
            }

            source.PlayOneShot(dodge);
        }
    }
}
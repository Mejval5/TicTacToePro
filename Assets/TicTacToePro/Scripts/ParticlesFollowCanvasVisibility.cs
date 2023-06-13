using UnityEngine;

namespace TicTacToePro
{
    [ExecuteAlways]
    public class ParticlesFollowCanvasVisibility : MonoBehaviour
    {
        public Canvas DaddyCanvas;
        public CanvasGroup DaddyCanvasGroup;
        public Gradient ParticleGradient;

        ParticleSystem _ps;
        float _visibility = -1f;

        void Start()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_visibility != DaddyCanvasGroup.alpha)
            {
                _visibility = DaddyCanvasGroup.alpha;
                var main = _ps.main;

                var gradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[ParticleGradient.colorKeys.Length];
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    colorKeys[i] = new GradientColorKey(ParticleGradient.colorKeys[i].color, ParticleGradient.colorKeys[i].time);
                }

                GradientAlphaKey[] alphakeys = new GradientAlphaKey[ParticleGradient.alphaKeys.Length];
                for (int i = 0; i < alphakeys.Length; i++)
                {
                    alphakeys[i] = new GradientAlphaKey(_visibility, ParticleGradient.alphaKeys[i].time);
                }

                gradient.SetKeys(colorKeys, alphakeys);
                var startColor = new ParticleSystem.MinMaxGradient(gradient);
                main.startColor = startColor;
            }



            if (!DaddyCanvas.enabled && _ps.isPlaying)
                _ps.Stop();
            if (DaddyCanvas.enabled && !_ps.isPlaying)
                _ps.Play();
        }
    }
}
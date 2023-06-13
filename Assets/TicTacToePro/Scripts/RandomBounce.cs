using System.Collections;
using UnityEngine;

namespace TicTacToePro
{
    public class RandomBounce : MonoBehaviour
    {
        public float BounceDuration = 1;
        public float BounceMaxOffset = 0.2f;

        public float InitDelay = 1;
        public Vector2 BounceRandomTimeSpan = new Vector2(1, 25);
        public AnimationCurve BounceCurve;


        Vector3 _defaultScale;
        Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _defaultScale = transform.localScale;
        }

        void OnDisable()
        {
            StopAllCoroutines();
            transform.localScale = _defaultScale;
            if (_animator != null)
                _animator.enabled = true;
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            StartCoroutine(FirstBounce());
        }

        IEnumerator FirstBounce()
        {
            yield return new WaitForSeconds(InitDelay);
            if (_animator != null)
                _animator.enabled = false;

            StartCoroutine(DoNextBounce());
        }

        IEnumerator DoNextBounce()
        {
            var delay = Random.Range(BounceRandomTimeSpan.x, BounceRandomTimeSpan.y);
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(Bounce());
            FinishBounce();
        }

        IEnumerator Bounce()
        {
            var xTime = Random.Range(0f, BounceDuration / 2f);
            var yTime = Random.Range(0f, BounceDuration / 2f);
            var minOffset = (1f - BounceMaxOffset);
            var maxOffset = (1f + BounceMaxOffset);

            var startScale = transform.localScale;
            var xTarget = Random.Range(startScale.x * minOffset, startScale.x * maxOffset);
            var yTarget = Random.Range(startScale.y * minOffset, startScale.y * maxOffset);
            var targetScale = new Vector3(xTarget, yTarget, transform.localScale.z);

            var time = 0f;
            while (time <= BounceDuration * 3f / 2f)
            {
                var x = Mathf.Clamp((time - xTime) / BounceDuration, 0f, 1f);
                x = BounceCurve.Evaluate(x);
                var y = Mathf.Clamp((time - yTime) / BounceDuration, 0f, 1f);
                y = BounceCurve.Evaluate(y);

                var xScale = Mathf.Lerp(startScale.x, targetScale.x, x);
                var yScale = Mathf.Lerp(startScale.y, targetScale.y, y);

                transform.localScale = new Vector3(xScale, yScale, startScale.z);

                yield return null;
                time += Time.deltaTime;
            }

            transform.localScale = startScale;
        }

        void FinishBounce()
        {
            StartCoroutine(DoNextBounce());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
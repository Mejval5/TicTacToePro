using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TicTacToePro.Game2048
{
    public class Cube2048 : MonoBehaviour
    {
        public MeshRenderer Renderer;
        public float Gravity = 9f;
        public float AppearTime = 1f;
        public float ShootForce;
        public Vector2 LaunchForce;
        public float SiblingForce;
        public float RotateForce;
        public float ScaleMaxBounce = 1.35f;
        public float BounceTime = 0.15f;
        public float BounceDelay = 0.5f;
        public GameObject Aimer;
        public int Value;
        public TextMeshProUGUI[] ValueTexts;
        public Settings2048 Settings;
        public ParticleSystem[] Particles;

        Rigidbody _rigidBody;
        Vector3 _targetScale;
        Game2048 _game;
        bool _matched = false;
        System.Guid _scaleID;

        private Coroutine _scaleCoroutine;

        void OnValidate()
        {
            EnsureAllComponents();
        }

        void Awake()
        {
            _targetScale = transform.localScale;
            EnsureAllComponents();
            UpdateValue(Value);
        }

        public void SetKinematic(bool toggle)
        {
            _rigidBody.isKinematic = toggle;
            if (toggle)
                _rigidBody.interpolation = RigidbodyInterpolation.None;
            else
                _rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void Init(Game2048 game)
        {
            _game = game;
        }

        public virtual void EnsureAllComponents()
        {
            this.EnsureComponent(ref _rigidBody);
        }

        void FixedUpdate()
        {
            var force = Vector3.down * Gravity * Time.fixedDeltaTime;
            _rigidBody.AddForce(force, ForceMode.Force);
        }

        public void Shoot()
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }

            transform.localScale = _targetScale;
            _rigidBody.isKinematic = false;
            Aimer.SetActive(false);
            var force = Vector3.forward * ShootForce;
            _rigidBody.AddForce(force, ForceMode.VelocityChange);
        }

        void OnEnable()
        {
            Appear();
        }

        void Appear()
        {
            transform.localScale = Vector3.zero;
            _scaleCoroutine = StartCoroutine(ScaleAnimationAppear(transform, _targetScale, AppearTime));
        }

        IEnumerator ScaleAnimationAppear(Transform trans, Vector3 targetScale, float time)
        {
            // do the animation with inoutquad, loop back to the original scale
            float t = 0f;
            Vector3 startScale = trans.localScale;
            while (t < time)
            {
                t += Time.unscaledDeltaTime;
                float lerpT = Mathf.Sin(t / (time) * Mathf.PI * 0.5f);
                trans.localScale = Vector3.Lerp(startScale, targetScale, lerpT);
                yield return null;
            }

            trans.localScale = targetScale;

            yield return null;
        }

        IEnumerator ScaleAnimationLoop(Transform trans, float targetScaleFloat, float time, int loops, float delay)
        {
            float delayT = 0f;

            while (delayT < delay)
            {
                delayT += Time.unscaledDeltaTime;
                yield return null;
            }

            // do the animation with inoutquad, loop back to the original scale
            float t = 0f;
            Vector3 startScale = trans.localScale;
            Vector3 targetScale = startScale * targetScaleFloat;
            while (t < time * loops)
            {
                t += Time.unscaledDeltaTime;
                float lerpT = Mathf.Sin(t / (time) * Mathf.PI * 0.5f);
                trans.localScale = Vector3.Lerp(startScale, targetScale, lerpT);
                yield return null;
            }

            trans.localScale = startScale;

            yield return null;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (_game.IsPlaying == false)
                return;

            if (collision.gameObject.CompareTag("Cube") == false)
                return;

            var otherCube = collision.gameObject.GetComponent<Cube2048>();
            if (otherCube == null)
                return;

            if (otherCube.Value == Value && otherCube._matched == false)
            {
                _matched = true;
                VibrationsManager.shared.Vibrate();
                _game.PlayMatchSound();
                List<Cube2048> cubes = new List<Cube2048>() { this, otherCube };
                var cubeToRemove = cubes.GetRandom(this);
                cubes.Remove(cubeToRemove);
                var upgradeCube = cubes[0];
                cubeToRemove.KillThis();
                var newVal = upgradeCube.Value * 2;
                _game.AddScore(newVal);
                if (Settings.PossibleValues.Contains(newVal) == false)
                {
                    upgradeCube.KillThis();
                    return;
                }

                upgradeCube.UpdateValue(newVal);
                upgradeCube.Launch();
            }
        }

        public void KillThis()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
                _game.ActiveCubes.Remove(this);
            }
        }

        public void UpdateValue(int value)
        {
            Value = value;
            UpdateValue();
            UpdateColor();
        }

        void UpdateColor()
        {
            int index = Settings.PossibleValues.IndexOf(Value);
            var color = Settings.CubeColors[index];
            Renderer.material.color = color;
        }


        void UpdateValue()
        {
            foreach (var text in ValueTexts)
            {
                float value = Value;
                string valueText = Value.ToString();
                if (value > 4000)
                {
                    value /= 1000f;
                    valueText = Mathf.FloorToInt(value).ToString() + "K";
                }

                text.text = valueText;
            }
        }

        [Button(nameof(Launch))] public bool launch;

        public void Launch()
        {
            if (gameObject == null)
                return;

            _matched = true;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            //transform.DOShakeScale(1f);
            var forceToSibling = DirToSibling() * SiblingForce;
            var force = Vector3.up * Random.Range(LaunchForce.x, LaunchForce.y);
            _rigidBody.AddForce(force + forceToSibling, ForceMode.VelocityChange);

            transform.localScale = _targetScale;
            _scaleCoroutine = StartCoroutine(ScaleAnimationLoop(transform, ScaleMaxBounce, BounceTime, 2, BounceDelay));

            StartCoroutine(DelayMatching(0.25f));

            var rotateForce = Random.insideUnitCircle * RotateForce;
            _rigidBody.AddTorque(rotateForce);

            foreach (var ps in Particles)
            {
                var main = ps.main;
                int index = Settings.PossibleValues.IndexOf(Value);
                var color = Settings.CubeColors[index];
                main.startColor = color;
                ps.Play();
            }
        }

        IEnumerator DelayMatching(float time)
        {
            yield return new WaitForSeconds(time);
            _matched = false;
        }

        Vector3 DirToSibling()
        {
            var randomDir = Random.insideUnitCircle / 10f;
            Vector3 dir = new Vector3(randomDir.x, 0f, randomDir.y);
            float distanceClosest = float.MaxValue;
            foreach (var cube in _game.ActiveCubes)
            {
                if (cube.Value != Value)
                    continue;
                if (cube.gameObject == gameObject)
                    continue;


                var distanceVector = cube.transform.position - transform.position;
                if (distanceClosest < distanceVector.magnitude)
                    continue;

                dir = distanceVector;
                distanceClosest = distanceVector.magnitude;
            }

            return dir;
        }
    }
}
using UnityEngine;

namespace TicTacToePro
{
    public class GameScreenGO : GameScreen
    {
        public override void EnsureAllComponents()
        {
            this.EnsureComponent<Animator>(ref _animator);
        }

        public override void ShowSelf()
        {
            GetAnimator.SetTrigger(ScreenTransition.Show.ToString());
            if (!Application.isPlaying)
                transform.GetChild(0).gameObject.SetActive(true);
            else
                StartCoroutine(DelayAction(OnShow, TransitionTime));
        }


        public override void HideSelf()
        {
            GetAnimator.SetTrigger(ScreenTransition.Hide.ToString());
            if (!Application.isPlaying)
                transform.GetChild(0).gameObject.SetActive(false);
            else
                StartCoroutine(DelayAction(OnHide, TransitionTime));
        }



        public override void IdleSelf()
        {
            GetAnimator.SetTrigger(ScreenTransition.Idle.ToString());
            if (!Application.isPlaying)
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
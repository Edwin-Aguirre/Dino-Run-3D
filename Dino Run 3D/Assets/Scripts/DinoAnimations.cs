using UnityEngine;

public class DinoAnimations : MonoBehaviour
{
    [SerializeField]
    private Animator volume;

    [SerializeField]
    private Animator fullscreen;

    [SerializeField]
    private Animator windowed;

    [SerializeField]
    private Animator resetScore;

    [SerializeField]
    private RuntimeAnimatorController resetScoreController;

    [SerializeField]
    private RuntimeAnimatorController resetShakeController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayVolumeAnimation()
    {
        volume.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void StopVolumeAnimation()
    {
        volume.Play("Volume", -1, 0f);
    }

    public void PlayFullscreenAnimation()
    {
        fullscreen.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void StopFullscreenAnimation()
    {
        fullscreen.Play("Fullscreen", -1, 0f);
    }

    public void PlayWindowedAnimation()
    {
        windowed.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void StopWindowedAnimation()
    {
        windowed.Play("Windowed", -1, 0f);
    }

    public void PlayResetScoreAnimation()
    {
        resetScore.runtimeAnimatorController = resetScoreController;
        resetScore.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void StopResetScoreAnimation()
    {
        resetScore.Play("Reset Score", -1, 0f);
    }

    public void PlayResetScoreShakeAnimation()
    {
        resetScore.runtimeAnimatorController = resetShakeController;
        resetScore.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void StopResetScoreShakeAnimation()
    {
        resetScore.Play("Reset Score Shake", -1, 0f);
    }
}

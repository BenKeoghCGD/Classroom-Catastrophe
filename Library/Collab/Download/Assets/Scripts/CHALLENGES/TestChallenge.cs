using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestChallenge : Challenge
{
    public GameObject Player;
    public ParticleSystem PS;
    public CanvasGroup popup;
    public CanvasGroup poup;
    public Image over;

    //--

    public GameObject Hat;
    public GameObject otherStudent;
    public Scan sc;
    public CanvasGroup pb;

    [Header("SCRIPTED EVENT")]
    public GameObject teacher;
    public AudioClip yawn, PhoneSound;

    protected override void Main()
    {
        PS.Stop();
        Finalized = false;
        p_Completed = 50;
        p_Failure = -40;
        p_Hidden = 15;

        t_Sender = "Adam";
        t_Message = "I dare you to steal Gaz' hat!";
    }

    public override void Initialize()
    {
        poup.alpha = 0;
        popup.alpha = 0;
        popup.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -190, 0);
        pb.alpha = Mathf.Lerp(0, 1, zeroOne);
        pb.gameObject.GetComponent<RectTransform>().position = new Vector3(-291, Mathf.Lerp(-50, 0, zeroOne), 0f);
    }

    public override void Prompt()
    {
        StartCoroutine(prompt());
    }

    IEnumerator prompt()
    {
        SoundManager.AddSound(PhoneSound);
        SoundManager.ChangeVolume(PhoneSound, 1.5f);
        yield return null;
        for(float f = 0; Mathf.Clamp(f,0,1) != 1; f += (Time.deltaTime * 2))
        {
            popup.alpha = Mathf.Clamp(f, 0, 1);
            popup.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, Mathf.Lerp(popup.gameObject.GetComponent<RectTransform>().localPosition.y, -70, Mathf.Clamp(f, 0, 1)), 0);
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        for (float f = 1; Mathf.Clamp(f, 0, 1) != 0f; f -= (Time.deltaTime * 2))
        {
            popup.alpha = Mathf.Clamp(f, 0, 1);
            popup.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, Mathf.Lerp(popup.gameObject.GetComponent<RectTransform>().localPosition.y, -190, Mathf.Clamp(f, 0, 1)), 0);
            yield return null;
        }
        popup.alpha = 0;
    }

    bool pup = false;
    IEnumerator pop()
    {
        pup = true;
        yield return null;
        for (float f = 0; Mathf.Clamp(f, 0, 1) != 1; f += (Time.deltaTime * 2))
        {
            poup.alpha = Mathf.Clamp(f, 0, 1);
            yield return null;
        }
        yield return new WaitForSeconds(10f);
        for (float f = 1; Mathf.Clamp(f, 0, 1) != 0; f -= (Time.deltaTime * 2))
        {
            poup.alpha = Mathf.Clamp(f, 0, 1);
            yield return null;
        }
        poup.alpha = 0;
    }

    bool close = false;
    float zeroOne = 0f;
    float fill = 0.5f;

    bool complete = false;
    bool alert = false;

    void scan()
    {
        StartCoroutine(sc.scan(1, 1, 0.03f, true));
    }

    bool se = false;
    public override void Check(AbilityHitDetector.ChallengeStates state, GameObject player)
    {
        if(state != AbilityHitDetector.ChallengeStates.NONE)
        {
            if (!PS.isPlaying)
            {

                PS.Play();
                SoundManager.AddSound(yawn);
            }
            if (Vector3.Distance(player.transform.position, otherStudent.transform.position) <= 6) close = true;
            else close = false;

            if (fill >= 1)
            {
                Destroy(Hat.gameObject);
                close = false;
                complete = true;
                alert = false;
            }
            else if (fill <= 0)
            {
                Destroy(Hat.gameObject);
                close = false;
                complete = true;
                alert = true;
            }

            if(fill >= 0.75f && !se)
            {
                se = true;
                scan();
            }

            if(!complete)
            {
                if (close) zeroOne += Time.deltaTime * 2;
                else zeroOne -= Time.deltaTime * 2;

                zeroOne = Mathf.Clamp(zeroOne, 0, 1);
            }
            else
            {
                zeroOne -= Time.deltaTime * 2;
            }
            
            pb.alpha = Mathf.Lerp(0, 1, zeroOne);
            pb.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, Mathf.Lerp(-50, 20, zeroOne), 0f);

            if (close && !complete)
            {
                if (!pup) StartCoroutine(pop());
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q)) fill += 0.01f;
                else fill -= 0.0005f;
                over.fillAmount = fill;
            }
        }

        if (alert)
        {
            PS.Stop();
            setState(cStates.ALERT);
        }
        else if (complete)
        {
            StartCoroutine(ScriptedEvent());
            setState(cStates.COMPLETED);
        }

        if ((alert || complete) && pb.alpha == 0 && pb.gameObject.GetComponent<RectTransform>().localPosition == new Vector3(0, -50, 0f)) Finalized = true;
    }

    public override void Hit(AbilityHitDetector.ChallengeStates state, bool hiding)
    {
        if (state != AbilityHitDetector.ChallengeStates.NONE)
        {
            int amnt = 0;

            if (state == AbilityHitDetector.ChallengeStates.HIDDEN || hiding) amnt = p_Hidden;
            else if (state == AbilityHitDetector.ChallengeStates.NONE) amnt = 0;
            else if (state == AbilityHitDetector.ChallengeStates.ONGOING) amnt = p_Failure;

            CloutHandler.alterClout(amnt);
        }
    }

    IEnumerator ScriptedEvent()
    {
        yield return null;
        yield return new WaitForSeconds(1f);
        for(float f = 0; Mathf.Clamp(f,0,1) != 1; f += (Time.deltaTime * 2))
        {

        }
    }
}
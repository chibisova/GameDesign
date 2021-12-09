using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public AudioSource source;

    public AudioClip baseline;
    public AudioClip calm;
    public AudioClip excited;
    public AudioClip stress;
    public AudioClip focus;

    public GameObject Player;
    public GameObject Fog;
    public GameObject CalmParticles;
    public GameObject ExcitedParticles;
    public GameObject StressParticles;
    public GameObject AtmosphereTitle;
    public Text AtmosphereTitleText;

    private CharacterChangeManager Character;

    // Start is called before the first frame update
    void Start()
    {
        Character = Player.GetComponent<CharacterChangeManager>();
        playBase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playBase(){
        source.clip = baseline;
        Character.boostedState = CharacterChangeManager.State.Baseline;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        AtmosphereTitleText.text = "Baseline";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 255, 255, 255);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(false);
    }

    public void playCalm(){
        source.clip = calm;
        Character.boostedState = CharacterChangeManager.State.Calm;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(52, 161, 207, 255);
        AtmosphereTitleText.text = "Relaxed";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(52, 161, 207, 255);
        CalmParticles.SetActive(true);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(false);
    }

    public void playExcited(){
        source.clip = excited;
        Character.boostedState = CharacterChangeManager.State.Excited;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 245, 102, 255);
        AtmosphereTitleText.text = "Excited";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 245, 102, 255);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(true);
        StressParticles.SetActive(false);
    }

    public void playStress(){
        source.clip = stress;
        Character.boostedState = CharacterChangeManager.State.Stress; 
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        AtmosphereTitleText.text = "Stressed";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 0, 0, 255);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(true);
    }

    public void playFocus(){
        source.clip = focus;
        Character.boostedState = CharacterChangeManager.State.Focus;
        source.Play();
        Fog.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 150);
        AtmosphereTitleText.text = "Focused";
        AtmosphereTitle.GetComponent<UnityEngine.UI.Text>().color= new Color32(255, 255, 255, 150);
        CalmParticles.SetActive(false);
        ExcitedParticles.SetActive(false);
        StressParticles.SetActive(false);
    }
}

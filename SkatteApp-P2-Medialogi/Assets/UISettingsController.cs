using UnityEngine;
using UnityEngine.UI;

public class UISettingsController : MonoBehaviour
{
    void Start()
    {
        // Set the sliders to the current volume levels
        _musicSlider.value = AudioManager.Instance.BGSource.volume;
        _sfxSlider.value = AudioManager.Instance.sfxSource.volume;
        
        // Play the default background music
    }
    public Slider _musicSlider, _sfxSlider;

    public void ToogleMusic()
    {
       AudioManager.Instance.ToogleMusic();
    }

    public void ToogleSFX()
    {
        AudioManager.Instance.ToogleSFX();
    }

    public void BgVolume()
    {
        AudioManager.Instance.BgVolume(_musicSlider.value);
    }
    
    public void SfxVolume()
    {
        AudioManager.Instance.SfxVolume(_sfxSlider.value);
    }
    
}

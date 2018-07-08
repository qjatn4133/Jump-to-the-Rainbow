using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;


public class SettingPopup : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider masterSFXSlider;

    public LoadoutState loadoutState;
    //public DataDeleteConfirmation confirmationPopup;

    [Header("Language")]
    public Text title;
    public Text masterVolume;
    public Text musicVolume;
    public Text sfxVolume;
    //public Toggle koreanToggle;
    public Button koreanButton;
    public Color koreanButtonColor;

    protected float m_MasterVolume;
    protected float m_MusicVolume;
    protected float m_MasterSFXVolume;

    protected const float k_MinVolume = -80f;
    protected const string k_MasterVolumeFloatName = "MasterVolume";
    protected const string k_MusicVolumeFloatName = "MusicVolume";
    protected const string k_MasterSFXVolumeFloatName = "MasterSFXVolume";

    public void Open()
    {
        gameObject.SetActive(true);        
        UpdateUI();
    }

    public void Close()
    {
        PlayerData.instance.Save();
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        mixer.GetFloat(k_MasterVolumeFloatName, out m_MasterVolume);
        mixer.GetFloat(k_MusicVolumeFloatName, out m_MusicVolume);
        mixer.GetFloat(k_MasterSFXVolumeFloatName, out m_MasterSFXVolume);

        masterSlider.value = 1.0f - (m_MasterVolume / k_MinVolume);
        musicSlider.value = 1.0f - (m_MusicVolume / k_MinVolume);
        masterSFXSlider.value = 1.0f - (m_MasterSFXVolume / k_MinVolume);

        if (PlayerData.instance.koreanCheck)
        {
            koreanButton.image.color = koreanButtonColor;
        }
        else
        {
            koreanButton.image.color = Color.grey;
        }

        //Language Text
        if (!PlayerData.instance.koreanCheck)
        {
            title.text = "SETTING";
            masterVolume.text = "MASTER";
            musicVolume.text = "MUSIC";
            sfxVolume.text = "SFX";
        }
        else
        {
            title.text = "설 정";
            masterVolume.text = "전체 음량";
            musicVolume.text = "배경 음악";
            sfxVolume.text = "효과음";
        }

    }

    public void ChangeTutorialCheck()
    {
        if (PlayerData.instance.tutorialCheck)
        {
            PlayerData.instance.tutorialCheck = false;

        }
        else
        {
            PlayerData.instance.tutorialCheck = true;
        }
    }

    public void ChangeKoreanCheck()
    {
        if (PlayerData.instance.koreanCheck)
        {
            PlayerData.instance.koreanCheck = false;
        }
        else
        {
            PlayerData.instance.koreanCheck = true;

        }
    }

    /*
    public void DeleteData()
    {
        confirmationPopup.Open(loadoutState);
    }
    */

    public void MasterVolumeChangeValue(float value)
    {
        m_MasterVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MasterVolumeFloatName, m_MasterVolume);
        PlayerData.instance.masterVolume = m_MasterVolume;
    }

    public void MusicVolumeChangeValue(float value)
    {
        m_MusicVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MusicVolumeFloatName, m_MusicVolume);
        PlayerData.instance.musicVolume = m_MusicVolume;
    }

    public void MasterSFXVolumeChangeValue(float value)
    {
        m_MasterSFXVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MasterSFXVolumeFloatName, m_MasterSFXVolume);
        PlayerData.instance.masterSFXVolume = m_MasterSFXVolume;
    }

    public void LoadPlayerData()
    {
        PlayCloudDataManager.Instance.LoadFromCloud((string starsDataToLoad) => { PlayerData.instance.stars = int.Parse(starsDataToLoad); });
        PlayCloudDataManager.Instance.LoadFromCloud((string coinsDataToLoad) => { PlayerData.instance.coins = int.Parse(coinsDataToLoad); });

        //PlayCloudDataManager.Instance.LoadFromCloud((string skinsDataToLoad) => { PlayerData.instance.skins = List<string>.Parse(skinsDataToLoad); });

    }

    public void SavePlayerData()
    {
        PlayCloudDataManager.Instance.SaveToCloud(PlayerData.instance.stars.ToString());
        PlayCloudDataManager.Instance.SaveToCloud(PlayerData.instance.coins.ToString());
    }
}

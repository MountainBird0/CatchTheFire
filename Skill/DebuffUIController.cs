using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.ParticleSystem;

public class DebuffUIController : NetworkBehaviour
{
    public static DebuffUIController Instance;
    public List<Image> distractImg;
    public List<ParticleSystem> particles;

    private void Awake()
    {
        Instance = this;

        foreach (var image in distractImg)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void ActivateDistractionImages()
    {
        distractImg[0].gameObject.SetActive(true);
        particles[0].Play();

        Sequence sequence = DOTween.Sequence();
        //sequence.Append(distractImg[0].DOFade(1, 1f));
        particles[0].Play();
        sequence.AppendInterval(5f);
        //sequence.Append(distractImg[0].DOFade(0, 1f));
        sequence.OnComplete(() =>
        {   particles[0].Stop();
            distractImg[0].gameObject.SetActive(false);
        });
    }
}

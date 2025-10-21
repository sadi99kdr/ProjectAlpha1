using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;


public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    public Sprite hiddenIconSprite;
    public Sprite iconSprite;

    public bool isSelected;
    private Animator animator;

    [Header("Sounds")]
    private AudioSource _audioSource;


    public GameManager gameManager;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
       // HideImmediate();
    }

    private void HideImmediate()
    {
        iconImage.sprite = hiddenIconSprite;
        isSelected = false;
    }
    public void SetIconsSprite(Sprite sp)
    {
        iconSprite = sp;
    }

    public void ShowIcon()
    {


        Tween.Rotation(transform, new Vector3(0f, 180f, 0f), 0.2f);
        Tween.Delay(0.1f, () =>

        iconImage.sprite = iconSprite);

        //animator.SetBool("isFlipped", true);
        isSelected = true;
    }
    public void HideIcon() 
    {
        Tween.Rotation(transform, new Vector3(0f, 0f, 0f), 0.2f);
        Tween.Delay(0.1f, () =>
        {
            iconImage.sprite = hiddenIconSprite;
            isSelected = false;
        });
       // animator.SetBool("isFlipped", false);
         
    }

    public void OnCardClick()
    {
        gameManager.SetSelected(this);
        if (_audioSource != null && _audioSource != null)
        {
            _audioSource.Play();
        }
        Debug.Log("Clicked");
    }
}

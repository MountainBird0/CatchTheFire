using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SpriteAnimation : NetworkBehaviour
{
    public SpriteRenderer spriteRenderer;

    public AnimationData[] animations;
    private AnimationData currentAnimation;

    [Header("Auto Start")]
    private int frameCount = 0;
    [HideInInspector] public bool isPlaying = false;

    [SerializeField] private float animationSpeed = 1; //Animation Frame Speed 

    private int currentFrame = 0;
    private int targetPlayTime = 0;
    private float animationTimer = 0;

    private void Awake()
    {
        isPlaying = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (isPlaying && currentAnimation != null)
        {
            animationTimer += Time.deltaTime * animationSpeed;

            if (animationTimer >= 1f / (float)currentAnimation.frame)
            {
                animationTimer = 0;

                if (frameCount >= currentAnimation.sprites.Length)
                {
                    if (!currentAnimation.loop) // Not Loop Animation
                    {
                        isPlaying = false;
                    }

                    if (targetPlayTime != 0)
                    {
                        currentFrame++;
                        if (currentFrame >= targetPlayTime)
                        {
                            isPlaying = false;
                            currentFrame = 0;
                            targetPlayTime = 0;
                        }
                    }
                    frameCount = 0;
                }

                ChangeSpriteServerRpc(frameCount);
                frameCount++;
            }
        }
    }

    [ServerRpc]
    private void ChangeSpriteServerRpc(int count)
    {
        // Broadcast to all clients to change the sprite
        ChangeSpriteClientRpc(count);
    }

    [ClientRpc]
    private void ChangeSpriteClientRpc(int count)
    {
        if (currentAnimation != null && count < currentAnimation.sprites.Length)
        {
            spriteRenderer.sprite = currentAnimation.sprites[count];
        }
    }

    [ServerRpc]
    public void PlayAnimationServerRpc(string stateAnimationName, Vector3 dir)
    {
        PlayAnimationClientRpc(stateAnimationName, dir);
    }

    [ClientRpc]
    public void PlayAnimationClientRpc(string stateAnimationName, Vector3 dir)
    {
        currentAnimation = Array.Find(animations, x => x.animationState == stateAnimationName);

        if (currentAnimation != null)
        {
            currentFrame = 0;
            animationTimer = 0f;
            isPlaying = true;
            // spriteRenderer.flipX = dir.x > 0 ? true : false;
            spriteRenderer.sprite = currentAnimation.sprites[currentFrame];
        }
    }

    [ServerRpc]
    public void SwitchFlipServerRpc(Vector3 dir)
    {
        SwitchFlipClientRpc(dir);
    }
    [ClientRpc]
    private void SwitchFlipClientRpc(Vector3 dir)
    {
        spriteRenderer.flipX = dir.x > 0 ? true : false;
    }
    public bool IsPlaying(string stateAnimationName)
    {
        return currentAnimation != null && currentAnimation.animationState == stateAnimationName;
    }


    //TODO : Add Animation Code

    public float hideTime = 1f;
    public Vector3 growScale;
    private bool activeFalse = false;
    private float frameTime;
    bool isSprite = true;
    public bool autoStart = false;


    [Header("Sprites")]
    public Sprite[] _sprites;

    private void OnEnable()
    {
        if (autoStart) Play();
    }
    public void SetAutoActiveFalse()
    {
        activeFalse = true;
    }

    public void ChangeFrame(int frame)
    {
        frameTime = 1f / (float)frame;
    }

    public void PlayCount(int count)
    {
        targetPlayTime = count;

        if (!gameObject.activeSelf) return;
        if (_sprites.Length == 0) return;

        frameCount = 0;
        isPlaying = true;
    }

    public void Stop(bool reset = true)
    {
        isPlaying = false;
        if (reset) ResetFrame();
    }

    public void Play()
    {
        if (_sprites.Length == 0) return;

        frameCount = 0;
        isPlaying = true;
    }

    public void Play(UnityAction unityEv)
    {
        if (_sprites.Length == 0) return;
        //completeEvent = unityEv;

        frameCount = 0;
        isPlaying = true;
    }




    public void ResetFrame()
    {
        frameCount = 0;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = _sprites[frameCount];
        }
    }
}
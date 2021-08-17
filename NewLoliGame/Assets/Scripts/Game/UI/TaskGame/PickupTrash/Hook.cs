using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class Hook : MonoBehaviour
{
    BoxCollider2D boxCollider;
    Rigidbody2D rigidbody;
    LineRenderer lineRenderer;
    Vector2 point;
    bool isStart;
    bool isBack;
    float speed = 2f;
    float backSpeed = 250f;
    Material mat;
    AudioClip audioClip;
    AudioClip audioBadClip;
    AudioClip audioRopeClip;
    AudioSource audioSource;
    public void Init()
    {
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector2(54.76f, 47);
        boxCollider.offset = new Vector2(29.1f, -21.4f);
        point = transform.localPosition;
        mat = Resources.Load<Material>("Game/GFX/Materials/shengzi");

        Debug.Log(Mathf.Sin(transform.rotation.z));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.SetPosition(0, new Vector3(transform.position.x+0.14f, transform.position.y+0.15f, 10));
        lineRenderer.SetPosition(1, new Vector3(transform.position.x + Mathf.Cos(transform.rotation.z) * 0.18f, transform.position.y + Mathf.Sin(transform.rotation.z) * 0.18f, 10));
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.material = mat;
        lineRenderer.sortingOrder = 200;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Throw()
    {
        if (!isStart && !isBack)
        {
            isStart = true;
            lineRenderer.gameObject.SetActive(true);
            EventMgr.Ins.DispachEvent(EventConfig.GAME_THROW);
            StartCoroutine(AudioRopeClip());
        }
    }
    IEnumerator AudioRopeClip()
    {
        if ((isStart||isBack)&& !audioSource.isPlaying)
        {
            if (audioRopeClip == null)
                audioRopeClip = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.PICK_ROPE)) as AudioClip;
            audioSource.PlayOneShot(audioRopeClip);
        }
        else
        {

            yield return 0;
        }
        yield return new WaitForSeconds(audioRopeClip.length);
        StartCoroutine(AudioRopeClip());
    }
    private void Update()
    {
        if (isStart)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            
        }
        else if (isBack)
        {
            float speed = backSpeed;
            if (config != null)
            {
                speed /= int.Parse(config.ckey);
            }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, point, speed * Time.deltaTime);
            if (transform.localPosition.y >= point.y)
            {
               
                isBack = false;

                audioSource.Stop();



                if (barrier != null)
                {
                    barrier.gameObject.SetActive(false);
                    if (config.type == 1)
                    {
                        if (audioClip == null)
                            audioClip = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Pickup)) as AudioClip;
                        GRoot.inst.PlayEffectSound(audioClip);
                    }
                    else
                    {
                    
                        if (audioBadClip == null)
                            audioBadClip = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.PICK_ROCK)) as AudioClip;
                        GRoot.inst.PlayEffectSound(audioBadClip);
                    }
                    
                   
                }
                EventMgr.Ins.DispachEvent(EventConfig.GAME_THROW_BACK, config);
                
            }
        }
        if (isStart|| isBack)
            lineRenderer.SetPosition(1, new Vector3(transform.position.x + Mathf.Cos(transform.rotation.z) * 0.18f, transform.position.y + Mathf.Sin(transform.rotation.z) * 0.18f, 10));
     
       
    }

    GameXinglingDlllConfig config;
    Barrier barrier;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStart)
        {
            barrier = collision.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.transform.parent = transform;
                config = barrier.config;
               
                collision.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, 10);
                collision.gameObject.transform.rotation = transform.rotation;
            }
            else
            {
                config = null;
            }
            isStart = false;
            isBack = true;
            EventMgr.Ins.DispachEvent(EventConfig.GAME_THROW_GET);
        }
    }

}

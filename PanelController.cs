using System.Collections;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour{
    
    [SerializeField]                    bool        hideOnAwake = true;
    [SerializeField, Range(.5f, 10f)]   float       speed       = 5f;
    [SerializeField]                    CanvasGroup group;

   

    public delegate void Begin();
    public delegate void Complete();
    public event Begin      OnBegin;
    public event Complete   OnComplete;

    public bool Closed => group.alpha<=0f;
    public bool Locked => !group.interactable;

    void Awake() {
        if(group==null){
            group = GetComponent<CanvasGroup>();
        }
        if(hideOnAwake){
            Close(true);
        }else{
            Open(true);
        }
    }

    public void Open(bool instant=false){        
        gameObject.SetActive(true); 
        StopAllCoroutines();
        if(instant){
            SetInstant(true);
            return;
        }
        
        StartCoroutine(Fade(true, ()=>{
            group.interactable=true;
        }));
    }

    public void Close(bool instant=false){
        gameObject.SetActive(true); 
        StopAllCoroutines();
        if(instant){
            SetInstant(false);
            return;
        }
        
        StartCoroutine(Fade(false, ()=>{
            group.interactable      = false;
            group.blocksRaycasts    = false;
            gameObject.SetActive(false); 
        }));
    }

    public void Lock(bool locked=true) => group.interactable=locked;
    
    IEnumerator Fade(bool fadeIn, Action internalComplete=null){
        // set up for fade actions
        int dir                 = fadeIn ? 1 : -1;
        float target            = fadeIn ? 1f : 0f;
        group.interactable      = false;
        group.blocksRaycasts    = true;

        // brodcast start of fade
        OnBegin?.Invoke(); 
        // run the fade
        while(group.alpha != target){
            group.alpha = Mathf.Clamp( (group.alpha + (Time.deltaTime*speed)*dir), 0f, 1f);
            yield return null;
        }
        // internal complete actions
        internalComplete?.Invoke();
        // brodcast end
        OnComplete?.Invoke(); 
    }

    void SetInstant(bool state){
        group.alpha             = state?1:0;
        group.blocksRaycasts    = state;
        group.interactable      = state;
        gameObject.SetActive(state); 
    }

    void OnValidate() {
        if(group==null){
            group = GetComponent<CanvasGroup>();
        }
    }
}

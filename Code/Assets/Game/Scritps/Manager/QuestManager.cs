using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class QuestManager : MonoBehaviour
{    
    public bool isStart = false;
    public int questID = 0;
    PlayableDirector playTimeLine;
    public bool isTimeline = false;

    float time;
    float maxTimeCheck = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void StartQuest(){        
        LoadQuestObject();
        PlayTip();
        isStart = true;
    }

    void LoadQuestObject(){
        if(DataHolder.Quest(questID).prefabName != ""){
            string url = DataHolder.GetQuest(questID).prefabName.Replace("Assets/Game/Resources/","");
            url = url.Replace(".prefab",""); 
            url = DataHolder.Quests().GetPrefabPath() + url;
            GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject),new Vector3(0,0,-200),Quaternion.identity) as GameObject;		
            playTimeLine = go.GetComponent<PlayableDirector>();
        }
    }

    void PlayTip(){
        OnQuestNotification();
        if(playTimeLine != null){
            StartCoroutine(PlayTimeline());
        }
    }

    IEnumerator PlayTimeline(){
        
        playTimeLine.gameObject.SetActive(true);
        playTimeLine.Play();
        Debug.Log(playTimeLine.duration);
        yield return new WaitForSeconds((float)playTimeLine.duration);
        playTimeLine.Stop();
        playTimeLine.gameObject.SetActive(false);
    }

    public void ResetQuest(){
        playTimeLine.time = 0;
        PlayTip();
    }

    public void CompleteQuest(){
        questID ++;
        isTimeline = false;
        isStart = false;
    }

    public void CheckQuest(){

    }

    // Update is called once per frame
    void Update()
    {
        if(!isStart){
            StartQuest();
        }else{
            if(time > maxTimeCheck){
                CheckQuest();
                time = 0;
            }else
                time +=  Time.deltaTime;
        }
        
    }

    public void OnQuestNotification(){
        UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(DataHolder.Quest(questID).dialogId).GetDescription(0));
    }   
}

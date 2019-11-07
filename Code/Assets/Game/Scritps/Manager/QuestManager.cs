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
    System.DateTime startTime;
    float time;
    float maxTimeCheck = 0.2f;
    MPopup tipUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void StartQuest(){        
        LoadQuestObject();
        PlayTip();
        isStart = true;
        startTime = System.DateTime.Now;
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
        if(tipUI != null)
            tipUI.Close();
    }

    public void ResetQuest(){
        playTimeLine.time = 0;
        PlayTip();
    }

    public void CompleteQuest(){
        questID ++;
        isTimeline = false;
        isStart = false;
        Debug.Log("Quest Complete");
    }

    public void CheckQuest(){
        int count = 0;
        int check = DataHolder.Quest(questID).requirements.Length;
        for(int i=0;i<DataHolder.Quest(questID).requirements.Length;i++){
            if(DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Action){
                List<ActionData> actions = GameManager.instance.GetActionLogs(startTime);
                if(actions != null){
                    foreach(ActionData a in actions){
                        if(a.actionType == DataHolder.Quest(questID).requirements[i].actionType){
                            count ++;
                        }
                    }
                }
            }else if(DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Skill){

            }else if(DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Interact){
                
            }else if(DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Variable){
                
            }
        }
        //Debug.Log(count +"   "+  check);
        if(count >= check){
            CompleteQuest();
        }

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
        tipUI = UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(DataHolder.Quest(questID).dialogId).GetDescription(0));
    }   
}

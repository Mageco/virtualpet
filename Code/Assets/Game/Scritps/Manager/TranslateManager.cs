using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MageSDK.Tools.Google;
using MageApi;

public class TranslateManager : MonoBehaviour
{
    public static TranslateManager instance;
    public bool isResetItem = false;
    int loadCount = 0;
    int loadProgress = 0;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        DataHolder.Instance().Init();


        //Dialog 
        for(int i = 0; i < DataHolder.Dialogs().GetDataCount(); i++)
        {
            for (int j = 1; j < DataHolder.Languages().GetDataCount(); j++)
            {
                if(DataHolder.Dialog(i).languageItem.Length < j)
                {
                    DataHolder.Dialog(i).AddLanguageItem();
                }

                //Name
                if(DataHolder.Dialog(i).GetName(j) == "" && DataHolder.Dialog(i).GetName(0) != "")
                {
                    int m = i;
                    int n = j;
                    loadCount++;
                    GoogleTranslationRequest request = new GoogleTranslationRequest(DataHolder.Languages().GetLanguageCode(j), DataHolder.Dialog(i).GetName(0));

                    //call to login api
                    GoogleHelper.instance.SendTranslationApi(
                        request,
                        (result) => {
                                
                            DataHolder.Dialog(m).SetName(n,result.translations[0].translatedText);
                            Debug.Log(m + " " + n + "  " + DataHolder.Dialog(m).GetName(n));
                            loadProgress++;
                            },
                            (errorStatus) => {
                                ApiUtils.Log("Error: " + errorStatus);
                                //do some other processing here
                            },
                            () => {
                                //timeout handler here
                                ApiUtils.Log("Api call is timeout");
                            }
                        );
                    yield return new WaitForEndOfFrame(); 
                }

                //Description
                if (DataHolder.Dialog(i).GetDescription(j) == "" && DataHolder.Dialog(i).GetDescription(0) != "")
                {
                    int m = i;
                    int n = j;
                    loadCount++;
                    GoogleTranslationRequest request = new GoogleTranslationRequest(DataHolder.Languages().GetLanguageCode(j), DataHolder.Dialog(i).GetDescription(0));

                    //call to login api
                    GoogleHelper.instance.SendTranslationApi(
                        request,
                        (result) => {

                            DataHolder.Dialog(m).SetDescription(n, result.translations[0].translatedText);
                            Debug.Log(m + " " + n + "  " + DataHolder.Dialog(m).GetDescription(n));
                            loadProgress++;
                        },
                            (errorStatus) => {
                                ApiUtils.Log("Error: " + errorStatus);
                                //do some other processing here
                            },
                            () => {
                                //timeout handler here
                                ApiUtils.Log("Api call is timeout");
                            }
                        );
                    yield return new WaitForEndOfFrame();
                }
            }
        }


        //Quest
        for (int i = 0; i < DataHolder.Quests().GetDataCount(); i++)
        {
            for (int j = 1; j < DataHolder.Languages().GetDataCount(); j++)
            {
                if (DataHolder.Quest(i).languageItem.Length < j)
                {
                    DataHolder.Quest(i).AddLanguageItem();
                }

                //Description
                if (DataHolder.Quest(i).GetDescription(j) == "" && DataHolder.Quest(i).GetDescription(0) != "")
                {
                    int m = i;
                    int n = j;
                    loadCount++;
                    GoogleTranslationRequest request = new GoogleTranslationRequest(DataHolder.Languages().GetLanguageCode(j), DataHolder.Quest(i).GetDescription(0));

                    //call to login api
                    GoogleHelper.instance.SendTranslationApi(
                        request,
                        (result) => {

                            DataHolder.Quest(m).SetDescription(n, result.translations[0].translatedText);
                            Debug.Log(m + " " + n + "  " + DataHolder.Quest(m).GetDescription(n));
                            loadProgress++;
                        },
                            (errorStatus) => {
                                ApiUtils.Log("Error: " + errorStatus);
                                //do some other processing here
                            },
                            () => {
                                //timeout handler here
                                ApiUtils.Log("Api call is timeout");
                            }
                        );
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        //Achivement 
        for (int i = 0; i < DataHolder.Achivements().GetDataCount(); i++)
        {
            for (int j = 1; j < DataHolder.Languages().GetDataCount(); j++)
            {
                if (DataHolder.Achivement(i).languageItem.Length < j)
                {
                    DataHolder.Achivement(i).AddLanguageItem();
                }

                //Name
                if (DataHolder.Achivement(i).GetName(j) == "" && DataHolder.Achivement(i).GetName(0) != "")
                {
                    int m = i;
                    int n = j;
                    loadCount++;
                    GoogleTranslationRequest request = new GoogleTranslationRequest(DataHolder.Languages().GetLanguageCode(j), DataHolder.Achivement(i).GetName(0));

                    //call to login api
                    GoogleHelper.instance.SendTranslationApi(
                        request,
                        (result) => {

                            DataHolder.Achivement(m).SetName(n, result.translations[0].translatedText);
                            Debug.Log(m + " " + n + "  " + DataHolder.Achivement(m).GetName(n));
                            loadProgress++;
                        },
                            (errorStatus) => {
                                ApiUtils.Log("Error: " + errorStatus);
                                //do some other processing here
                            },
                            () => {
                                //timeout handler here
                                ApiUtils.Log("Api call is timeout");
                            }
                        );
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        //Item 
        for (int i = 0; i < DataHolder.Items().GetDataCount(); i++)
        {
            for (int j = 1; j < DataHolder.Languages().GetDataCount(); j++)
            {
                if (DataHolder.Item(i).languageItem.Length < j)
                {
                    DataHolder.Item(i).AddLanguageItem();
                }

                //Name
                if ((DataHolder.Item(i).GetName(j) == "" || isResetItem) && DataHolder.Item(i).GetName(0) != "")
                {
                    int m = i;
                    int n = j;
                    loadCount++;
                    GoogleTranslationRequest request = new GoogleTranslationRequest(DataHolder.Languages().GetLanguageCode(j), DataHolder.Item(i).GetName(0));

                    //call to login api
                    GoogleHelper.instance.SendTranslationApi(
                        request,
                        (result) => {

                            DataHolder.Item(m).SetName(n, result.translations[0].translatedText);
                            Debug.Log(m + " " + n + "  " + DataHolder.Item(m).GetName(n));
                            loadProgress++;
                        },
                            (errorStatus) => {
                                ApiUtils.Log("Error: " + errorStatus);
                                //do some other processing here
                            },
                            () => {
                                //timeout handler here
                                ApiUtils.Log("Api call is timeout");
                            }
                        );
                    yield return new WaitForEndOfFrame();
                }

                //Description
                if ((DataHolder.Item(i).GetDescription(j) == "" || isResetItem) && DataHolder.Item(i).GetDescription(0) != "")
                {
                    int m = i;
                    int n = j;
                    loadCount++;
                    GoogleTranslationRequest request = new GoogleTranslationRequest(DataHolder.Languages().GetLanguageCode(j), DataHolder.Item(i).GetDescription(0));

                    //call to login api
                    GoogleHelper.instance.SendTranslationApi(
                        request,
                        (result) => {

                            DataHolder.Item(m).SetDescription(n, result.translations[0].translatedText);
                            Debug.Log(m + " " + n + "  " + DataHolder.Item(m).GetDescription(n));
                            loadProgress++;
                        },
                            (errorStatus) => {
                                ApiUtils.Log("Error: " + errorStatus);
                                //do some other processing here
                            },
                            () => {
                                //timeout handler here
                                ApiUtils.Log("Api call is timeout");
                            }
                        );
                    yield return new WaitForEndOfFrame();
                }
            }
        }


        //Check complete 
        Debug.Log("Total " + loadCount);
        while(loadCount > loadProgress)
        {
            Debug.Log("Progress " + loadProgress);
            yield return new WaitForEndOfFrame();
        }

        DataHolder.Dialogs().SaveData();
        DataHolder.Quests().SaveData();
        DataHolder.Achivements().SaveData();
        Debug.Log("All Translate completed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

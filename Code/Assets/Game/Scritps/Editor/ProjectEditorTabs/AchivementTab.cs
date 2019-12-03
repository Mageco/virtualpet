
using UnityEditor;
using UnityEngine;

public class AchivementTab : BaseTab
{

	private GameObject tmpPrefab;
 	private int tempId = 0;
    int lastSellection = -1;
	
	public AchivementTab(ProjectWindow pw) : base(pw)
	{
		this.Reload();
	}
	
	public void ShowTab()
	{
		int tmpSelection = selection;
		EditorGUILayout.BeginVertical();

		// buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Achivement", GUILayout.Width(pw.mWidth)))
		{
			DataHolder.Achivements().AddAchivement();
			selection = DataHolder.Achivements().GetDataCount() - 1;
			GUI.FocusControl("ID");
		}
		this.ShowCopyButton(DataHolder.Achivements());
		if (selection >= 0)
		{
			this.ShowRemButton("Remove Achivement", DataHolder.Achivements());
		}
		this.CheckSelection(DataHolder.Achivements());
		EditorGUILayout.EndHorizontal();

		// color list
		this.AddItemList(DataHolder.Achivements());

		// color settings

		EditorGUILayout.BeginVertical();
		SP2 = EditorGUILayout.BeginScrollView(SP2);
		if (DataHolder.Achivements().GetDataCount() > 0)
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();

			DataHolder.Achivement(selection).order = EditorGUILayout.IntField("Achivement Order", DataHolder.Achivement(selection).order, GUILayout.Width(pw.mWidth));

			for (int i = 0; i < DataHolder.Languages().GetDataCount(); i++)
			{
				if (DataHolder.Achivement(selection).languageItem[i] == null)
					DataHolder.Achivement(selection).AddLanguageItem();

				EditorGUILayout.LabelField(DataHolder.Language(i));
				DataHolder.Achivement(selection).languageItem[i].Name = EditorGUILayout.TextField("Name", DataHolder.Achivement(selection).languageItem[i].Name, GUILayout.Width(pw.mWidth * 2));
				DataHolder.Achivement(selection).languageItem[i].Description = EditorGUILayout.TextField("Description", DataHolder.Achivement(selection).languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
				EditorGUILayout.Separator();
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
			if(DataHolder.Achivement(selection).iconUrl != null){
                this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(DataHolder.Achivement(selection).iconUrl);
            }
			this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
			if (this.tmpSprites != null)
			{
				DataHolder.Achivement(selection).iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

			}
			EditorGUILayout.LabelField(DataHolder.Achivement(selection).iconUrl);
			EditorGUILayout.EndHorizontal();

			if (this.tmpSprites != null)
			{
				if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
				{
					DataHolder.Achivement(selection).iconUrl = "";
					tmpSprites = null;
				}
			}

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			DataHolder.Achivement(selection).achivementType = (AchivementType)EditorGUILayout.EnumPopup("Achivement Type", DataHolder.Achivement(selection).achivementType, GUILayout.Width (pw.mWidth));
			if(DataHolder.Achivement(selection).achivementType == AchivementType.Do_Action){
				DataHolder.Achivement(selection).actionType = (ActionType)EditorGUILayout.EnumPopup("Action Type", DataHolder.Achivement(selection).actionType, GUILayout.Width (pw.mWidth));
			}else if(DataHolder.Achivement(selection).achivementType == AchivementType.Buy_Item || DataHolder.Achivement(selection).achivementType == AchivementType.Use_Item){
				 if(lastSellection != selection)
                    tempId = DataHolder.Items().GetItemPosition(DataHolder.Achivement(selection).itemId);

                    if(tempId == -1)
                        tempId = 0;
                 
                   tempId = EditorGUILayout.Popup("Item",tempId, DataHolder.Items().GetNameList(true), GUILayout.Width(pw.mWidth));

                   if(DataHolder.Item(tempId) != null)
                        DataHolder.Achivement(selection).itemId = DataHolder.Item(tempId).iD;
                   lastSellection = selection;
			}else if(DataHolder.Achivement(selection).achivementType == AchivementType.Eat){
				if(lastSellection != selection)
                    tempId = DataHolder.Items().GetItemPosition(DataHolder.Achivement(selection).itemId,(int)ItemType.Food);

                    if(tempId == -1)
                        tempId = 0;
                 
                   tempId = EditorGUILayout.Popup("Item",tempId, DataHolder.Items().GetNameListFilter((int)ItemType.Food), GUILayout.Width(pw.mWidth));

                   if(DataHolder.Items().GetItem(tempId,(int)ItemType.Food) != null)
                        DataHolder.Achivement(selection).itemId = DataHolder.Items().GetItem(tempId,(int)ItemType.Food).iD;
                   lastSellection = selection;
			}else if(DataHolder.Achivement(selection).achivementType == AchivementType.Drink){
				if(lastSellection != selection)
                    tempId = DataHolder.Items().GetItemPosition(DataHolder.Achivement(selection).itemId,(int)ItemType.Drink);

                    if(tempId == -1)
                        tempId = 0;
                 
                   tempId = EditorGUILayout.Popup("Item",tempId, DataHolder.Items().GetNameListFilter((int)ItemType.Drink), GUILayout.Width(pw.mWidth));

                   if(DataHolder.Items().GetItem(tempId,(int)ItemType.Drink) != null)
                        DataHolder.Achivement(selection).itemId = DataHolder.Items().GetItem(tempId,(int)ItemType.Drink).iD;
                   lastSellection = selection;
			}
			
			else if(DataHolder.Achivement(selection).achivementType == AchivementType.Tap_Animal || DataHolder.Achivement(selection).achivementType == AchivementType.Dissmiss_Animal){
				DataHolder.Achivement(selection).animalType = (AnimalType)EditorGUILayout.EnumPopup("Animal", DataHolder.Achivement(selection).animalType, GUILayout.Width (pw.mWidth));
			}

			 


			EditorGUILayout.Separator();

			if (GUILayout.Button("Add Achivement Level",GUILayout.Width (pw.mWidth))){
				DataHolder.Achivement(selection).maxProgress = ArrayHelper.Add(1,DataHolder.Achivement(selection).maxProgress);
				DataHolder.Achivement(selection).levelDescription = ArrayHelper.Add("",DataHolder.Achivement(selection).levelDescription);
				DataHolder.Achivement(selection).coinValue = ArrayHelper.Add(0,DataHolder.Achivement(selection).coinValue);
				DataHolder.Achivement(selection).diamondValue = ArrayHelper.Add(0,DataHolder.Achivement(selection).diamondValue);
				//Debug.Log(DataHolder.Achivement(selection).maxProgress.Length);
			}

			
			for(int i=0;i<DataHolder.Achivement(selection).maxProgress.Length;i++){
				EditorGUILayout.BeginVertical("box");
				fold2 = EditorGUILayout.Foldout(fold2, "Achivement Level " + (i+1).ToString());
				//EditorGUILayout.LabelField("Achivement Level " + i.ToString(), GUILayout.MaxWidth(110));
				EditorGUILayout.BeginHorizontal();
				DataHolder.Achivement(selection).maxProgress[i] = EditorGUILayout.IntField("Require Number", DataHolder.Achivement(selection).maxProgress[i], GUILayout.Width (pw.mWidth));
				
				if (GUILayout.Button("X",GUILayout.Width (pw.mWidth * 0.2f))){
					DataHolder.Achivement(selection).maxProgress = ArrayHelper.Remove(DataHolder.Achivement(selection).maxProgress.Length - 1,DataHolder.Achivement(selection).maxProgress);
					DataHolder.Achivement(selection).levelDescription = ArrayHelper.Remove(DataHolder.Achivement(selection).levelDescription.Length - 1,DataHolder.Achivement(selection).levelDescription);
					DataHolder.Achivement(selection).coinValue = ArrayHelper.Remove(DataHolder.Achivement(selection).coinValue.Length - 1,DataHolder.Achivement(selection).coinValue);
					DataHolder.Achivement(selection).diamondValue = ArrayHelper.Remove(DataHolder.Achivement(selection).diamondValue.Length - 1,DataHolder.Achivement(selection).diamondValue);
				}
				EditorGUILayout.EndHorizontal();
				
				if(fold2)
				{
					DataHolder.Achivement(selection).levelDescription[i] = EditorGUILayout.TextField("Description", DataHolder.Achivement(selection).levelDescription[i], GUILayout.Width (pw.mWidth));
					DataHolder.Achivement(selection).coinValue[i] = EditorGUILayout.IntField("Coin", DataHolder.Achivement(selection).coinValue[i], GUILayout.Width(pw.mWidth));
					DataHolder.Achivement(selection).diamondValue[i] = EditorGUILayout.IntField("Diamond", DataHolder.Achivement(selection).diamondValue[i], GUILayout.Width(pw.mWidth));
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.Separator();


			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		this.EndTab();
	}
}
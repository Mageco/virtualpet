using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;
using Mage.Models.Game;
using MageApi;

namespace Mage.Models.Users{
	[Serializable]
	public class User : BaseModel
	{
		public string id = "";
		public string uuid = "";
		public string username = "";
		public string password = "";
		public string facebook_id = "";
		public string authentication_method = "0";
		public string fullname = "";
		public string phone = "";
		public string email = "";
		public string avatar = "";
		public UserStatus status = UserStatus.ACTIVE;
		public string notification_token = "";
		public string country_code = "";

		public string last_run_app_version = "";

		public string app_version = "";

		public List<UserData> user_datas = new List<UserData>();

		public List<Character> characters = new List<Character>();

		public List<CharacterItem> character_items = new List<CharacterItem>();

		public User() : base () {
			user_datas = new List<UserData>();
		}
		
		public string GetUserData(string key)
		{
			foreach (UserData u in user_datas) {
				if (u.attr_name == key)
					return u.attr_value;
			}
			SetUserData(new UserData(key, "", "MageEngine"));
			return "";
		}

		public string GetUserData(UserBasicData key)
		{
			return GetUserData(key.ToString());
		}

		public int GetUserDataInt(string key) {
			foreach (UserData u in user_datas) {
				if (u.attr_name == key.ToString())
					return int.Parse(u.attr_value);
			}
			SetUserData(new UserData(key, "0", "MageEngine"));
			return 0;
		}

		public int GetUserDataInt(UserBasicData key) {
			return GetUserDataInt(key.ToString());
		}

		public void SetUserData(UserData data) {
			bool found = false;

			foreach (UserData u in user_datas) {
				if (u.attr_name == data.attr_name) {
					u.attr_value = data.attr_value;
					u.attr_type = data.attr_type;
					found = true;
					break;
				}
			}

			if (!found) {
				user_datas.Add (data);
			}
		}

		public Character GetCharacter(string id)
		{
			foreach (Character u in characters) {
				if (u.id == id)
					return u;
			}
			return null;
		}

		public void SetCharacter(Character data) {
			//remove current data
			var item = characters.RemoveAll(x => x.id == data.id);
			characters.Add (data);

		}

		public CharacterItem GetCharacterItem(string id)
		{
			foreach (CharacterItem u in character_items) {
				if (u.id == id)
					return u;
			}
			return null;
		}

		public List<CharacterItem> GetUsedCharacterItems(string characterId)
		{
			List<CharacterItem> output = new List<CharacterItem>();

			foreach (CharacterItem u in character_items) {
				if (u.character_id == characterId)
					output.Add(u);
			}
			return output;
		}

		public void SetCharacterItem(CharacterItem data) {
			//remove current data
			var item = character_items.RemoveAll(x => x.id == data.id);
			character_items.Add (data);

		}

		public void RemoveCharacterItem(CharacterData data) {
			var item = character_items.RemoveAll(x => x.id == data.id);
		}
		
		public T GetUserData<T>() where T:BaseModel {
			string key = ApiHandler.instance.ApplicationKey + "_" + typeof(T).Name;

			if ("" != this.GetUserData(key)) {
				return BaseModel.CreateFromJSON<T>(this.GetUserData(key));
			} else {
				return null;
			}
			
		}
	}

	public enum UserStatus {
		DELETED = -1,
		ACTIVE = 1,	
		CLOSED = 2,
		CHANGE_INFO_REQUIRED = 3,
		CHANGE_PASSWORD_REQUIRED = 4,
		FIRST_LOGIN = 5,
		INACTIVE = 100
	}

	public enum UserBasicData {
		Version,
		StoredIAPTransactionIDs
	}
}


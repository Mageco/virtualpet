using System;
using System.Collections;
using System.Collections.Generic;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageApi.Models.Request;
using MageApi.Models.Response;
using UnityEngine;

namespace MageSDK.Client.Adaptors
{
    public class MageAdaptor
    {
        public static void GetApplicationDataFromServer(Action<List<ApplicationData>> onCompleteCallback, Action<int> onError = null, Action onTimeout = null, bool isQueueTask = true)
        {

            GetApplicationDataRequest r = new GetApplicationDataRequest();
            //call to login api
            MageEngine.instance.SendApi<GetApplicationDataResponse>(
                ApiSettings.API_GET_APPLICATION_DATA,
                r,
                (result) =>
                {
                    // store application data

                    onCompleteCallback(result.ApplicationDatas);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                },
                isQueueTask
            );
        }

        public static IEnumerator LoginWithDeviceID(Action<LoginResponse> onCompleteCallback, Action<int> onError = null, Action onTimeout = null)
        {
            ApiUtils.Log("[Time Checking]: Login device uuid started at: " + DateTime.Now.Subtract(MageEngine.instance._startTime).TotalSeconds);
            LoginRequest r = new LoginRequest(ApiSettings.LOGIN_DEVICE_UUID);

            //call to login api
            MageEngine.instance.SendApi<LoginResponse>(
                ApiSettings.API_LOGIN,
                r,
                (result) =>
                {
                    ApiUtils.Log("[Time Checking]: get response from server after: " + DateTime.Now.Subtract(MageEngine.instance._startTime).TotalSeconds);
                    ApiUtils.Log("[Time Checking]: user info:  " + result.User.ToJson());
                    onCompleteCallback(result);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                },
                false
            );

            yield return null;
        }

        ///<summary>Save Application Data to server</summary>
        public static void AdminSaveApplicationDataToServer(List<ApplicationData> applicationDatas, string unityAdminToken, Action successCallback = null, Action<int> onError = null, Action onTimeout = null)
        {
            UpdateApplicationDataRequest r = new UpdateApplicationDataRequest(applicationDatas, unityAdminToken);

            //call to login api
            MageEngine.instance.SendApi<UpdateApplicationDataResponse>(
                ApiSettings.API_UPDATE_APPLICATION_DATA,
                r,
                (result) =>
                {
                    if (null != successCallback)
                    {
                        successCallback();
                    }
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        ///<summary>Send list of user events to server</summary>
        public static void SendUserEventList(List<MageEvent> cachedEvent, Action onSendComplete, Action<int> onError = null, Action onTimeout = null)
        {
            SendUserEventListRequest r = new SendUserEventListRequest(cachedEvent);

            //call to login api
            MageEngine.instance.SendApi<SendUserEventListResponse>(
                ApiSettings.API_SEND_USER_EVENT_LIST,
                r,
                (result) =>
                {
                    onSendComplete();
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        ///<summary>Send user events to server</summary>
        public static void SendUserEvent(MageEvent t, Action onSendComplete, Action<int> onError = null, Action onTimeout = null)
        {
            SendUserEventRequest r = new SendUserEventRequest(t.eventName, t.eventDetail);
            //call to login api
            MageEngine.instance.SendApi<SendUserEventResponse>(
                ApiSettings.API_SEND_USER_EVENT,
                r,
                (result) =>
                {
                    onSendComplete();
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        public static void UploadImage(Texture2D image, Action<string> onUploadCompleteCallback, Action<int> onError = null, Action onTimeout = null)
        {
            UploadFileRequest r = new UploadFileRequest();
            r.SetUploadFile(image.EncodeToPNG());

            //call to login api
            MageEngine.instance.UploadFile<UploadFileResponse>(
                r,
                (result) =>
                {
                    ApiUtils.Log("Success: Upload file successfully");
                    ApiUtils.Log("Upload URL: " + result.UploadedURL);

                    MageEngine.instance.UpdateUserAvatar(image, result.UploadedURL, onUploadCompleteCallback);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        public static void GetUserByUserData(List<SearchData> searchDatas, Action<List<User>> onUploadCompleteCallback, Action<int> onError = null, Action onTimeout = null)
        {
            GetUsersByUserDataRequest r2 = new GetUsersByUserDataRequest();
            r2.SearchDatas = searchDatas;

            //call to login api
            MageEngine.instance.SendApi<GetUsersByUserDataResponse>(
                ApiSettings.API_GET_USERS_BY_USER_DATAS,
                r2,
                (result) =>
                {
                    onUploadCompleteCallback(result.Users);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        public static void GetUserBySocialId(string socialId, Action<List<User>> onUploadCompleteCallback, Action<int> onError = null, Action onTimeout = null)
        {
            GetUsersBySocialIdRequest r2 = new GetUsersBySocialIdRequest(socialId);

            //call to login api
            MageEngine.instance.SendApi<GetUsersBySocialIdResponse>(
                ApiSettings.API_GET_USERS_BY_SOCIAL_ID,
                r2,
                (result) =>
                {
                    onUploadCompleteCallback(result.Users);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        public static void GetServerTimestamp()
        {
            GetServerTimeRequest r2 = new GetServerTimeRequest();

            //call to login api
            MageEngine.instance.SendApi<GetServerTimeResponse>(
                ApiSettings.API_GET_SERVER_TIMESTAMP,
                r2,
                (result) =>
                {
                },
                (errorStatus) =>
                {
                },
                () =>
                {
                }
            );
        }

        public static void UpdateLeaderBoardInMage(string userId, string boardName, int score)
        {
            UpdateUserLeaderboardRequest r = new UpdateUserLeaderboardRequest();

            UserData tmp = new UserData(boardName, score.ToString(), "Leaderboard");
            r.LeaderboardDatas.Add(tmp);
            r.OtherUserId = userId;
            //call to login api
            MageEngine.instance.SendApi<UpdateUserLeaderboardResponse>(
                ApiSettings.API_UPDATE_USER_LEADER_BOARD,
                r,
                (result) =>
                {

                },
                (errorStatus) =>
                {
                },
                () =>
                {
                }
            );
        }

        public static void GetUserDataByIds(string[] userIdList, string dataName, Action<List<UserData>> onCompleteCallback, Action<int> onError = null, Action onTimeout = null)
        {
            GetUserDataByUserListRequest r2 = new GetUserDataByUserListRequest(userIdList, dataName);

            //call to login api
            MageEngine.instance.SendApi<GetUserDataByUserListResponse>(
                ApiSettings.API_GET_USER_DATA_BY_USER_LIST,
                r2,
                (result) =>
                {
                    onCompleteCallback(result.UserDatas);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                    if (onError != null)
                    {
                        onError(errorStatus);
                    }

                },
                () =>
                {
                    if (onTimeout != null)
                    {
                        onTimeout();
                    }
                }
            );
        }

        public static void SendMessage(string userId, MessageType messageType = MessageType.PushNotification, string title = "", string messageBody = "", string additionalData = "")
        {
            SendMessageRequest r = new SendMessageRequest(userId, MessageType.PushNotification, messageBody, title, additionalData);

            //call to login api
            MageEngine.instance.SendApi<SendMessageResponse>(
                ApiSettings.API_SEND_MESSAGE,
                r,
                (result) =>
                {
                    ApiUtils.Log("Messages result: " + result.ToJson());
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                },
                () =>
                {
                    //timeout handler here
                    ApiUtils.Log("Api call is timeout");
                }
            );
        }

        public static void UpdateUserMessageStatusToServer(string msgId, MessageStatus status)
        {

            UpdateMessageStatusRequest r = new UpdateMessageStatusRequest(msgId, status);

            //call to send action log api
            MageEngine.instance.SendApi<UpdateMessageStatusResponse>(
                ApiSettings.API_UPDATE_MESSAGE_STATUS,
                r,
                (result) =>
                {
                        //
                    },
                (errorStatus) =>
                {
                        //
                    },
                () =>
                {
                }
            );

        }

        public static void GetLeaderBoardFromMageServer(
            string fieldName,
            SelectBoardOption selectOption = SelectBoardOption.Both,
            Action<List<LeaderBoardItem>> onCompleteCallback = null,
            SortType sortMethod = SortType.Ascendent,
            int topLimit = 50,
            int nearByLimit = 10)
        {

            GetLeaderBoardRequest r = new GetLeaderBoardRequest()
            {
                DataName = fieldName,
                SelectOption = selectOption.ToString(),
                TopLimit = topLimit,
                NearByLimit = nearByLimit,
                SortMethod = sortMethod
            };
            //call to login api
            MageEngine.instance.SendApi<GetLeaderBoardResponse>(
                ApiSettings.API_GET_LEADER_BOARD,
                r,
                (result) =>
                {
                    ApiUtils.Log("Success: get leaderboard successfully");
                    ApiUtils.Log("Leaderboard result: " + result.ToJson());

                    if (null != onCompleteCallback)
                    {
                        onCompleteCallback(result.Leaders);
                    }
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                },
                () =>
                {
                    //timeout handler here
                    ApiUtils.Log("Api call is timeout");
                }
            );
        }

        public static void GetRandomFriend(Action<User> getRandomFriendCallback, string friendId = "")
        {
            GetUserProfileRequest r = new GetUserProfileRequest();
            if (friendId != "")
            {
                r.ProfileId = friendId;
            }

            MageEngine.instance.SendApi<GetUserProfileResponse>(
                ApiSettings.API_GET_USER_PROFILE,
                r,
                (result) =>
                {
                    ApiUtils.Log("Success: get user profile successfully");
                    ApiUtils.Log("Profile result: " + result.ToJson());
                    getRandomFriendCallback(result.UserProfile);
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                },
                () =>
                {
                    //timeout handler here
                    ApiUtils.Log("Api call is timeout");
                }
            );
        }

    }


}


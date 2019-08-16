using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using MageApi.Models.Request;

namespace Crs.Models.Request {
	[Serializable]
	public class SendQuestionAnswerRecordRequest: BaseRequest {

		public int TestRound;
		public int ClientSequence;
		public int LessonId;
		public int QuestionId;
		public int Status;
		public int IsLessonCompleted;
		public int DurationInSeconds;
		public string StartTime;
		public string EndTime;

		public SendQuestionAnswerRecordRequest() : base() {
		}

		public SendQuestionAnswerRecordRequest(int testRound, int clientSequence, int lessonId, int questionId, int status, int isLessonCompleted, int duration, DateTime startTime, DateTime endTime) : base() {
			this.TestRound = testRound;
			this.ClientSequence = clientSequence;
			this.LessonId = lessonId;
			this.QuestionId = questionId;
			this.Status = status;
			this.IsLessonCompleted = isLessonCompleted;
			this.DurationInSeconds = duration;
			this.StartTime = startTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
			this.EndTime = endTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
		} 
	}
}



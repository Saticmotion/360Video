using UnityEngine;

public enum InteractionType
{
	None,
	Text,
	Image,
	Video,
	MultipleChoice,
	Audio,
	FindArea,
	MultipleChoiceArea,
	MultipleChoiceImage,
	TabularData,
	Chapter
}

public class InteractionTypePicker : MonoBehaviour
{
	public bool answered = false;
	public InteractionType answer;

	public void OnEnable()
	{
		StartCoroutine(UIAnimation.FadeIn(GetComponent<RectTransform>(), GetComponent<CanvasGroup>()));
	}

	public void AnswerImage()
	{
		answered = true;
		answer = InteractionType.Image;
	}

	public void AnswerText()
	{
		answered = true;
		answer = InteractionType.Text;
	}

	public void AnswerVideo()
	{
		answered = true;
		answer = InteractionType.Video;
	}

	public void AnswerMultipleChoice()
	{
		answered = true;
		answer = InteractionType.MultipleChoice;
	}

	public void AnswerAudio()
	{
		answered = true;
		answer = InteractionType.Audio;
	}

	public void AnswerFindArea()
	{
		answered = true;
		answer = InteractionType.FindArea;
	}

	public void AnswerMultipleChoiceArea()
	{
		answered = true;
		answer = InteractionType.MultipleChoiceArea;
	}

	public void AnswerMultipleChoiceImage()
	{
		answered = true;
		answer = InteractionType.MultipleChoiceImage;
	}

	//TODO(Simon): Disable chapter option if no chapters defined. Also show message explaining why
	public void AnswerChapter()
	{
		answered = true;
		answer = InteractionType.Chapter;
		Debug.LogError("TODO(Simon): Disable chapter option if no chapters defined. Also show message explaining why");
	}
}

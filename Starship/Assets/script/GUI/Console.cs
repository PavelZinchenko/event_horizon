using UnityEngine;
using System.Linq;
using Services.Audio;
using Zenject;

public class Console : MonoBehaviour
{
    [Inject] private readonly ISoundPlayer _soundPlayer;

	public AudioClip InvitationSound;
	public ViewModel.PanelController MessagePrefab;

	public Console()
	{
		_self = new WeakReference<Console>(this);
	}

	public static Console Instance { get { return _self.Target; } }

	public void Message(string message)
	{
		if (_instance == null)
		{
			var canvas = FindObjectsOfType<Canvas>().Where(item => item.isRootCanvas).FirstOrDefault();
			if (canvas == null)
				return;
			
			_instance = (ViewModel.PanelController)GameObject.Instantiate(MessagePrefab);
			var rectTransform = _instance.GetComponent<RectTransform>();
			rectTransform.SetParent(canvas.transform);
			rectTransform.anchoredPosition = Vector2.zero;
			_instance.transform.localScale = Vector3.one;
		}
		
		_instance.Open();
		_instance.GetComponentInChildren<UnityEngine.UI.Text>().text = message;

		_soundPlayer.Play(InvitationSound);
		_textDelay = 5f;
	}
	
	void Update() 
	{
		if (_textDelay < 0)
			return;

		_textDelay -= Time.unscaledDeltaTime;
		if (_textDelay < 0 && _instance != null)
			_instance.Close();
	}

	private ViewModel.PanelController _instance;
	private float _textDelay;
	private static WeakReference<Console> _self;
}
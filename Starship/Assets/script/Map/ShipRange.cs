using Services.Audio;
using UnityEngine;
using Zenject;

public class ShipRange : Circle
{
    [Inject] private readonly ISoundPlayer _soundPlayer;

	public float LifeTime = 2f;
	public AudioClip Sound;

	public void Refresh()
	{
		gameObject.SetActive(true);
		_lifetime = 1;
		_soundPlayer.Play(Sound);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		_lifetime = 0;
	}

    protected override void Update()
	{
		base.Update();

		_lifetime = Mathf.Clamp01(_lifetime - Time.deltaTime/LifeTime);

		var renderer = GetComponent<Renderer>();
		if (renderer != null)
			renderer.material.color = Color.Lerp(Color.clear, Color, Mathf.Sqrt(_lifetime));

		if (_lifetime <= 0)
			gameObject.SetActive(false);
	}

	private float _lifetime;
}
